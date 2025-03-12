using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    abstract class Service<T> where T : Storable, IPageable
    {
        protected readonly Repository<T> repository;
        protected Service(Repository<T> repository)
        {
            this.repository = repository;
        }
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
        public virtual bool ListAll(CommandAction back, int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            var backCommand = new Command("Back", (_) => { repeat = false; return back(); }, "back");
            bool ret = true;
            while (repeat && ret)
            {
                ret = ConsoleMenuService.ListEntities(repository.GetPage(page, pageSize), this, backCommand);
            }
            return ret;
        }
        public virtual bool GetById(string id, CommandAction back)
        {
            try
            {
                int parsedId = int.Parse(id);
                var entity = repository.GetById(parsedId);
                if (entity is null)
                {
                    return CommandList.InvalidChoice($"The {typeof(T).Name.ToLower()} with id {id} does not exist.");
                }
                bool repeat = true;
                var options = new CommandList()
                {
                    { $"Update", (_) => Add(id) },
                    { $"Delete", (_) => Delete([id]) },
                    { "Back", (_) => { repeat = false; return back(); } }
                };
                bool ret = true;
                while (repeat)
                {
                    ret = ConsoleMenuService.DetailEntity(entity, options);
                }
                return ret;
            }
            catch (FormatException ex)
            {
                return CommandList.InvalidChoice("One or more of the provided ids are not valid integers.", ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleMenuService.ShowError($"An error occurred while processing the request. {ex.Message}");
                return true;
            }
        }
        public abstract bool Add(string? id = null);
        public abstract bool Delete(string[] ids);
    }
}