using Humanizer;
using RecipeBook.Database.Repositories;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class IngredientsService : Service<Ingredient>
    {
        private readonly IngredientsRepository _ingredientsRepository;

        public IngredientsService(IngredientsRepository ingredientsRepository)
            : base()
        {
            _ingredientsRepository = ingredientsRepository;
        }

        public override bool ListAll(int page = 1, int pageSize = 10)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = ConsoleMenuService.ListEntities(_ingredientsRepository.GetPage(page, pageSize), this);
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
                var results = new Dictionary<int, bool>();
                if (!ConsoleMenuService.Confirm($"You are about to attempt to delete the following ingredients: {convertedIds.Humanize()}"))
                {
                    return true;
                }
                foreach (int id in convertedIds)
                {
                    results.Add(id, _ingredientsRepository.Remove(id));
                }
                var deleted = results.Where(r => r.Value).Select(r => r.Key).ToArray();
                var used = results.Where(r => !r.Value).Select(r => r.Key).ToArray();
                var messages = new List<string>();
                if (deleted.Length > 0)
                {
                    messages.Add($"The following ingredients were deleted: {deleted.Humanize(",")}");
                }
                if (used.Length > 0)
                {
                    messages.Add($"The following ingredients are being used and cannot be deleted: {used.Humanize(",")}");
                }
                ConsoleMenuService.ShowMessage([.. messages]);
                return true;
            }
            catch (FormatException ex)
            {
                return CommandList.InvalidChoice("One or more if the provided ids are not valid integers.", ex.Message);
            }
            catch (Exception ex)
            {
                ConsoleMenuService.ShowError($"An error occurred while deleting the ingredients. {ex.Message}");
                return true;
            }
        }
    }
}
