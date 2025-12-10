using ConsoleRpg.Helpers;
using ConsoleRpg.Models;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace ConsoleRpg.Services;

public class GameEngine
{
    private readonly GameContext _context;
    private readonly MenuManager _menuManager;
    private readonly MapManager _mapManager;
    private readonly ExplorationUI _explorationUI;
    private readonly PlayerService _playerService;
    private readonly AdminService _adminService;
    private readonly ILogger<GameEngine> _logger;

    private Player _currentPlayer;
    private Room _currentRoom;
    private GameMode _currentMode = GameMode.Exploration;

    public GameEngine(GameContext context, MenuManager menuManager, MapManager mapManager,
                     ExplorationUI explorationUI, PlayerService playerService,
                     AdminService adminService, ILogger<GameEngine> logger)
    {
        _context = context;
        _menuManager = menuManager;
        _mapManager = mapManager;
        _explorationUI = explorationUI;
        _playerService = playerService;
        _adminService = adminService;
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation("Game engine started");

        // Initialize game - get or create first player
        InitializeGame();

        // Main game loop
        while (true)
        {
            if (_currentMode == GameMode.Exploration)
            {
                ExplorationMode();
            }
            else
            {
                AdminMode();
            }
        }
    }

    /// <summary>
    /// Initialize the game by getting the first player or creating one
    /// </summary>
    private void InitializeGame()
    {
        // Try to get the first player
        _currentPlayer = _context.Players
            .Include(p => p.Room)
            .Include(p => p.Equipment)
            .Include(p => p.Abilities)
            .FirstOrDefault();

        if (_currentPlayer == null)
        {
            AnsiConsole.MarkupLine("[yellow]No players found! Please create a character first.[/]");
            _currentMode = GameMode.Admin;
            return;
        }

        // Get current room or default to first room
        _currentRoom = _currentPlayer.Room ?? _context.Rooms.Include(r => r.Players).Include(r => r.Monsters).FirstOrDefault();

        if (_currentRoom == null)
        {
            AnsiConsole.MarkupLine("[red]No rooms found! Database may not be properly seeded.[/]");
            _currentMode = GameMode.Admin;
            return;
        }

        _logger.LogInformation("Game initialized with player {PlayerName} in room {RoomName}",
            _currentPlayer.Name, _currentRoom.Name);
    }

    #region Exploration Mode

    /// <summary>
    /// Main exploration mode - this is where the player navigates the world
    /// </summary>
    private void ExplorationMode()
    {
        // Reload room with all related data
        _currentRoom = _context.Rooms
            .Include(r => r.Players)
            .Include(r => r.Monsters)
            .Include(r => r.NorthRoom)
            .Include(r => r.SouthRoom)
            .Include(r => r.EastRoom)
            .Include(r => r.WestRoom)
            .FirstOrDefault(r => r.Id == _currentRoom.Id);

        // Reload player to get fresh health data
        _currentPlayer = _context.Players
            .Include(p => p.Equipment)
            .Include(p => p.Abilities)
            .FirstOrDefault(p => p.Id == _currentPlayer.Id);

        // Get all rooms for map
        var allRooms = _context.Rooms.ToList();
        bool hasMonsters = _currentRoom.Monsters != null && _currentRoom.Monsters.Any();

        // Render UI and get player's action choice
        var selectedAction = _explorationUI.RenderAndGetAction(allRooms, _currentRoom, _currentPlayer);

        // Handle the selected action
        HandleExplorationAction(selectedAction, hasMonsters);
    }

