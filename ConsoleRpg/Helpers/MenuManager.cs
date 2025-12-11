namespace ConsoleRpg.Helpers;

/// <summary>
/// Manages the admin/developer menu display and input handling.
/// Displays all available CRUD and query operations organized by grade level.
/// Uses OutputManager for colored console output.
/// </summary>
public class MenuManager
{
    private readonly OutputManager _outputManager;

    /// <summary>
    /// Constructor with OutputManager dependency for colored output.
    /// </summary>
    public MenuManager(OutputManager outputManager)
    {
        _outputManager = outputManager;
    }

    /// <summary>
    /// Displays the main admin menu and handles user selection.
    /// Menu is organized by feature level: Basic, C-Level, B-Level, A-Level.
    /// </summary>
    /// <param name="handleChoice">Callback action to process the user's selection</param>
    public void ShowMainMenu(Action<string> handleChoice)
    {
        _outputManager.Clear();

        // Header
        _outputManager.WriteLine("=================================", ConsoleColor.Yellow);
        _outputManager.WriteLine("      ADMIN / DEVELOPER MENU", ConsoleColor.Yellow);
        _outputManager.WriteLine("=================================", ConsoleColor.Yellow);
        _outputManager.WriteLine("");

        // Navigation options
        _outputManager.WriteLine("E. Return to Exploration Mode", ConsoleColor.Yellow);
        _outputManager.WriteLine("S. Switch Active Character", ConsoleColor.Yellow);
        _outputManager.WriteLine("Q. Quit Game", ConsoleColor.Red);
        _outputManager.WriteLine("");

        // Basic CRUD features
        _outputManager.WriteLine("BASIC FEATURES:", ConsoleColor.Green);
        _outputManager.WriteLine("1. Add New Character", ConsoleColor.Cyan);
        _outputManager.WriteLine("2. Edit Character", ConsoleColor.Cyan);
        _outputManager.WriteLine("3. Display All Characters", ConsoleColor.Cyan);
        _outputManager.WriteLine("4. Search Character by Name", ConsoleColor.Cyan);
        _outputManager.WriteLine("");

        // C-Level features (Ability management)
        _outputManager.WriteLine("'C' LEVEL FEATURES:", ConsoleColor.Green);
        _outputManager.WriteLine("5. Add Ability to Character", ConsoleColor.Cyan);
        _outputManager.WriteLine("6. Display Character Abilities", ConsoleColor.Cyan);
        _outputManager.WriteLine("7. Attack with Ability (use Exploration Mode)", ConsoleColor.DarkGray);
        _outputManager.WriteLine("");

        // B-Level features (Room management)
        _outputManager.WriteLine("'B' LEVEL FEATURES:", ConsoleColor.Green);
        _outputManager.WriteLine("8. Add New Room", ConsoleColor.Cyan);
        _outputManager.WriteLine("9. Display Room Details", ConsoleColor.Cyan);
        _outputManager.WriteLine("10. Navigate Rooms (use Exploration Mode)", ConsoleColor.DarkGray);
        _outputManager.WriteLine("");

        // A-Level features (Advanced queries)
        _outputManager.WriteLine("'A' LEVEL FEATURES:", ConsoleColor.Green);
        _outputManager.WriteLine("11. List Characters in Room by Attribute", ConsoleColor.Cyan);
        _outputManager.WriteLine("12. List All Rooms with Characters/Monsters", ConsoleColor.Cyan);
        _outputManager.WriteLine("13. Find Equipment Location", ConsoleColor.Cyan);
        _outputManager.WriteLine("");

        _outputManager.WriteLine("Select an option:", ConsoleColor.White);
        _outputManager.Display();

        // Get user input and pass to handler
        var input = Console.ReadLine();
        handleChoice(input);
    }
}
