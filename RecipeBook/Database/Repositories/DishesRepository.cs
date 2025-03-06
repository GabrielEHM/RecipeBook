using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class DishesRepository
    {
        private readonly DbService _dbService;

        public DishesRepository(DbService dbService)
        {
            _dbService = dbService;
        }

        public IAsyncEnumerable<Paged<Dish>> GetAll(int startPage = 1, int pageSize = 10)
        {
            return _dbService.GetAll<Dish>("Dishes_GetAll", pageSize, startPage);
        }

        public Paged<Dish> GetPage(int page, int pageSize = 10)
        {
            return _dbService.GetPage<Dish>("Dishes_GetAll", page, pageSize);
        }
    }
}
