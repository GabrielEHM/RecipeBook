using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class MenusService : Service<Menu>
    {
        private readonly MenusRepository _repository;

        public MenusService(MenusRepository menusRepository)
            : base(menusRepository)
        {
            _repository = (MenusRepository)repository;
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
