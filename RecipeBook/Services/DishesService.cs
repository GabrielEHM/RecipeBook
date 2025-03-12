using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class DishesService : Service<Dish>
    {
        private readonly DishesRepository _repository;

        public DishesService(DishesRepository dishesRepository)
            : base(dishesRepository)
        {
            _repository = (DishesRepository)repository;
        }

        public bool GetById(string id, string servings, CommandAction back)
        {
            try
            {
                int parsedId = int.Parse(id);
                int parsedServings = int.Parse(servings);
                var entity = _repository.GetById(parsedId, parsedServings);
                if (entity is null)
                {
                    return CommandList.InvalidChoice($"The dish with id {id} does not exist.");
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

        public override bool Add(string? id = null)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string[] ids)
        {
            try
            {
                int[] convertedIds = Array.ConvertAll(ids, int.Parse);
                if (!ConsoleMenuService.Confirm($"You are about to attempt to delete the following Dishes: {convertedIds.Humanize()}"))
                {
                    return true;
                }
                _repository.Remove(convertedIds);
                ConsoleMenuService.ShowMessage($"The Dishes {convertedIds.Humanize()} were deleted successfully.");
                return true;
            }
            catch (FormatException ex)
            {
                return CommandList.InvalidChoice("One or more if the provided ids are not valid integers.", ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleMenuService.ShowError($"An error occurred while deleting the Dishes. {ex.Message}");
                return true;
            }
        }
    }
}
