using System.Text;

namespace RecipeBook.Models
{
    class Dish : IPageable<Dish>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Servings { get; set; }
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public string Recipe { get; set; } = string.Empty;
        public List<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();
        public List<Menu> Menus { get; set; } = new List<Menu>();
        private int _usedIn;
        public int UsedIn
        {
            get
            {
                return Menus.Count > 0 ? Menus.Count : _usedIn;
            }
            set
            {
                if (Menus.Count == 0)
                {
                    _usedIn = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot set UsedIn directly when Menus list is not empty.");
                }
            }
        }
        public static string[] GetTableHeaders()
        {
            return ["Id", "Name", "Servings", "PrepTime", "CookTime", "Used in (Menus)"];
        }
        public override string ToString()
        {
            var result = new StringBuilder($"Id: {Id} Name: {Name}, Servings: {Servings}");
            if (PrepTime != null)
                result.Append($", PrepTime: {PrepTime}");
            if (CookTime != null)
                result.Append($", CookTime: {CookTime}");
            result.Append($", Used in (Menus): {UsedIn}");
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Servings.ToString(), PrepTime?.ToString() ?? "N/A", CookTime?.ToString() ?? "N/A", UsedIn.ToString()];
        }
    }
}
