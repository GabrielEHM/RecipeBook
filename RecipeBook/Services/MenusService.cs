using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class MenusService : Service<Menu>
    {
        private readonly MenusRepository _repository;

        public MenusService(MenusRepository menusRepository)
            : base()
        {
            _repository = menusRepository;
        }

        public override bool ListAll(int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = ConsoleMenuService.ListEntities(_repository.GetPage(page, pageSize), this);
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
            try
            {
                int[] convertedIds = Array.ConvertAll(ids, int.Parse);
                if (!ConsoleMenuService.Confirm($"You are about to attempt to delete the following Menus: {convertedIds.Humanize()}"))
                {
                    return true;
                }
                _repository.Remove(convertedIds);
                ConsoleMenuService.ShowMessage($"The Menus {convertedIds.Humanize()} were deleted successfully.");
                return true;
            }
            catch (FormatException ex)
            {
                return CommandList.InvalidChoice("One or more if the provided ids are not valid integers.", ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleMenuService.ShowError($"An error occurred while deleting the Menus. {ex.Message}");
                return true;
            }
        }
    }
}
