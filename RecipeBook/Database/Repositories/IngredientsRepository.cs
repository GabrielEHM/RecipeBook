using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class IngredientsRepository(DbService dbService) : Repository<Ingredient>(dbService)
    {
        public bool Remove(int id)
        {
            return _dbService.ConstrainedRemove("Ingredients_Remove", id);
        }
    }
}
