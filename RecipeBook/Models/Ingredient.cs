using Azure;
using ConsoleTables;
using Dapper;
using System.Reflection.PortableExecutable;
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
        public string ToDetailedString(bool detailed = true, bool inline = false)
        {
            var result = new StringBuilder($"Id: {Id}");
            StringBuilder Append(string text) => _ = inline ? result.Append($", {text}") : result.AppendLine(text);
            Append($"Name: {Name}");
            if (Description != null)
                Append($"Description: {Description}");
            if (Quantity != null)
                Append($"Quantity: {Quantity}");
            if (Unit != null)
                Append($"Unit: {Unit}");
            if (detailed && UsedIn > 0)
            {
                var table = new ConsoleTable(Dish.GetTableHeaders());
                foreach (var dish in Dishes)
                {
                    table.AddRow(dish.ToTableRow());
                }
                result.AppendLine();
                result.AppendLine($"=== Dishes used in ===");
                result.AppendLine();
                result.AppendLine(table.ToString());

            } 
            else
            {
                Append($"Dishes used in: {UsedIn}");
            }
            return result.ToString();
        }
        public override string ToString()
        {
            return ToDetailedString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Description ?? "", UsedIn.ToString()];
        }
        protected override void FromReader(SqlMapper.GridReader reader)
        {
            Dishes = reader.Read<Dish>().ToList();
        }
    }
}
