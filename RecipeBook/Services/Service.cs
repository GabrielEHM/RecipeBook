using Humanizer;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    abstract class Service<T>
    {
        public virtual bool ShowMenu()
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = ConsoleMenuService.ShowMenu($"{typeof(T).Name.Pluralize()} - What do you want to do?", new CommandList
                        {
                            { $"List all {typeof(T).Name.ToLower().Pluralize()}", (_) => ListAll() },
                            { $"Add a new {typeof(T).Name.ToLower()}", (_) => Add() }
                        });
            }
            return repeat;
        }
        public abstract bool ListAll(int page = 1, int pageSize = 10);
        public abstract bool GetById(string id);
        public abstract bool Add(string? id = null);
        public abstract bool Delete(string[] ids);
    }
}