using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class IngredientsService : Service<Ingredient>
    {
        private readonly IngredientsRepository _ingredientsRepository;
        private readonly ConsoleMenuService _consoleMenuService;

        public IngredientsService(IngredientsRepository ingredientsRepository, ConsoleMenuService consoleMenuService)
        {
            _ingredientsRepository = ingredientsRepository;
            _consoleMenuService = consoleMenuService;
        }

        public override bool ShowMenu()
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = _consoleMenuService.ShowMenu("Ingredients Menu - What do you want to do?", new CommandList
                    {
                        { "List all ingredients", (_) => ListAll() },
                        { "Add a new ingredient", (_) => Add() }
                    });
            }
            return repeat;
        }

        public override bool ListAll(int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = _consoleMenuService.ListEntities(_ingredientsRepository.GetPage(page, pageSize), this);
            }
            return repeat;
        }

        public override bool GetById(string id)
        {
            throw new NotImplementedException();
        }

        public override bool Add(string? id = null)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
