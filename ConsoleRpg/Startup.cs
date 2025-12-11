using ConsoleRpg.Helpers;
using ConsoleRpg.Services;
using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;

namespace ConsoleRpg;

/// <summary>
/// Application startup configuration.
/// Configures dependency injection, logging, and database connection.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Configures all application services for dependency injection.
    /// Sets up: Logging, Database Context, Game Services, and UI Helpers.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        // Load configuration from appsettings.json
        var configuration = ConfigurationHelper.GetConfiguration();

        // Configure file logging options from config
        var fileLoggerOptions = new NReco.Logging.File.FileLoggerOptions();
        configuration.GetSection("Logging:File").Bind(fileLoggerOptions);

        // ===== Configure Logging =====
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));

            // Add Console logging for development
            loggingBuilder.AddConsole();

            // Add File logging for persistent logs
            var logFileName = "Logs/log.txt";
            loggingBuilder.AddProvider(new FileLoggerProvider(logFileName, fileLoggerOptions));
        });

        // ===== Configure Database Context =====
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<GameContext>(options =>
        {
            ConfigurationHelper.ConfigureDbContextOptions(options, connectionString);
        });

        // ===== Register Game Services =====
        // Transient: New instance per request
        services.AddTransient<GameEngine>(); // Main game loop
        services.AddTransient<MenuManager>(); // Admin menu display
        services.AddTransient<PlayerService>(); // Player actions (combat, movement)
        services.AddTransient<AdminService>(); // CRUD operations

        // Singleton: Single instance shared across application
        services.AddSingleton<OutputManager>(); // Console output buffering
        services.AddSingleton<MapManager>(); // Map visualization
        services.AddSingleton<ExplorationUI>(); // Exploration mode UI
    }
}
