using ConsoleRpgEntities.Models.Rooms;
using ConsoleRpgEntities.Models.Characters;
using Spectre.Console;

namespace ConsoleRpg.Helpers;

/// <summary>
/// Manages the exploration mode UI layout and rendering.
/// Handles the main gameplay screen with map, room details, and action menus.
/// Maintains message and output logs for displaying combat results and events.
/// 
/// Responsibilities:
/// - Render the exploration screen (map, room info, recent events)
/// - Display available actions and get player choice
/// - Manage message/output logs with automatic truncation
/// - Show combat summaries with health bars
/// </summary>
public class ExplorationUI
{
    private readonly MapManager _mapManager;

    // Rolling logs for displaying recent activity
    private readonly List<string> _messageLog = new List<string>(); // Brief status messages
    private readonly List<string> _outputLog = new List<string>(); // Detailed combat/event output

    // Configuration for log display limits
    private const int MaxMessages = 8; // Max brief messages to show
    private const int MaxOutputLines = 20; // Max detailed output lines

    /// <summary>
    /// Constructor with MapManager dependency for rendering map panels.
    /// </summary>
    /// <param name="mapManager">Manager for map visualization</param>
    public ExplorationUI(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    /// <summary>
    /// Main render method - displays the full exploration UI and returns selected action.
    /// Clears screen, renders all panels, shows action menu, and gets player choice.
    /// </summary>
    /// <param name="allRooms">All rooms for map rendering</param>
    /// <param name="currentRoom">The player's current room</param>
    /// <param name="activePlayer">The active player character</param>
    /// <returns>The selected action string (e.g., "Go North", "Attack Monster")</returns>
    public string RenderAndGetAction(IEnumerable<Room> allRooms, Room currentRoom, Player activePlayer)
    {
        AnsiConsole.Clear();

        // Display player info header
        AnsiConsole.Write(new Rule($"[blue]Playing as: [bold]{activePlayer.Name}[/] (Lvl {activePlayer.Level})[/]").LeftJustified());
        AnsiConsole.WriteLine();

        // Render map panel showing room grid
        var mapPanel = _mapManager.GetCompactMapPanel(allRooms, currentRoom);
        AnsiConsole.Write(mapPanel);

        // Render current room details (description, inhabitants)
        var roomPanel = _mapManager.GetCompactRoomDetailsPanel(currentRoom);
        AnsiConsole.Write(roomPanel);

        AnsiConsole.WriteLine();

        // Render recent events log if there are any
        if (_outputLog.Any())
        {
            var outputPanel = new Panel(string.Join("\n", _outputLog))
            {
                Header = new PanelHeader("[yellow]Recent Events[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0, 1, 0)
            };
            AnsiConsole.Write(outputPanel);
            AnsiConsole.WriteLine();

            // Trim old entries if log exceeds maximum
            if (_outputLog.Count > MaxOutputLines)
            {
                int linesToRemove = _outputLog.Count - MaxOutputLines;
                _outputLog.RemoveRange(0, linesToRemove);
            }
        }

        // Get context-sensitive actions based on current room
        var actions = _mapManager.GetAvailableActions(currentRoom);

        // Display action menu with numbered options
        AnsiConsole.MarkupLine("[cyan bold]═══════════════════════════════════════[/]");
        AnsiConsole.MarkupLine("[white bold]What will you do?[/]");
        AnsiConsole.MarkupLine("[cyan bold]═══════════════════════════════════════[/]");

        for (int i = 0; i < actions.Count; i++)
        {
            AnsiConsole.MarkupLine($"  [cyan]{i + 1}[/]. {actions[i]}");
        }

        // Get player's choice
        AnsiConsole.WriteLine();
        int choice = AnsiConsole.Ask<int>("[white]Enter your choice:[/]", 1);

        // Validate and clamp choice to valid range
        if (choice < 1 || choice > actions.Count)
            choice = 1;

        return actions[choice - 1];
    }

    /// <summary>
    /// Adds a timestamped message to the brief message log.
    /// Automatically removes oldest messages when limit exceeded.
    /// </summary>
    /// <param name="message">The message to add (supports Spectre markup)</param>
    public void AddMessage(string message)
    {
        _messageLog.Add($"[dim]{DateTime.Now:HH:mm:ss}[/] {message}");

        // Keep only the last N messages
        if (_messageLog.Count > MaxMessages)
        {
            _messageLog.RemoveAt(0);
        }
    }

    /// <summary>
    /// Adds detailed output text to the main output area.
    /// Handles multi-line strings by splitting them.
    /// </summary>
    /// <param name="output">The output text (can contain newlines)</param>
    public void AddOutput(string output)
    {
        if (string.IsNullOrEmpty(output)) return;

        // Split multi-line output into individual lines
        var lines = output.TrimEnd().Split(new[] { '\n', '\r' }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            _outputLog.Add(line);
        }

        // Trim old entries to stay within limit
        while (_outputLog.Count > MaxOutputLines)
        {
            _outputLog.RemoveAt(0);
        }
    }

    /// <summary>
    /// Adds a visual separator line to the output log.
    /// Useful for separating different events or combat rounds.
    /// </summary>
    public void AddSeparator()
    {
        _outputLog.Add("[dim]----------------------------------------[/]");
    }

    /// <summary>
    /// Displays a formatted combat summary with visual health bar.
    /// Color-coded based on health percentage: green > 70%, yellow > 30%, red otherwise.
    /// </summary>
    /// <param name="playerName">Name to display</param>
    /// <param name="playerHealth">Current health points</param>
    /// <param name="playerMaxHealth">Maximum health points</param>
    public void ShowCombatSummary(string playerName, int playerHealth, int playerMaxHealth)
    {
        // Calculate health percentage for color coding
        var healthPercent = (double)playerHealth / playerMaxHealth * 100;
        var healthColor = healthPercent > 70 ? "green" : healthPercent > 30 ? "yellow" : "red";

        // Generate ASCII health bar
        var healthBar = GenerateHealthBar(playerHealth, playerMaxHealth);

        // Add formatted status to output
        AddOutput("");
        AddOutput($"[{healthColor} bold]{playerName}'s Status:[/] {Markup.Escape(healthBar)} [white]{playerHealth}/{playerMaxHealth} HP[/]");
    }

    /// <summary>
    /// Generates an ASCII health bar using = for filled and - for empty.
    /// Example: [=========-----------] for 45% health
    /// </summary>
    /// <param name="current">Current health value</param>
    /// <param name="max">Maximum health value</param>
    /// <returns>Formatted health bar string</returns>  
    private string GenerateHealthBar(int current, int max)
    {
        const int barLength = 20;

        // Calculate filled portion, clamped to valid range
        int filled = (int)((double)current / max * barLength);
        filled = Math.Max(0, Math.Min(barLength, filled));

        // Build the bar: [====----]
        var bar = "[" + new string('=', filled) + new string('-', barLength - filled) + "]";
        return bar;
    }
}