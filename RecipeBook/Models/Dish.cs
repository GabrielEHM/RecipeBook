namespace RecipeBook.Models
{
    class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Servings { get; set; }
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public string Recipe { get; set; }
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

        public Dish(int id, string name, string? description, int servings, int? prepTime, int? cookTime, string recipe)
        {
            (Id, Name, Description, Servings, PrepTime, CookTime, Recipe) = (id, name, description, servings, prepTime, cookTime, recipe);

            if (PrepTime is null && CookTime is null)
            {
                throw new ArgumentException("Either PrepTime or CookTime must be provided.");
            }
        }
    }
}
