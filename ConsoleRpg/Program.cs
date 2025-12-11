using ConsoleRpg.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleRpg;

/// <summary>
/// Application entry point.
/// Configures dependency injection and starts the game engine.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point - sets up DI container and runs the game.
    /// </summary>
    /// <param name="args">Command line arguments (not used)</param>
    private static void Main(string[] args)
    {
        // Set up dependency injection container
        var serviceCollection = new ServiceCollection();
        Startup.ConfigureServices(serviceCollection);

        // Build the service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Resolve GameEngine and start the game
        var gameEngine = serviceProvider.GetService<GameEngine>();
        gameEngine?.Run();
    }
}
