using ConsoleRpg.Models;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ConsoleRpg.Services;

/// <summary>
/// Handles all player-related actions and interactions
/// Separated from GameEngine to follow Single Responsibility Principle
/// Returns ServiceResult objects to decouple from UI concerns
/// </summary>
public class PlayerService
{
    private readonly GameContext _context;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(GameContext context, ILogger<PlayerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Move the player to a different room
    /// </summary>
    public ServiceResult<Room> MoveToRoom(Player player, Room currentRoom, int? roomId, string direction)
    {
        try
        {
            if (!roomId.HasValue)
            {
                return ServiceResult<Room>.Fail(
                    $"[red]Cannot go {direction}[/]",
                    $"[red]You cannot go {direction} from here - there is no exit in that direction.[/]");
            }

            var newRoom = _context.Rooms
                .Include(r => r.Players)
                .Include(r => r.Monsters)
                .Include(r => r.NorthRoom)
                .Include(r => r.SouthRoom)
                .Include(r => r.EastRoom)
                .Include(r => r.WestRoom)
                .FirstOrDefault(r => r.Id == roomId.Value);

            if (newRoom == null)
            {
                _logger.LogWarning("Attempted to move to non-existent room {RoomId}", roomId.Value);
                return ServiceResult<Room>.Fail(
                    $"[red]Room not found[/]",
                    $"[red]Error: Room {roomId.Value} does not exist.[/]");
            }

            // Update player's room
            player.RoomId = roomId.Value;
            _context.SaveChanges();

            _logger.LogInformation("Player {PlayerName} moved {Direction} to {RoomName}",
                player.Name, direction, newRoom.Name);

            var detailedOutput = new System.Text.StringBuilder();
            detailedOutput.AppendLine($"[cyan]You travel {direction.ToLower()}...[/]");
            detailedOutput.AppendLine();
            detailedOutput.AppendLine($"[yellow bold]>>> {newRoom.Name} <<<[/]");
            detailedOutput.AppendLine($"[dim]{newRoom.Description}[/]");

            // Warn about monsters if present
            if (newRoom.Monsters != null && newRoom.Monsters.Any())
            {
                detailedOutput.AppendLine();
                detailedOutput.AppendLine($"[red bold] Warning: {newRoom.Monsters.Count} hostile creature(s) detected![/]");
            }

            return ServiceResult<Room>.Ok(
                newRoom,
                $"[green]→ {direction}[/]",
                detailedOutput.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving player {PlayerName} to room {RoomId}", player.Name, roomId);
            return ServiceResult<Room>.Fail(
                "[red]Movement failed[/]",
                $"[red]An error occurred while moving: {ex.Message}[/]");
        }
    }

    /// <summary>
    /// Show player character stats
    /// </summary>
    public ServiceResult ShowCharacterStats(Player player)
    {
        try
        {
            var output = new System.Text.StringBuilder();
            output.AppendLine($"[yellow bold]=== Character Sheet ===[/]");
            output.AppendLine();
            output.AppendLine($"[cyan]Name:[/] {player.Name}");
            output.AppendLine($"[green]Health:[/] {player.Health}/{player.MaxHealth} HP");
            output.AppendLine($"[yellow]Level:[/] {player.Level}");
            output.AppendLine($"[magenta]Experience:[/] {player.Experience} XP ({player.Experience % 100}/100 to next level)");

            if (player.Equipment != null)
            {
                output.AppendLine();
                output.AppendLine($"[yellow]Equipment:[/]");
                output.AppendLine($"  Weapon: {player.Equipment.Weapon?.Name ?? "None"} (Attack: {player.Equipment.Weapon?.Attack ?? 0})");
                output.AppendLine($"  Armor: {player.Equipment.Armor?.Name ?? "None"} (Defense: {player.Equipment.Armor?.Defense ?? 0})");
            }

            _logger.LogInformation("Displaying stats for player {PlayerName}", player.Name);

            return ServiceResult.Ok(
                "[cyan]Viewing stats[/]",
                output.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying stats for player {PlayerName}", player.Name);
            return ServiceResult.Fail(
                "[red]Error[/]",
                $"[red]Failed to display stats: {ex.Message}[/]");
        }
    }

    /// <summary>
    /// Show player inventory and stats
    /// </summary>
    public ServiceResult ShowInventory(Player player)
    {
        try
        {
            var equipment = new System.Text.StringBuilder();
            equipment.AppendLine("[yellow bold]=== Inventory ===[/]");
            equipment.AppendLine();
            equipment.AppendLine("[magenta]--- Equipped ---[/]");
            equipment.AppendLine($"Weapon: {(player.Equipment?.Weapon != null ? player.Equipment.Weapon.Name : "None")}");
            equipment.AppendLine($"Armor:  {(player.Equipment?.Armor != null ? player.Equipment.Armor.Name : "None")}");

            equipment.AppendLine();
            equipment.AppendLine("[blue]--- Backpack ---[/]");

            if (player.Inventory?.Items != null && player.Inventory.Items.Any())
            {
                foreach (var item in player.Inventory.Items)
                {
                    equipment.AppendLine($"- {item.Name} ({item.Type}) - Wt: {item.Weight}");
                }
            }
            else
            {
                equipment.AppendLine("[dim]Empty[/]");
            }

            _logger.LogInformation("Displaying inventory for player {PlayerName}", player.Name);

            return ServiceResult.Ok(
                "[magenta]Inventory[/]",
                equipment.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying inventory for player {PlayerName}", player.Name);
            return ServiceResult.Fail(
                "[red]Error[/]",
                $"[red]Failed to display inventory: {ex.Message}[/]");
        }
    }

    /// <summary>
    /// Attack a monster in the current room
    /// </summary>
    public ServiceResult<Monster> AttackMonster()
    {
        try
        {
            var player = _context.Players
                .Include(p => p.Room)
                .ThenInclude(r => r.Monsters)
                .ThenInclude(m => m.LootItem)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Weapon)
                .Include(p => p.Inventory)
                .ThenInclude(i => i.Items)
                .FirstOrDefault();

            if (player?.Room?.Monsters == null || !player.Room.Monsters.Any())
            {
                return ServiceResult<Monster>.Fail("No monsters here!", "There are no monsters in this room to attack.");
            }

            var monster = player.Room.Monsters.First();
            if (player.Room.Monsters.Count > 1)
            {
                monster = AnsiConsole.Prompt(
                    new SelectionPrompt<Monster>()
                        .Title("Select a [red]monster[/] to attack:")
                        .AddChoices(player.Room.Monsters)
                        .UseConverter(m => $"{m.Name} (HP: {m.Health})"));
            }

            var sb = new System.Text.StringBuilder();

            int damage = player.Equipment?.Weapon?.Attack ?? 1;
            int actualDamage = monster.ReceiveAttack(damage);

            sb.AppendLine($"You attack [red]{monster.Name}[/] with {player.Equipment?.Weapon?.Name ?? "fists"}!");
            sb.AppendLine($"Dealt [red]{actualDamage}[/] damage. (Monster Armor absorbed {damage - actualDamage})");

            if (monster.Health <= 0)
            {
                sb.AppendLine();
                sb.AppendLine($"[gold1 bold]*** VICTORY! ***[/]");
                sb.AppendLine($"[gold1]{monster.Name} has been defeated![/]");

                string xpMessage = player.GainExperience(50);
                sb.AppendLine();
                sb.AppendLine(xpMessage);

                if (monster.LootItem != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"[gold1 bold]*** LOOT DROP! ***[/]");
                    sb.AppendLine($"The monster dropped: [cyan]{monster.LootItem.Name}[/]!");

                    if (player.Inventory == null)
                    {
                        player.Inventory = new ConsoleRpgEntities.Models.Equipments.Inventory { PlayerId = player.Id };
                        _context.Add(player.Inventory);
                    }

                    player.Inventory.Items.Add(monster.LootItem);
                    monster.LootItemId = null;
                }

                _context.Monsters.Remove(monster);
            }
            else
            {
                sb.AppendLine($"[red]{monster.Name}[/] has [yellow]{monster.Health} HP[/] remaining.");
            }

            _context.SaveChanges();

            // Return the specific monster that was targeted
            return ServiceResult<Monster>.Ok(monster, "Attack successful", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during attack");
            return ServiceResult<Monster>.Fail("Attack failed", $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Use an ability on a monster
    /// </summary>
    public ServiceResult<Monster> UseAbilityOnMonster()
    {
        try
        {
            var player = _context.Players
                .Include(p => p.Abilities)
                .Include(p => p.Room)
                .ThenInclude(r => r.Monsters)
                .ThenInclude(m => m.LootItem)
                .Include(p => p.Inventory)
                .ThenInclude(i => i.Items)
                .FirstOrDefault();

            if (player == null || player.Room?.Monsters == null || !player.Room.Monsters.Any())
            {
                return ServiceResult<Monster>.Fail("Cannot use ability", "There are no monsters here to target.");
            }

            if (!player.Abilities.Any())
            {
                return ServiceResult<Monster>.Fail("No abilities", "You haven't learned any abilities yet!");
            }

            var ability = AnsiConsole.Prompt(
                new SelectionPrompt<Ability>()
                    .Title("Choose an [cyan]Ability[/]:")
                    .AddChoices(player.Abilities)
                    .UseConverter(a => $"{a.Name} ({a.AbilityType})"));

            var monster = player.Room.Monsters.First();
            if (player.Room.Monsters.Count > 1)
            {
                monster = AnsiConsole.Prompt(
                    new SelectionPrompt<Monster>()
                        .Title("Select a [red]target[/]:")
                        .AddChoices(player.Room.Monsters)
                        .UseConverter(m => $"{m.Name} (HP: {m.Health})"));
            }

            var sb = new System.Text.StringBuilder();

            string combatLog = ability.Activate(player, monster);
            sb.AppendLine(combatLog);

            if (monster.Health <= 0)
            {
                sb.AppendLine();
                sb.AppendLine($"[gold1 bold]*** VICTORY! ***[/]");
                sb.AppendLine($"[gold1]{monster.Name}[/] has been defeated!");

                string xpMessage = player.GainExperience(50);
                sb.AppendLine();
                sb.AppendLine(xpMessage);

                if (monster.LootItem != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"[gold1 bold]*** LOOT DROP! ***[/]");
                    sb.AppendLine($"The monster dropped: [cyan]{monster.LootItem.Name}[/]!");

                    if (player.Inventory == null)
                    {
                        player.Inventory = new ConsoleRpgEntities.Models.Equipments.Inventory { PlayerId = player.Id };
                        _context.Add(player.Inventory);
                    }

                    player.Inventory.Items.Add(monster.LootItem);
                    monster.LootItemId = null;
                }

                _context.Monsters.Remove(monster);
            }
            else
            {
                sb.AppendLine($"[red]{monster.Name}[/] has [yellow]{monster.Health} HP[/] remaining.");
            }

            _context.SaveChanges();

            return ServiceResult<Monster>.Ok(monster, $"Used {ability.Name}", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using ability");
            return ServiceResult<Monster>.Fail("Ability failed", $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Equip an item from inventory
    /// </summary>
    public ServiceResult EquipItem()
    {
        try
        {
            var player = _context.Players
                .Include(p => p.Inventory)
                .ThenInclude(i => i.Items)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Weapon)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Armor)
                .FirstOrDefault();

            if (player?.Inventory?.Items == null || !player.Inventory.Items.Any())
            {
                return ServiceResult.Fail("Inventory empty", "You have no items to equip.");
            }

            var equippables = player.Inventory.Items
                .Where(i => i.Type == "Weapon" || i.Type == "Armor")
                .ToList();

            if (!equippables.Any())
            {
                return ServiceResult.Fail("No gear", "You have items, but none of them are weapons or armor.");
            }

            var itemToEquip = AnsiConsole.Prompt(
                new SelectionPrompt<ConsoleRpgEntities.Models.Equipments.Item>()
                    .Title("Select an item to [green]Equip[/]:")
                    .PageSize(10)
                    .AddChoices(equippables)
                    .UseConverter(i => $"{i.Name} ({i.Type}) - Power: {Math.Max(i.Attack, i.Defense)}"));

            var sb = new System.Text.StringBuilder();

            if (itemToEquip.Type == "Weapon")
            {
                if (player.Equipment.Weapon != null)
                {
                    player.Inventory.Items.Add(player.Equipment.Weapon);
                    sb.AppendLine($"You put away your [dim]{player.Equipment.Weapon.Name}[/].");
                }

                player.Equipment.Weapon = itemToEquip;
                sb.AppendLine($"You equipped [cyan bold]{itemToEquip.Name}[/]! (Attack: {itemToEquip.Attack})");
            }
            else if (itemToEquip.Type == "Armor")
            {
                if (player.Equipment.Armor != null)
                {
                    player.Inventory.Items.Add(player.Equipment.Armor);
                    sb.AppendLine($"You took off your [dim]{player.Equipment.Armor.Name}[/].");
                }

                player.Equipment.Armor = itemToEquip;
                sb.AppendLine($"You equipped [cyan bold]{itemToEquip.Name}[/]! (Defense: {itemToEquip.Defense})");
            }

            player.Inventory.Items.Remove(itemToEquip);

            _context.SaveChanges();

            return ServiceResult.Ok("Equipped Item", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error equipping item");
            return ServiceResult.Fail("Error", $"Could not equip item: {ex.Message}");
        }
    }

    /// <summary>
    /// Switches the active player character
    /// </summary>
    public Player SwitchCharacter()
    {
        var players = _context.Players.OrderBy(p => p.Name).ToList();

        if (!players.Any())
        {
            AnsiConsole.MarkupLine("[red]No players found in database![/]");
            return null;
        }

        var selectedPlayer = AnsiConsole.Prompt(
            new SelectionPrompt<Player>()
                .Title("Select a [green]Character[/] to play:")
                .PageSize(10)
                .AddChoices(players)
                .UseConverter(p => $"{p.Name} (Lvl {p.Level})"));

        // Re-load full player data
        return _context.Players
            .Include(p => p.Room)
            .Include(p => p.Equipment)
            .ThenInclude(e => e.Weapon)
            .Include(p => p.Equipment)
            .ThenInclude(e => e.Armor)
            .Include(p => p.Inventory)
            .ThenInclude(i => i.Items)
            .Include(p => p.Abilities)
            .FirstOrDefault(p => p.Id == selectedPlayer.Id);
    }
}