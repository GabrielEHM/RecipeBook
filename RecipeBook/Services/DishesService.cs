﻿using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class DishesService : Service<Dish>
    {
        private readonly DishesRepository _repository;

        public DishesService(DishesRepository dishesRepository)
            : base()
        {
            _repository = dishesRepository;
        }

        public override bool ListAll(CommandAction back, int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            var backCommand = new Command("Go Back", (_) => { repeat = false; return back(); }, "back");
            bool ret = true;
            while (repeat)
            {
                ret = ConsoleMenuService.ListEntities(_repository.GetPage(page, pageSize), this, backCommand);
            }
            return ret;
        }

        public override bool GetById(string id, CommandAction back)
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
