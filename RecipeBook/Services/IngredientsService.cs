using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class IngredientsService
    {
        private readonly IngredientsRepository _ingredientsRepository;
        private readonly ConsoleMenuService _consoleMenuService;

        public IngredientsService(IngredientsRepository ingredientsRepository, ConsoleMenuService consoleMenuService)
        {
            _ingredientsRepository = ingredientsRepository;
            _consoleMenuService = consoleMenuService;
        }

        public bool ShowMenu()
        {
            _consoleMenuService.ShowMenu("Ingredients Menu - What do you want to do?", new CommandList
                {
                    { "List all ingredients", (_) => ListAll().Result },
                    { "Add a new ingredient", (_) => Create() }
                });
            return false;
        }

        public async Task<bool> ListAll(int page = 1, int pageSize = 10)
        {
            var listCommands = new CommandList
                {
                    { "Last Page", (args) => { return ListAll(page: int.Parse(args[0]), pageSize: int.Parse(args[1])).Result; }, "last" },
                    { "Next Page", (args) => { return false; }, "next" },
                    { "Go to Page", (args) => { return ListAll(page: int.Parse(args[0]), pageSize: int.Parse(args[1])).Result; }, "goto" },
                    { "Add a new ingredient", (_) => Create() , "add"},
                    { "Details", (args) => GetById(id: int.Parse(args[0])), "detail" },
                    { "Delete", (args) => Delete(ids: args), "delete" }
                };
            await _consoleMenuService.ListEntitiesAsync(_ingredientsRepository.GetAll(page, pageSize), listCommands);
            return true;
        }

        private bool GetById(int id)
        {
            throw new NotImplementedException();
        }

        private bool Create()
        {
            throw new NotImplementedException();
        }

        private bool Delete(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
