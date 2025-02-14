using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data.Common;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var baseConnectionString = config.GetConnectionString("RecipeBook");

// Modify the connection string to connect to the master database
var masterConnectionString = new SqlConnectionStringBuilder(baseConnectionString)
{
    InitialCatalog = "master"
}.ConnectionString;

using (var masterConnection = new SqlConnection(masterConnectionString))
{
    CreateDatabase(masterConnection);
}

var recipeBookConnectionString = new SqlConnectionStringBuilder(baseConnectionString)
{
    InitialCatalog = "RecipeBook"
}.ConnectionString;

using (var connection = new SqlConnection(recipeBookConnectionString))
{
    connection.Open();
    var ingredients = connection.Query("SELECT * FROM Ingredients").ToList();

    foreach (var ingredient in ingredients)
    {
        Console.WriteLine(JsonConvert.SerializeObject(ingredient));
    }

    connection.Close();
}

void CreateDatabase(DbConnection connection)
{
    try
    {
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "SQL Scripts", "0001-CreateDatabase.sql");
        var sqlScript = File.ReadAllText(scriptPath);

        var commands = sqlScript.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

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
        Console.WriteLine($"An error occurred while creating the database. \n {ex.Message}");
        throw;
    }
}