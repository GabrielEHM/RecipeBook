using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class DishesRepository(DbService dbService) : BulkRemoveEnabledRepository<Dish>(dbService)
    {
        public Dish? GetById(int id, int? servings)
        {
            return _dbService.GetById("Dishes_GetById", id, servings);
        }
    }
}
