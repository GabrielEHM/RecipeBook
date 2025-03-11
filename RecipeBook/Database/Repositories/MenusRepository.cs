using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class MenusRepository(DbService dbService) : BulkRemoveEnabledRepository<Menu>(dbService)
    {

    }
}
