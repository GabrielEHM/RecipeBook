// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();

var connection = new SqlConnection(config.GetConnectionString("RecipeBook"));

var ingredients = connection.Query("SELECT * FROM Ingredients").ToList();

foreach (var ingredient in ingredients)
{
    Console.WriteLine(JsonConvert.SerializeObject(ingredient));
}