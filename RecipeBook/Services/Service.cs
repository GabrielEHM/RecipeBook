namespace RecipeBook.Services
{
    abstract class Service<T>
    {
        public abstract bool ShowMenu();
        public abstract bool ListAll(int page = 1, int pageSize = 10);
        public abstract bool GetById(string id);
        public abstract bool Add(string? id = null);
        public abstract bool Delete(string[] ids);
    }
}