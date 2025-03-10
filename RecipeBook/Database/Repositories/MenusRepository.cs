using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class MenusRepository
    {
        private readonly DbService _dbService;

        public MenusRepository(DbService dbService)
        {
            _dbService = dbService;
        }

        public IAsyncEnumerable<Paged<Menu>> GetAll(int startPage = 1, int pageSize = 10)
        {
            return _dbService.GetAll<Menu>("Menus_GetAll", pageSize, startPage);
        }

        public Paged<Menu> GetPage(int page, int pageSize = 10)
        {
            return _dbService.GetPage<Menu>("Menus_GetAll", page, pageSize);
        }

        public void Remove(int[] ids)
        {
            if (ids.Length == 1)
            {
                _dbService.Remove("Menus_Remove", ids[0]);
            }
            else
            {
                _dbService.BulkRemove("Menus_BulkRemove", ids);
            }
        }
    }
}
