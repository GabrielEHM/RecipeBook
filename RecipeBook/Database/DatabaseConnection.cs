using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RecipeBook.Database
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _config;
        private readonly string _baseConnectionString;

        public DatabaseConnection(IConfiguration config)
        {
            _config = config;
            _baseConnectionString = _config.GetConnectionString("RecipeBook")!;
        }

        private SqlConnection GetConnectionToMaster()
        {
            var masterConnectionString = new SqlConnectionStringBuilder(_baseConnectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            return new SqlConnection(masterConnectionString);
        }

        public SqlConnection GetConnection()
        {
            var recipeBookConnectionString = new SqlConnectionStringBuilder(_baseConnectionString)
            {
                InitialCatalog = "RecipeBook"
            }.ConnectionString;

            return new SqlConnection(recipeBookConnectionString);
        }

        public void MigrateDatabase()
        {
            var connection = GetConnectionToMaster();
            Console.WriteLine("Migrating database...");
            try
            {
                string[] fileEntries = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Database", "SQL Scripts"));

                foreach (var scriptPath in fileEntries)
                {
                    RunSqlFile(connection, scriptPath);
                }
                Console.WriteLine("Database migration completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the database. \n {ex.Message}");
                throw;
            }
        }

        public void RunSqlFile(SqlConnection connection, string scriptPath)
        {
            try
            {
                var sqlScript = File.ReadAllText(scriptPath);
                var commands = sqlScript.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);
                connection.Open();
                foreach (var commandText in commands)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText.Trim();
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while running the SQL script. \n {ex.Message}");
                throw;
            }
        }
    }

}
