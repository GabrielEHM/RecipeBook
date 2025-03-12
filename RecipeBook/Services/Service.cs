using Humanizer;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    abstract class Service<T>
    {
        public virtual bool ShowMenu()
        {
            bool repeat = true;
            bool ret = true;
            while (repeat)
            {
                ret = ConsoleMenuService.ShowMenu($"{typeof(T).Name.Pluralize()} - What do you want to do?", new CommandList
                        {
                            { $"List all {typeof(T).Name.ToLower().Pluralize()}", (_) => ListAll(CommandList.GoBack()) },
                            { $"Add a new {typeof(T).Name.ToLower()}", (_) => Add() },
                            { "Back", (_) => { return repeat = false; } }
                        });
            }
            return ret;
        }
        public abstract bool ListAll(CommandAction back, int page = 1, int pageSize = 10);
        public abstract bool GetById(string id, CommandAction back);
        public abstract bool Add(string? id = null);
        public abstract bool Delete(string[] ids);
    }
}