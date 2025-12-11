namespace ConsoleRpg.Helpers;

/// <summary>
/// Manages buffered console output with color support.
/// Allows building up multiple lines of colored text before displaying.
/// Used by MenuManager for the admin menu display.
/// 
/// Usage:
/// 1. Call Write/WriteLine to buffer messages with colors
/// 2. Call Display() to render all buffered messages
/// 3. Call Clear() to reset the buffer and clear screen
/// </summary>
public class OutputManager
{
    // Buffer storing messages with their associated colors
    private readonly List<(string message, ConsoleColor color)> _outputBuffer;

    /// <summary>
    /// Initializes a new OutputManager with an empty buffer.
    /// </summary>
    public OutputManager()
    {
        _outputBuffer = new List<(string message, ConsoleColor color)>();
    }

    /// <summary>
    /// Clears the console screen and empties the output buffer.
    /// </summary>
    public void Clear()
    {
        Console.Clear();
        _outputBuffer.Clear();
    }

    /// <summary>
    /// Renders all buffered messages to the console with their colors.
    /// Clears the buffer after displaying.
    /// </summary>
    public void Display()
    {
        foreach (var (message, color) in _outputBuffer)
        {
            WriteColorToConsole(message, color);
        }

        _outputBuffer.Clear();
    }

    /// <summary>
    /// Adds a message to the buffer without a newline.
    /// </summary>
    /// <param name="message">Text to display</param>
    /// <param name="color">Console color (default: White)</param>
    public void Write(string message, ConsoleColor color = ConsoleColor.White)
    {
        _outputBuffer.Add((message, color));
    }

    /// <summary>
    /// Adds a message to the buffer with a newline.
    /// </summary>
    /// <param name="message">Text to display</param>
    /// <param name="color">Console color (default: White)</param>
    public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
    {
        _outputBuffer.Add((message + Environment.NewLine, color));
    }

    /// <summary>
    /// Helper method to write colored text to the console.
    /// Resets color after writing.
    /// </summary>
    private void WriteColorToConsole(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color; // Set the text color
        Console.Write(message); // Write the message to the console
        Console.ResetColor(); // Reset the console color back to default
    }
}
