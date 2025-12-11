using System.Reflection;

namespace ConsoleRpgEntities.Helpers
{
    /// <summary>
    /// Helper class for loading SQL migration scripts from embedded files.
    /// Allows migrations to use raw SQL scripts for complex seed data.
    /// Scripts are stored in the Migrations/Scripts directory.
    /// </summary>
    public static class MigrationHelper
    {
        /// <summary>
        /// Loads a SQL script file for a migration.
        /// Scripts should be named: {MigrationClassName}.sql for Up, {MigrationClassName}.rollback.sql for Down.
        /// </summary>
        /// <param name="migrationClassName">Name of the migration class (e.g., "InitialSeedData")</param>
        /// <param name="scriptType">"Up" for forward migration, "Down" for rollback</param>
        /// <returns>The SQL script contents as a string</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script file doesn't exist</exception>
        public static string GetMigrationScript(string migrationClassName, string scriptType)
        {
            // Build the script file name based on migration direction
            string scriptFileName = scriptType == "Up"
                ? $"{migrationClassName}.sql"
                : $"{migrationClassName}.rollback.sql";

            // Get the directory of the executing assembly
            string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Build the path to the SQL script
            string scriptPath = Path.Combine(assemblyLocation, "Migrations", "Scripts", scriptFileName);

            // Check if the file exists
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"SQL script file not found: {scriptPath}");
            }

            // Read and return the SQL script content
            return File.ReadAllText(scriptPath);
        }
    }
}
