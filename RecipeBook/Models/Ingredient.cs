using Dapper;
using System.Text;

namespace RecipeBook.Models
{
    class Ingredient : Storable, IPageable
    {
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public List<Dish> Dishes { get; set; } = new List<Dish>();
        private int _usedIn;
        public int UsedIn
        {
            get
            {
                return Dishes.Count > 0 ? Dishes.Count : _usedIn;
            }
            set
            {
                if (Dishes.Count == 0)
                {
                    _usedIn = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot set UsedIn directly when Dishes list is not empty.");
                }
            }
        }
        public static string[] GetTableHeaders()
        {
            return ["Id", "Name", "Description", "Used in (Dishes)"];
        }
        public override string ToString()
        {
            var result = new StringBuilder($"Id: {Id} Name: {Name}");
            if (Description != null)
                result.Append($", Description: {Description}");
            if (Quantity != null)
                result.Append($", Quantity: {Quantity}");
            if (Unit != null)
                result.Append($", Unit: {Unit}");
            result.Append($", Used in (Dishes): {UsedIn}");
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Description ?? "", UsedIn.ToString()];
        }
    }
}
