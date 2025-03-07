using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class IngredientsRepository
    {
        private readonly DbService _dbService;

        public IngredientsRepository(DbService dbService)
        {
            _dbService = dbService;
        }

        public IAsyncEnumerable<Paged<Ingredient>> GetAll(int startPage = 1, int pageSize = 10)
        {
            return _dbService.GetAll<Ingredient>("Ingredients_GetAll", pageSize, startPage);
        }

        public Paged<Ingredient> GetPage(int page, int pageSize = 10)
        {
            return _dbService.GetPage<Ingredient>("Ingredients_GetAll", page, pageSize);
        }

        public bool Remove(int id)
        {
            return _dbService.ConstrainedRemove("Ingredients_Remove", id);
        }
    }
}
