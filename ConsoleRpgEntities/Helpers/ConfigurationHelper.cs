using ConsoleRpgEntities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleRpgEntities.Helpers
{
    /// <summary>
    /// Static helper class for application configuration management.
    /// Loads settings from appsettings.json and configures database connections.
    /// Used by both the main application and EF Core design-time tools.
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Builds and returns the application configuration from appsettings.json.
        /// Supports environment-specific overrides (e.g., appsettings.Development.json).
        /// </summary>
        /// <param name="basePath">Base directory path (defaults to current directory)</param>
        /// <param name="environmentName">Optional environment name for environment-specific config</param>
        /// <returns>IConfigurationRoot containing all configuration settings</returns>
        public static IConfigurationRoot GetConfiguration(string basePath = null, string environmentName = null)
        {
            basePath ??= Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Optionally add environment-specific configuration
            if (!string.IsNullOrEmpty(environmentName))
            {
                builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            // Allow environment variables to override settings
            builder.AddEnvironmentVariables();

            return builder.Build();
        }

        /// <summary>
        /// Configures DbContext options with SQL Server and lazy loading proxies.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to configure</param>
        /// <param name="connectionString">SQL Server connection string</param>
        public static void ConfigureDbContextOptions(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseSqlServer(connectionString)
                .UseLazyLoadingProxies();
        }
    }
}
