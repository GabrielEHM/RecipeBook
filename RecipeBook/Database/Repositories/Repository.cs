using Humanizer;
using RecipeBook.Models;

namespace RecipeBook.Database.Repositories
{
    abstract class Repository<T> where T : Storable, IPageable
    {
        protected readonly DbService _dbService;

        public Repository(DbService dbService)
        {
            _dbService = dbService;
        }
        public virtual IAsyncEnumerable<Paged<T>> GetAll(int startPage = 1, int pageSize = 10)
        {
            return _dbService.GetAll<T>($"{typeof(T).Name.Pluralize()}_GetAll", pageSize, startPage);
        }
        public virtual Paged<T> GetPage(int page, int pageSize = 10)
        {
            return _dbService.GetPage<T>($"{typeof(T).Name.Pluralize()}_GetAll", page, pageSize);
        }
        public virtual T? GetById(int id)
        {
            return _dbService.GetById<T>($"{typeof(T).Name.Pluralize()}_GetById", id);
        }
        public virtual int CreateOrUpdate(T entity)
        {
            return _dbService.CreateOrUpdate($"{typeof(T).Name.Pluralize()}_Add", entity);
        }
    }
}
