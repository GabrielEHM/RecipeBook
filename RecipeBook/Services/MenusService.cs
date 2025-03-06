using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class MenusService : Service<Menu>
    {
        private readonly MenusRepository _repository;

        public MenusService(MenusRepository menusRepository, ConsoleMenuService consoleMenuService)
            : base(consoleMenuService)
        {
            _repository = menusRepository;
        }

        public override bool ListAll(int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = _consoleMenuService.ListEntities(_repository.GetPage(page, pageSize), this);
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
