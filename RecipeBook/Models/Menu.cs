using Dapper;
using System.Text;

namespace RecipeBook.Models
{
    class Menu : Storable, IPageable
    {
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
        public override DynamicParameters ToDynamicParameters()
        {
            throw new NotImplementedException();
        }
        public static string[] GetTableHeaders()
        {
            return ["Id", "Name", "Description", "Dish Count"];
        }
        public override string ToString()
        {
            var result = new StringBuilder($"Id: {Id} Name: {Name}");
            if (Description != null)
                result.Append($", Description: {Description}");
            result.Append($", Dish Count: {DishCount}");
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Description ?? "", DishCount.ToString()];
        }
    }
}
