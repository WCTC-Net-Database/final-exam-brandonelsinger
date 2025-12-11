using ConsoleRpgEntities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConsoleRpgEntities.Data
{
    /// <summary>
    /// Design-time factory for creating GameContext instances.
    /// Used by EF Core CLI tools (dotnet ef) for migrations and database updates.
    /// Implements IDesignTimeDbContextFactory to provide context without running the full application.
    /// </summary>
    public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        /// <summary>
        /// Creates a new GameContext instance for design-time operations.
        /// Loads configuration from appsettings.json and configures SQL Server connection.
        /// </summary>
        /// <param name="args">Command line arguments (not used)</param>
        /// <returns>Configured GameContext instance</returns>
        public GameContext CreateDbContext(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = ConfigurationHelper.GetConfiguration();

            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Build DbContext options with SQL Server
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            ConfigurationHelper.ConfigureDbContextOptions(optionsBuilder, connectionString);

            // Create and return the context
            return new GameContext(optionsBuilder.Options);
        }
    }
}
