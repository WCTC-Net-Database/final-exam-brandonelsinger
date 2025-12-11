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
/// Service class handling all administrative and CRUD operations.
/// Provides functionality for managing Characters, Abilities, Rooms, and advanced queries.
/// 
/// Feature Groups:
/// - Basic CRUD: Add, Edit, Display, Search characters
/// - C-Level: Ability management (add to character, display abilities)
/// - B-Level: Room management (add room, display room details)
/// - A-Level: Advanced queries (filter by attribute, world population, equipment search)
/// 
/// All operations use Spectre.Console for rich terminal UI and include logging.
/// </summary>
public class AdminService
{
    private readonly GameContext _context;
    private readonly ILogger<AdminService> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    /// <param name="context">Database context for entity operations</param>
    /// <param name="logger">Logger for operation tracking and debugging</param>
    public AdminService(GameContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Basic CRUD Operations

    /// <summary>
    /// Creates a new player character and saves to database.
    /// Prompts for: name, health, experience, and optional starting room.
    /// Uses Spectre.Console TextPrompt with validation.
    /// </summary>
    public void AddCharacter()
    {
        try
        {
            _logger.LogInformation("User selected Add Character");
            AnsiConsole.MarkupLine("[yellow]=== Add New Character ===[/]");

            var name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter character [green]name[/]:")
                    .AllowEmpty()
                    .Validate(n => !string.IsNullOrWhiteSpace(n)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Name cannot be empty![/]")));

            var health = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [green]health[/]:")
                    .AllowEmpty()
                    .Validate(h => h > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Health must be greater than 0![/]")));

            var experience = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [green]experience[/]:")
                    .AllowEmpty()
                    .Validate(e => e >= 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Experience cannot be negative![/]")));

            var player = new Player
            {
                Name = name,
                Health = health,
                MaxHealth = health,
                Experience = experience
            };

            // Allow user to assign the character to a room
            var rooms = _context.Rooms.OrderBy(r => r.Name).ToList();
            if (rooms.Any() && AnsiConsole.Confirm("Would you like to assign this character to a [green]room[/]?"))
            {
                var selectedRoom = AnsiConsole.Prompt(
                    new SelectionPrompt<Room>()
                        .Title("Select a [green]Room[/]:")
                        .PageSize(10)
                        .AddChoices(rooms)
                        .UseConverter(r => r.Name));

                player.RoomId = selectedRoom.Id;
                AnsiConsole.MarkupLine($"[cyan]{name}[/] will start in [cyan]{selectedRoom.Name}[/].");
            }

            _context.Players.Add(player);
            _context.SaveChanges();

            _logger.LogInformation("Character {Name} added to database with Id {Id}", name, player.Id);
            AnsiConsole.MarkupLine($"[green]Character '{name}' added successfully![/]");
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding character");
            AnsiConsole.MarkupLine($"[red]Error adding character: {ex.Message}[/]");
            PressAnyKey();
        }
    }

    /// <summary>
    /// Edit an existing character's properties
    /// </summary>
    public void EditCharacter()
    {
        try
        {
            _logger.LogInformation("User selected Edit Character");
            AnsiConsole.MarkupLine("[yellow]=== Edit Character ===[/]");

            var id = AnsiConsole.Ask<int>("Enter character [green]ID[/] to edit:");

            var player = _context.Players.Find(id);
            if (player == null)
            {
                _logger.LogWarning("Character with Id {Id} not found", id);
                AnsiConsole.MarkupLine($"[red]Character with ID {id} not found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"Editing: [cyan]{player.Name}[/]");

            if (AnsiConsole.Confirm("Update name?"))
            {
                player.Name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter new [green]name[/]:")
                        .AllowEmpty()
                        .Validate(n => !string.IsNullOrWhiteSpace(n)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Name cannot be empty![/]")));
            }

            if (AnsiConsole.Confirm("Update health?"))
            {
                player.Health = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter new [green]health[/]:")
                        .AllowEmpty()
                        .Validate(h => h > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Health must be greater than 0![/]")));
            }

            if (AnsiConsole.Confirm("Update experience?"))
            {
                player.Experience = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter new [green]experience[/]:")
                        .AllowEmpty()
                        .Validate(e => e >= 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Experience cannot be negative![/]")));
            }

            _context.SaveChanges();

            _logger.LogInformation("Character {Name} (Id: {Id}) updated", player.Name, player.Id);
            AnsiConsole.MarkupLine($"[green]Character '{player.Name}' updated successfully![/]");
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing character");
            AnsiConsole.MarkupLine($"[red]Error editing character: {ex.Message}[/]");
            PressAnyKey();
        }
    }

    /// <summary>
    /// Display all characters in the database
    /// </summary>
    public void DisplayAllCharacters()
    {
        try
        {
            _logger.LogInformation("User selected Display All Characters");
            AnsiConsole.MarkupLine("[yellow]=== All Characters ===[/]");

            var players = _context.Players.Include(p => p.Room).ToList();

            if (!players.Any())
            {
                AnsiConsole.MarkupLine("[red]No characters found.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Level");
                table.AddColumn("Health");
                table.AddColumn("Experience");
                table.AddColumn("Location");

                foreach (var player in players)
                {
                    table.AddRow(
                        player.Id.ToString(),
                        player.Name,
                        player.Level.ToString(),
                        player.Health.ToString(),
                        player.Experience.ToString(),
                        player.Room?.Name ?? "Unknown"
                    );
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying all characters");
            AnsiConsole.MarkupLine($"[red]Error displaying characters: {ex.Message}[/]");
        }
    }

    /// <summary>
    /// Search for characters by name
    /// </summary>
    public void SearchCharacterByName()
    {
        try
        {
            _logger.LogInformation("User selected Search Character");
            AnsiConsole.MarkupLine("[yellow]=== Search Character ===[/]");

            var searchName = AnsiConsole.Ask<string>("Enter character [green]name[/] to search:");

            var players = _context.Players
                .Include(p => p.Room)
                .Where(p => p.Name.ToLower().Contains(searchName.ToLower()))
                .ToList();

            if (!players.Any())
            {
                _logger.LogInformation("No characters found matching '{SearchName}'", searchName);
                AnsiConsole.MarkupLine($"[red]No characters found matching '{searchName}'.[/]");
            }
            else
            {
                _logger.LogInformation("Found {Count} character(s) matching '{SearchName}'", players.Count, searchName);

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Level");
                table.AddColumn("Health");
                table.AddColumn("Experience");
                table.AddColumn("Location");

                foreach (var player in players)
                {
                    table.AddRow(
                        player.Id.ToString(),
                        player.Name,
                        player.Level.ToString(),
                        player.Health.ToString(),
                        player.Experience.ToString(),
                        player.Room?.Name ?? "Unknown"
                    );
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for characters");
            AnsiConsole.MarkupLine($"[red]Error searching characters: {ex.Message}[/]");
        }
    }

    #endregion

    #region C-Level Requirements

    /// <summary>
    /// C-LEVEL REQUIREMENT: Add Ability to Character
    /// Allows associating an existing ability with a player character.
    /// Uses the PlayerAbilities many-to-many join table.
    /// 
    /// Process:
    /// 1. Display list of characters using SelectionPrompt
    /// 2. Display available abilities from database
    /// 3. Check for duplicates (prevent adding same ability twice)
    /// 4. Add ability to player's collection and save
    /// </summary>
    public void AddAbilityToCharacter()
    {
        try
        {
            _logger.LogInformation("User selected Add Ability to Character");
            AnsiConsole.MarkupLine("[yellow]=== Add Ability to Character ===[/]");

            var players = _context.Players.OrderBy(p => p.Id).ToList();
            if (!players.Any())
            {
                AnsiConsole.MarkupLine("[red]No players found.[/]");
                PressAnyKey();
                return;
            }

            var playerChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Player>()
                    .Title("Select a [green]Character[/]:")
                    .PageSize(10)
                    .AddChoices(players)
                    .UseConverter(p => $"{p.Name} (Id: {p.Id})"));

            var abilities = _context.Abilities.OrderBy(a => a.Name).ToList();
            if (!abilities.Any())
            {
                AnsiConsole.MarkupLine("[red]No abilities found in database.[/]");
                PressAnyKey();
                return;
            }

            var abilityChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Ability>()
                    .Title($"Select an [green]Ability[/] to give {playerChoice.Name}:")
                    .PageSize(10)
                    .AddChoices(abilities)
                    .UseConverter(a => $"{a.Name} ({a.AbilityType})"));

            var playerToUpdate = _context.Players
                .Include(p => p.Abilities)
                .First(p => p.Id == playerChoice.Id);

            if (playerToUpdate.Abilities.Any(a => a.Id == abilityChoice.Id))
            {
                AnsiConsole.MarkupLine($"[yellow]{playerToUpdate.Name} already knows {abilityChoice.Name}![/]");
            }
            else
            {
                playerToUpdate.Abilities.Add(abilityChoice);
                _context.SaveChanges();
                AnsiConsole.MarkupLine($"[green]Success![/] {playerToUpdate.Name} learned {abilityChoice.Name}.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding ability");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    /// <summary>
    /// C-LEVEL REQUIREMENT: Display Character Abilities
    /// Shows all abilities learned by a selected character.
    /// Uses .Include(p => p.Abilities) for eager loading.
    /// 
    /// Displays ability-specific stats based on type:
    /// - ShoveAbility: Damage and Distance
    /// - FireballAbility: Damage and Distance
    /// - HealAbility: Heal amount
    /// - CombatAbility: Bonus damage
    /// </summary>
    public void DisplayCharacterAbilities()
    {
        try
        {
            _logger.LogInformation("User selected Display Character Abilities");
            AnsiConsole.MarkupLine("[yellow]=== Display Character Abilities ===[/]");

            var players = _context.Players.OrderBy(p => p.Id).ToList();
            if (!players.Any())
            {
                AnsiConsole.MarkupLine("[red]No players found.[/]");
                PressAnyKey();
                return;
            }

            var playerChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Player>()
                    .Title("Select a [green]Character[/]:")
                    .PageSize(10)
                    .AddChoices(players)
                    .UseConverter(p => $"{p.Name}"));

            var player = _context.Players
                .Include(p => p.Abilities)
                .FirstOrDefault(p => p.Id == playerChoice.Id);

            AnsiConsole.MarkupLine($"Abilities for [cyan]{player.Name}[/]:");

            if (!player.Abilities.Any())
            {
                AnsiConsole.MarkupLine("[dim]This character has no abilities.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("Name");
                table.AddColumn("Type");
                table.AddColumn("Description");
                table.AddColumn("Stats");

                foreach (var ability in player.Abilities)
                {
                    string stats = "-";

                    if (ability is ShoveAbility shove)
                    {
                        stats = $"Dmg: {shove.Damage}, Dist: {shove.Distance}";
                    }
                    else if (ability is FireballAbility fireball)
                    {
                        stats = $"Dmg: {fireball.Damage}, Dist: {fireball.Distance}";
                    }
                    else if (ability is HealAbility heal)
                    {
                        stats = $"Heal: {Math.Abs(heal.Damage)}";
                    }
                    else if (ability is CombatAbility combat)
                    {
                        stats = $"Bonus Dmg: {combat.Damage}";
                    }

                    table.AddRow(
                        ability.Name,
                        ability.AbilityType,
                        ability.Description,
                        stats
                    );
                }
                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying abilities");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    #endregion

    #region B-Level Requirements

    /// <summary>
    /// B-LEVEL REQUIREMENT: Add New Room
    /// Creates a new room and connects it to an existing room.
    /// Automatically sets up bidirectional navigation (new room connects back).
    /// 
    /// Process:
    /// 1. Prompt for room name and description
    /// 2. Select existing room to connect FROM
    /// 3. Choose direction (North, South, East, West)
    /// 4. Calculate X/Y coordinates based on source room
    /// 5. Optionally add a monster to the new room
    /// 6. Optionally move a character to the new room
    /// </summary>
    public void AddRoom()
    {
        try
        {
            _logger.LogInformation("User selected Add Room");
            AnsiConsole.MarkupLine("[yellow]=== Add New Room ===[/]");

            var name = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter room [green]name[/]:")
                    .AllowEmpty()
                    .Validate(n => !string.IsNullOrWhiteSpace(n)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Name cannot be empty![/]")));

            var description = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter room [green]description[/]:")
                    .AllowEmpty()
                    .Validate(d => !string.IsNullOrWhiteSpace(d)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Description cannot be empty![/]")));

            var existingRooms = _context.Rooms.ToList();
            var sourceRoom = AnsiConsole.Prompt(
                new SelectionPrompt<Room>()
                    .Title("Select a room to connect [green]FROM[/]:")
                    .PageSize(10)
                    .AddChoices(existingRooms)
                    .UseConverter(r => $"{r.Name} ({r.X}, {r.Y})"));

            var direction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Where is the new room located relative to [cyan]{sourceRoom.Name}[/]?")
                    .AddChoices("North", "South", "East", "West"));

            var newRoom = new Room
            {
                Name = name,
                Description = description,
                Monsters = new List<Monster>()
            };

            switch (direction)
            {
                case "North":
                    newRoom.X = sourceRoom.X;
                    newRoom.Y = sourceRoom.Y + 1;
                    sourceRoom.NorthRoom = newRoom;
                    newRoom.SouthRoom = sourceRoom;
                    break;
                case "South":
                    newRoom.X = sourceRoom.X;
                    newRoom.Y = sourceRoom.Y - 1;
                    sourceRoom.SouthRoom = newRoom;
                    newRoom.NorthRoom = sourceRoom;
                    break;
                case "East":
                    newRoom.X = sourceRoom.X + 1;
                    newRoom.Y = sourceRoom.Y;
                    sourceRoom.EastRoom = newRoom;
                    newRoom.WestRoom = sourceRoom;
                    break;
                case "West":
                    newRoom.X = sourceRoom.X - 1;
                    newRoom.Y = sourceRoom.Y;
                    sourceRoom.WestRoom = newRoom;
                    newRoom.EastRoom = sourceRoom;
                    break;
            }

            if (_context.Rooms.Any(r => r.X == newRoom.X && r.Y == newRoom.Y))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] There is already a room at ({newRoom.X}, {newRoom.Y})!");
                PressAnyKey();
                return;
            }

            if (AnsiConsole.Confirm("Would you like to add a [red]Monster[/] to this room?"))
            {
                var monsterName = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter monster [green]name[/]:")
                        .AllowEmpty()
                        .Validate(n => !string.IsNullOrWhiteSpace(n)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Monster name cannot be empty![/]")));

                var monsterHealth = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter monster [green]health[/]:")
                        .AllowEmpty()
                        .Validate(h => h > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Health must be greater than 0![/]")));

                var monsterType = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [green]Monster Type[/]:")
                        .AddChoices("Goblin", "Beast", "Undead", "Bandit"));

                Monster newMonster = null;

                switch (monsterType)
                {
                    case "Goblin":
                        newMonster = new Goblin { Sneakiness = 5 };
                        break;
                    case "Beast":
                        newMonster = new Beast();
                        break;
                    case "Undead":
                        newMonster = new Undead();
                        break;
                    case "Bandit":
                        newMonster = new Bandit();
                        break;
                }

                if (newMonster != null)
                {
                    newMonster.Name = monsterName;
                    newMonster.Health = monsterHealth;
                    newMonster.AggressionLevel = 10;
                    newMonster.MonsterType = monsterType;
                    newRoom.Monsters.Add(newMonster);

                    AnsiConsole.MarkupLine($"[green]Added[/] {monsterType} '{monsterName}' to the room.");
                }
            }

            // Option to move an existing character to this new room
            var players = _context.Players.OrderBy(p => p.Name).ToList();
            if (players.Any() && AnsiConsole.Confirm("Would you like to move an existing [cyan]Character[/] to this room?"))
            {
                var selectedPlayer = AnsiConsole.Prompt(
                    new SelectionPrompt<Player>()
                        .Title("Select a [cyan]Character[/] to move:")
                        .PageSize(10)
                        .AddChoices(players)
                        .UseConverter(p => $"{p.Name} (Id: {p.Id})"));

                selectedPlayer.RoomId = null; // Will be set after room is saved
                newRoom.Players = new List<Player> { selectedPlayer };
                AnsiConsole.MarkupLine($"[green]{selectedPlayer.Name}[/] will be moved to [cyan]{newRoom.Name}[/].");
            }

            _context.Rooms.Add(newRoom);
            _context.SaveChanges();

            _logger.LogInformation("Added room {RoomName} at ({X},{Y}) connected {Direction} from {SourceRoom}",
                newRoom.Name, newRoom.X, newRoom.Y, direction, sourceRoom.Name);

            AnsiConsole.MarkupLine($"[green]Success![/] Added [cyan]{newRoom.Name}[/] to the map.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding room");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    /// <summary>
    /// B-LEVEL REQUIREMENT: Display Room Details
    /// Shows comprehensive information about a selected room.
    /// Uses multiple .Include() calls to load related data.
    /// 
    /// Displays:
    /// - Room name and description
    /// - Available exits (North/South/East/West with destination names)
    /// - Players currently in the room
    /// - Monsters currently in the room
    /// 
    /// Uses Spectre.Console Grid and Panel for formatting.
    /// </summary>
    public void DisplayRoomDetails()
    {
        try
        {
            _logger.LogInformation("User selected Display Room Details");
            AnsiConsole.MarkupLine("[yellow]=== Display Room Details ===[/]");

            var rooms = _context.Rooms.OrderBy(r => r.Id).ToList();
            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms found.[/]");
                PressAnyKey();
                return;
            }

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<Room>()
                    .Title("Select a [green]Room[/] to inspect:")
                    .PageSize(10)
                    .AddChoices(rooms)
                    .UseConverter(r => $"{r.Name} ({r.X}, {r.Y})"));

            var roomDetails = _context.Rooms
                .Include(r => r.Players)
                .Include(r => r.Monsters)
                .Include(r => r.NorthRoom)
                .Include(r => r.SouthRoom)
                .Include(r => r.EastRoom)
                .Include(r => r.WestRoom)
                .First(r => r.Id == selectedRoom.Id);

            var grid = new Grid();
            grid.AddColumn();

            grid.AddRow(new Rule($"[yellow]{roomDetails.Name}[/]").LeftJustified());
            grid.AddRow($"[dim]{roomDetails.Description}[/]");
            grid.AddRow("");

            grid.AddRow(new Rule("[cyan]Exits[/]").LeftJustified());
            var exits = new List<string>();
            if (roomDetails.NorthRoom != null) exits.Add($"North -> {roomDetails.NorthRoom.Name}");
            if (roomDetails.SouthRoom != null) exits.Add($"South -> {roomDetails.SouthRoom.Name}");
            if (roomDetails.EastRoom != null) exits.Add($"East -> {roomDetails.EastRoom.Name}");
            if (roomDetails.WestRoom != null) exits.Add($"West -> {roomDetails.WestRoom.Name}");

            if (exits.Any())
                foreach (var exit in exits) grid.AddRow(exit);
            else
                grid.AddRow("[dim]No visible exits.[/]");

            grid.AddRow("");

            grid.AddRow(new Rule("[red]Inhabitants[/]").LeftJustified());

            if (roomDetails.Players.Any())
            {
                grid.AddRow($"[cyan]Players:[/]");
                foreach (var p in roomDetails.Players) grid.AddRow($"  - {p.Name} (HP: {p.Health})");
            }
            else
            {
                grid.AddRow("[dim]No players present.[/]");
            }

            if (roomDetails.Monsters.Any())
            {
                grid.AddRow($"[red]Monsters:[/]");
                foreach (var m in roomDetails.Monsters) grid.AddRow($"  - {m.Name} (HP: {m.Health}, Type: {m.MonsterType})");
            }
            else
            {
                grid.AddRow("[dim]No monsters present.[/]");
            }

            AnsiConsole.Write(new Panel(grid).Border(BoxBorder.Rounded));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying room details");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    #endregion

    #region A-Level Requirements

    /// <summary>
    /// A-LEVEL REQUIREMENT: List Characters in Room by Attribute
    /// Filters characters in a specific room by a chosen attribute threshold.
    /// 
    /// Process:
    /// 1. Select a room from the list
    /// 2. Choose filter attribute (Level, Health, Experience)
    /// 3. Enter threshold value
    /// 4. Query: WHERE RoomId = X AND Attribute >= threshold
    /// 
    /// Uses LINQ AsQueryable() for dynamic filtering.
    /// </summary>
    public void ListCharactersInRoomByAttribute()
    {
        try
        {
            _logger.LogInformation("User selected List Characters in Room by Attribute");
            AnsiConsole.MarkupLine("[yellow]=== List Characters by Attribute ===[/]");

            var rooms = _context.Rooms.OrderBy(r => r.Name).ToList();
            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms found.[/]");
                PressAnyKey();
                return;
            }

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<Room>()
                    .Title("Select a [green]Room[/] to search:")
                    .AddChoices(rooms)
                    .UseConverter(r => r.Name));

            var attribute = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select attribute to filter by:")
                    .AddChoices("Level", "Health", "Experience"));

            var threshold = AnsiConsole.Ask<int>($"Show characters with [cyan]{attribute}[/] greater than or equal to:");

            var query = _context.Players
                .Include(p => p.Room)
                .Where(p => p.RoomId == selectedRoom.Id)
                .AsQueryable();

            switch (attribute)
            {
                case "Level":
                    query = query.Where(p => p.Level >= threshold);
                    break;
                case "Health":
                    query = query.Where(p => p.Health >= threshold);
                    break;
                case "Experience":
                    query = query.Where(p => p.Experience >= threshold);
                    break;
            }

            var results = query.ToList();

            if (results.Any())
            {
                var table = new Table();
                table.AddColumn("Name");
                table.AddColumn(attribute);

                foreach (var p in results)
                {
                    int val;
                    switch (attribute)
                    {
                        case "Level":
                            val = p.Level;
                            break;
                        case "Health":
                            val = p.Health;
                            break;
                        case "Experience":
                        default:
                            val = p.Experience;
                            break;
                    }
                    table.AddRow(p.Name, val.ToString());
                }
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]No characters found in {selectedRoom.Name} with {attribute} > {threshold}.[/]");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing characters by attribute");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    /// <summary>
    /// A-LEVEL REQUIREMENT: List All Rooms with Characters
    /// Displays a world population report showing all inhabited rooms.
    /// 
    /// Query: Rooms with .Include(r => r.Players).Include(r => r.Monsters)
    /// Filter: Only rooms that have at least one player OR monster
    /// 
    /// Uses Spectre.Console Tree for hierarchical display:
    /// - Room names as parent nodes
    /// - Players and Monsters as child nodes with HP
    /// </summary>
    public void ListAllRoomsWithCharacters()
    {
        try
        {
            _logger.LogInformation("User selected List All Rooms with Characters");
            AnsiConsole.MarkupLine("[yellow]=== World Population Report ===[/]");

            var populatedRooms = _context.Rooms
                .Include(r => r.Players)
                .Include(r => r.Monsters)
                .Where(r => r.Players.Any() || r.Monsters.Any())
                .OrderBy(r => r.Name)
                .ToList();

            if (!populatedRooms.Any())
            {
                AnsiConsole.MarkupLine("[yellow]The world is completely empty![/]");
                PressAnyKey();
                return;
            }

            var tree = new Tree("[yellow]World Map[/]");

            foreach (var room in populatedRooms)
            {
                var roomNode = tree.AddNode($"[cyan bold]{room.Name}[/]");

                if (room.Players.Any())
                {
                    var playersNode = roomNode.AddNode("[green]Players[/]");
                    foreach (var player in room.Players)
                    {
                        playersNode.AddNode($"[green]{player.Name}[/] (HP: {player.Health})");
                    }
                }

                if (room.Monsters.Any())
                {
                    var monstersNode = roomNode.AddNode("[red]Monsters[/]");
                    foreach (var monster in room.Monsters)
                    {
                        monstersNode.AddNode($"[red]{monster.Name}[/] (HP: {monster.Health})");
                    }
                }
            }

            AnsiConsole.Write(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing rooms with characters");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    /// <summary>
    /// A-LEVEL REQUIREMENT: Find Equipment Location
    /// Searches for players who have a specific item equipped.
    /// 
    /// Query uses multiple Include/ThenInclude:
    /// - Players -> Equipment -> Weapon
    /// - Players -> Equipment -> Armor
    /// - Players -> Room
    /// 
    /// Searches both weapon and armor names using Contains().
    /// Displays: Character name, Item found, Room location
    /// </summary>
    public void FindEquipmentLocation()
    {
        try
        {
            _logger.LogInformation("User selected Find Equipment Location");
            AnsiConsole.MarkupLine("[yellow]=== Equipment Search ===[/]");

            var itemName = AnsiConsole.Ask<string>("Enter [green]item name[/] to search for (e.g. 'Sword'):");

            var results = _context.Players
                .Include(p => p.Room)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Weapon)
                .Include(p => p.Equipment)
                .ThenInclude(e => e.Armor)
                .Where(p => (p.Equipment.Weapon != null && p.Equipment.Weapon.Name.Contains(itemName)) ||
                            (p.Equipment.Armor != null && p.Equipment.Armor.Name.Contains(itemName)))
                .ToList();

            if (results.Any())
            {
                var table = new Table();
                table.AddColumn("Character");
                table.AddColumn("Item Found");
                table.AddColumn("Location");

                foreach (var p in results)
                {
                    string foundItem = "";
                    if (p.Equipment?.Weapon?.Name.Contains(itemName, StringComparison.OrdinalIgnoreCase) == true)
                        foundItem = p.Equipment.Weapon.Name;
                    else if (p.Equipment?.Armor?.Name.Contains(itemName, StringComparison.OrdinalIgnoreCase) == true)
                        foundItem = p.Equipment.Armor.Name;

                    table.AddRow(
                        $"[green]{p.Name}[/]",
                        $"[yellow]{foundItem}[/]",
                        $"[cyan]{p.Room?.Name ?? "Unknown"}[/]"
                    );
                }
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]No equipment matching '{itemName}' found on any player.[/]");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding equipment");
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
        PressAnyKey();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method for user interaction consistency
    /// </summary>
    private void PressAnyKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    #endregion
}