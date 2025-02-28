namespace RecipeBook.Models
{
    class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Dish> Dishes { get; set; } = new List<Dish>();
        private int _dishCount;
        public int DishCount
        {
            get
            {
                return Dishes.Count > 0 ? Dishes.Count : _dishCount;
            }
            set
            {
                if (Dishes.Count == 0)
                {
                    _dishCount = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot set DishCount directly when Dishes list is not empty.");
                }
            }
        }
    }
}
