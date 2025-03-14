using Humanizer;
using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    class BulkRemoveEnabledRepository<T>(DbService dbService) : Repository<T>(dbService) where T : Storable, IPageable
    {
        public virtual void Remove(int[] ids)
        {
            if (ids.Length == 1)
            {
                _dbService.Remove($"{typeof(T).Name.Pluralize()}_Remove", ids[0]);
            }
            else
            {
                _dbService.BulkRemove($"{typeof(T).Name.Pluralize()}_BulkRemove", ids);
            }
        }
    }
}
