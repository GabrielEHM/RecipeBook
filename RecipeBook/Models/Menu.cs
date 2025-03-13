using ConsoleTables;
using Dapper;
using RecipeBook.Database.Types;
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
            var parameters = base.ToDynamicParameters();
            var ingredientList = new DishList(Dishes);
            parameters.Add("DishList", ingredientList.AsTableValuedParameter("dbo.DishList"));
            return parameters;
        }
        public static string[] GetTableHeaders()
        {
            return ["Id", "Name", "Description", "Dish Count"];
        }
        public override string ToDetailedString(bool detailed = true, bool inline = false)
        {
            var result = new StringBuilder(base.ToDetailedString());
            if (detailed && DishCount > 0)
            {
                var table = new ConsoleTable(Dish.GetTableHeaders());
                foreach (var dish in Dishes)
                {
                    table.AddRow(dish.ToTableRow());
                }
                result.AppendLine();
                result.AppendLine($"=== Dishes ===");
                result.AppendLine();
                result.AppendLine(table.ToString());

            }
            else
            {
                result.AppendWithInline($"Dish Count: {DishCount}", inline);
            }
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Description ?? string.Empty, DishCount.ToString()];
        }
        protected override void FromReader(SqlMapper.GridReader reader)
        {
            Dishes = reader.Read<Dish>().ToList();
        }
    }
}
