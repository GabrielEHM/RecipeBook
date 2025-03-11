using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class DishesRepository(DbService dbService) : BulkRemoveEnabledRepository<Dish>(dbService)
    {

    }
}
