using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBook.Database;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;
using RecipeBook.Services;

var services = new ServiceCollection();

var provider = ConfigureServices(services);
var dbConnection = provider.GetRequiredService<DatabaseConnection>();
dbConnection.MigrateDatabase();

var consoleMenuService = provider.GetRequiredService<ConsoleMenuService>();

bool running = true;
while (running)
{
    consoleMenuService.ShowMenu("Main Menu - What do you want to see?", new CommandList
            {
                { "Ingredients", (_) => provider.GetRequiredService<IngredientsService>().ShowMenu() },
                //{ "Dishes", (_) => provider.GetRequiredService<DishesService>().ShowMenu() },
                //{ "Menus", (_) => provider.GetRequiredService<MenusService>().ShowMenu() },
                { "Exit", (_) => running = false }
            });
}

Console.WriteLine("\nExiting the application. Goodbye!");
Console.ReadLine();
static ServiceProvider ConfigureServices(IServiceCollection services)
{
    var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
    services.AddSingleton<IConfiguration>(config);
    services.AddSingleton<DatabaseConnection>();
    services.AddSingleton<ConsoleMenuService>();
    services.AddSingleton<DbService>();
    services.AddTransient<IngredientsRepository>();
    services.AddTransient<IngredientsService>();

    return services.BuildServiceProvider();
}