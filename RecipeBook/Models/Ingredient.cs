using ConsoleTables;
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
        public static string[] GetTableHeaders(bool inDish)
        {
            if (!inDish) return GetTableHeaders();
            return ["Id", "Name", "Quantity", "Unit"];
        }
        public override string ToDetailedString(bool detailed = true, bool inline = false)
        {
            var result = new StringBuilder(base.ToDetailedString());
            if (Quantity != null)
                result.AppendWithInline($"Quantity: {Quantity}", inline);
            if (Unit != null)
                result.AppendWithInline($"Unit: {Unit}", inline);
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
                result.AppendWithInline($"Dishes used in: {UsedIn}", inline);
            }
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Description ?? string.Empty, UsedIn.ToString()];
        }
        public string[] ToTableRow(bool inDish)
        {
            if (!inDish) return ToTableRow();
            return [Id.ToString(), Name, Quantity?.ToString() ?? "Al gusto", Unit ?? string.Empty];
        }
        protected override void FromReader(SqlMapper.GridReader reader)
        {
            Dishes = reader.Read<Dish>().ToList();
        }
        protected override List<string> GetFillablePropertiesNames()
        {
            var names = base.GetFillablePropertiesNames();
            names.AddRange(["Quantity", "Unit"]);
            return names;
        }
    }
}
