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

            return ServiceResult<Room>.Ok(
                newRoom,
                $"[green]→ {direction}[/]",
                $"[green]You travel {direction} and arrive at {newRoom.Name}.[/]");
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
            var output = $"[yellow]Character:[/] {player.Name}\n" +
                        $"[green]Health:[/] {player.Health}\n" +
                        $"[cyan]Experience:[/] {player.Experience}";

            _logger.LogInformation("Displaying stats for player {PlayerName}", player.Name);

            return ServiceResult.Ok(
                "[cyan]Viewing stats[/]",
                output);
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
            var output = $"[magenta]Equipment:[/] {(player.Equipment != null ? "Equipped" : "None")}\n" +
                        $"[blue]Abilities:[/] {player.Abilities?.Count ?? 0}";

            _logger.LogInformation("Displaying inventory for player {PlayerName}", player.Name);

            return ServiceResult.Ok(
                "[magenta]Viewing inventory[/]",
                output);
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
    /// TODO: Implement monster attack logic
    /// </summary>
    public ServiceResult AttackMonster()
    {
        try
        {
            var player = _context.Players
                .Include(p => p.Room)
                .ThenInclude(r => r.Monsters)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Weapon)
                .FirstOrDefault(); 

            if (player?.Room?.Monsters == null || !player.Room.Monsters.Any())
            {
                return ServiceResult.Fail("No monsters here!", "There are no monsters in this room to attack.");
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

            monster.Health -= damage;

            sb.AppendLine($"[yellow]Combat Log:[/]");
            sb.AppendLine($"You attack [red]{monster.Name}[/] with {player.Equipment?.Weapon?.Name ?? "fists"}!");
            sb.AppendLine($"Dealt [red]{damage}[/] damage.");

            if (monster.Health <= 0)
            {
                sb.AppendLine($"[gold1]VICTORY![/] {monster.Name} has been defeated!");

                player.Experience += 50;
                sb.AppendLine($"You gained [cyan]50 XP[/].");

                _context.Monsters.Remove(monster);
            }
            else
            {
                sb.AppendLine($"[red]{monster.Name}[/] has {monster.Health} HP remaining.");
            }

            _context.SaveChanges();

            return ServiceResult.Ok("Attack successful", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during attack");
            return ServiceResult.Fail("Attack failed", $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// TODO: Implement ability usage logic
    /// </summary>
    public ServiceResult UseAbilityOnMonster()
    {
        try
        {
            var player = _context.Players
                .Include(p => p.Abilities)
                .Include(p => p.Room)
                .ThenInclude(r => r.Monsters)
                .FirstOrDefault();

            if (player == null || player.Room?.Monsters == null || !player.Room.Monsters.Any())
            {
                return ServiceResult.Fail("Cannot use ability", "There are no monsters here to target.");
            }

            if (!player.Abilities.Any())
            {
                return ServiceResult.Fail("No abilities", "You haven't learned any abilities yet!");
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
            sb.AppendLine($"[yellow]Ability Log:[/]");
            sb.AppendLine($"You used [cyan]{ability.Name}[/] on [red]{monster.Name}[/]!");

            if (ability is ShoveAbility shove)
            {
                int damage = shove.Damage;
                monster.Health -= damage;
                sb.AppendLine($"[red]{monster.Name}[/] is shoved back {shove.Distance} feet! (Dealt {damage} dmg)");
            }
            else
            {
                int damage = 5;
                monster.Health -= damage;
                sb.AppendLine($"The ability hits [red]{monster.Name}[/] for {damage} damage.");
            }

            if (monster.Health <= 0)
            {
                sb.AppendLine($"[gold1]{monster.Name}[/] has been defeated!");
                _context.Monsters.Remove(monster);
            }
            else
            {
                sb.AppendLine($"[red]{monster.Name}[/] has {monster.Health} HP remaining.");
            }

            _context.SaveChanges();

            return ServiceResult.Ok($"Used {ability.Name}", sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using ability");
            return ServiceResult.Fail("Ability failed", $"Error: {ex.Message}");
        }
    }
}