    /// <summary>
    /// Handles player action selection during exploration mode
    /// Processes service results and displays them through ExplorationUI
    /// </summary>
    private void HandleExplorationAction(string action, bool hasMonsters)
    {
        // Track the specific monster that was targeted (if any)
        Monster targetedMonster = null;

        switch (action)
        {
            case "Go North":
                var moveNorthResult = _playerService.MoveToRoom(_currentPlayer, _currentRoom, _currentRoom.NorthRoomId, "North");
                HandleMoveResult(moveNorthResult);
                break;
            case "Go South":
                var moveSouthResult = _playerService.MoveToRoom(_currentPlayer, _currentRoom, _currentRoom.SouthRoomId, "South");
                HandleMoveResult(moveSouthResult);
                break;
            case "Go East":
                var moveEastResult = _playerService.MoveToRoom(_currentPlayer, _currentRoom, _currentRoom.EastRoomId, "East");
                HandleMoveResult(moveEastResult);
                break;
            case "Go West":
                var moveWestResult = _playerService.MoveToRoom(_currentPlayer, _currentRoom, _currentRoom.WestRoomId, "West");
                HandleMoveResult(moveWestResult);
                break;
            case "View Map":
                AnsiConsole.Clear();
                _mapManager.DisplayMap(_context.Rooms.ToList(), _currentRoom);
                _mapManager.DisplayRoomDetails(_currentRoom);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[dim]Press any key to return to game...[/]");
                Console.ReadKey(true);
                break;
            case "View Inventory":
                HandleActionResult(_playerService.ShowInventory(_currentPlayer));
                break;
            case "View Character Stats":
                HandleActionResult(_playerService.ShowCharacterStats(_currentPlayer));
                break;
            case "Attack Monster":
                _explorationUI.AddSeparator();
                _explorationUI.AddOutput("[yellow bold]=== COMBAT ROUND ===[/]");
                _explorationUI.AddOutput("");

                // Capture the returned monster from the service
                var attackResult = _playerService.AttackMonster();
                HandleActionResult(attackResult);

                if (attackResult.Success)
                    targetedMonster = attackResult.Value;
                break;
            case "Use Ability":
                _explorationUI.AddSeparator();
                _explorationUI.AddOutput("[cyan bold]=== ABILITY ACTIVATED ===[/]");
                _explorationUI.AddOutput("");

                // Capture the returned monster from the service
                var abilityResult = _playerService.UseAbilityOnMonster();
                HandleActionResult(abilityResult);

                if (abilityResult.Success)
                    targetedMonster = abilityResult.Value;
                break;
            case "Equip Item":
                HandleActionResult(_playerService.EquipItem());
                break;
            case "Return to Main Menu":
                _currentMode = GameMode.Admin;
                _explorationUI.AddMessage("[yellow]-> Admin Mode[/]");
                _explorationUI.AddOutput("[yellow]Switching to Admin Mode for database management and testing.[/]");
                break;
            default:
                _explorationUI.AddMessage($"[red]Unknown action[/]");
                _explorationUI.AddOutput($"[red]Unknown action: {action}[/]");
                break;
        }

        // Only trigger monster counter-attack if a monster was actually targeted
        if (targetedMonster != null)
        {
            // Reload current player to get updated health after player's attack
            _currentPlayer = _context.Players
                .Include(p => p.Equipment)
                .FirstOrDefault(p => p.Id == _currentPlayer.Id);

            // Show player's health status after their action
            _explorationUI.ShowCombatSummary(_currentPlayer.Name, _currentPlayer.Health, _currentPlayer.MaxHealth);
            _explorationUI.AddSeparator();

            // Reload current room to get fresh monster data (and check if our target died)
            _currentRoom = _context.Rooms
                .Include(r => r.Monsters)
                .FirstOrDefault(r => r.Id == _currentRoom.Id);

            // Check if the SPECIFIC monster we targeted is still alive
            var survivingMonster = _currentRoom?.Monsters?.FirstOrDefault(m => m.Id == targetedMonster.Id);

            if (survivingMonster != null)
            {
                _explorationUI.AddOutput("");
                _explorationUI.AddOutput("[red bold]=== MONSTER COUNTER-ATTACK ===[/]");
                _explorationUI.AddOutput("");

                // Only the surviving targeted monster attacks back
                string attackMessage = survivingMonster.Attack(_currentPlayer);
                _explorationUI.AddOutput($"[red]{attackMessage}[/]");

                _context.SaveChanges();

                // Show player's health status after monster attacks
                _explorationUI.ShowCombatSummary(_currentPlayer.Name, _currentPlayer.Health, _currentPlayer.MaxHealth);

                // Check if player died
                if (_currentPlayer.Health <= 0)
                {
                    AnsiConsole.WriteLine();
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red bold]╔════════════════════════════════╗[/]");
                    AnsiConsole.MarkupLine("[red bold]║      YOU HAVE DIED!            ║[/]");
                    AnsiConsole.MarkupLine("[red bold]╚════════════════════════════════╝[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[dim]Your adventure has come to an end...[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.Markup("[dim]Press any key to exit...[/]");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            }
        }
    }

    /// <summary>
    /// Handles the result of a move operation
    /// </summary>
    private void HandleMoveResult(ServiceResult<Room> result)
    {
        _explorationUI.AddMessage(result.Message);
        _explorationUI.AddOutput(result.DetailedOutput);

        if (result.Success && result.Value != null)
        {
            _currentRoom = result.Value;
        }
    }

    /// <summary>
    /// Handles the result of a general player action
    /// </summary>
    private void HandleActionResult(ServiceResult result)
    {
        _explorationUI.AddMessage(result.Message);
        _explorationUI.AddOutput(result.DetailedOutput);
    }

    #endregion

    #region Admin Mode

    /// <summary>
    /// Admin mode - provides access to CRUD operations and template methods
    /// </summary>
    private void AdminMode()
    {
        _menuManager.ShowMainMenu(AdminMenuChoice);
    }

    private void AdminMenuChoice(string choice)
    {
        switch (choice?.ToUpper())
        {
            // World Exploration / Return to Exploration Mode
            case "E":
            case "0":
                ExploreWorld();
                break;

            case "S":
                var newPlayer = _playerService.SwitchCharacter();
                if (newPlayer != null)
                {
                    _currentPlayer = newPlayer;
                    // Update the current room to match the new player's location
                    // If player has no room, fallback to the first room in DB (Safety check)
                    _currentRoom = _currentPlayer.Room ?? _context.Rooms.FirstOrDefault();

                    AnsiConsole.MarkupLine($"[green]Switched control to {_currentPlayer.Name}![/]");
                }
                PressAnyKey();
                break;

            // Basic Features
            case "1":
                _adminService.AddCharacter();
                break;
            case "2":
                _adminService.EditCharacter();
                break;
            case "3":
                _adminService.DisplayAllCharacters();
                PressAnyKey();
                break;
            case "4":
                _adminService.SearchCharacterByName();
                PressAnyKey();
                break;

            // C-Level Features
            case "5":
                _adminService.AddAbilityToCharacter();
                break;
            case "6":
                _adminService.DisplayCharacterAbilities();
                break;
            case "7":
                // Attack with ability - redirect to exploration mode
                AnsiConsole.MarkupLine("[yellow]Please use this feature in Exploration Mode[/]");
                PressAnyKey();
                break;

            // B-Level Features
            case "8":
                _adminService.AddRoom();
                break;
            case "9":
                _adminService.DisplayRoomDetails();
                break;
            case "10":
                // Navigate rooms - redirect to exploration mode
                AnsiConsole.MarkupLine("[yellow]Please use this feature in Exploration Mode[/]");
                PressAnyKey();
                break;

            // A-Level Features
            case "11":
                _adminService.ListCharactersInRoomByAttribute();
                break;
            case "12":
                _adminService.ListAllRoomsWithCharacters();
                break;
            case "13":
                _adminService.FindEquipmentLocation();
                break;

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection.[/]");
                PressAnyKey();
                break;
        }
    }

    #endregion

    #region Mode Switching

    /// <summary>
    /// Switch from Admin Mode to Exploration Mode
    /// </summary>
    private void ExploreWorld()
    {
        _logger.LogInformation("User selected Explore World - switching to Exploration Mode");

        // Simply switch to exploration mode
        _currentMode = GameMode.Exploration;
        _explorationUI.AddMessage("[green]Entered world[/]");
        _explorationUI.AddOutput("[green]Welcome to the world! Use the menu below to explore, fight monsters, and manage your character.[/]");
    }

    #endregion

    #region Helper Methods

    private void PressAnyKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    #endregion
}

/// <summary>
/// Represents the current game mode
/// </summary>
public enum GameMode
{
    Exploration,
    Admin
}