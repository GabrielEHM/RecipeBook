using ConsoleTables;
using Dapper;
using RecipeBook.Database.Types;
using System.Text;

namespace RecipeBook.Models
{
    class Dish : Storable, IPageable
    {
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
        public override DynamicParameters ToDynamicParameters()
        {
            var parameters = base.ToDynamicParameters();
            parameters.Add("servings", Servings);
            parameters.Add("prepTime", PrepTime);
            parameters.Add("cookTime", CookTime);
            parameters.Add("recipe", Recipe);
            if (Ingredients is not null)
            {
                var ingredientList = new IngredientList(Ingredients);
                parameters.Add("IngredientsList", ingredientList.AsTableValuedParameter("dbo.IngredientList"));
            }
            return parameters;
        }
        public static string[] GetTableHeaders()
        {
            return ["Id", "Name", "Servings", "PrepTime", "CookTime", "Used in (Menus)"];
        }
        public override string ToDetailedString(bool detailed = true, bool inline = false)
        {
            var result = new StringBuilder(base.ToDetailedString());
            result.AppendWithInline($"Servings: {Servings}", inline);
            if (PrepTime != null)
                result.AppendWithInline($"Preparation time: {PrepTime}", inline);
            if (CookTime != null)
                result.AppendWithInline($"Cook time: {CookTime}", inline);
            if (detailed && Ingredients?.Count > 0)
            {
                var table = new ConsoleTable(Ingredient.GetTableHeaders(true));
                foreach (var ingredient in Ingredients)
                {
                    table.AddRow(ingredient.ToTableRow(true));
                }
                result.AppendLine();
                result.AppendLine($"=== Ingredients ===");
                result.AppendLine();
                result.AppendLine(table.ToString());
            }
            if (detailed)
            {
                result.AppendLine();
                result.AppendLine($"=== Recipe ===");
                result.AppendLine();
                result.AppendLine(value: Recipe.Replace("\\n", Environment.NewLine));
            }
            if (detailed && UsedIn > 0)
            {
                var table = new ConsoleTable(Menu.GetTableHeaders());
                foreach (var menu in Menus)
                {
                    table.AddRow(menu.ToTableRow());
                }
                result.AppendLine();
                result.AppendLine($"=== Menus used in ===");
                result.AppendLine();
                result.AppendLine(table.ToString());

            }
            else
            {
                result.AppendWithInline($"Menus used in: {UsedIn}", inline);
            }
            return result.ToString();
        }
        public string[] ToTableRow()
        {
            return [Id.ToString(), Name, Servings.ToString(), PrepTime?.ToString() ?? "N/A", CookTime?.ToString() ?? "N/A", UsedIn.ToString()];
        }
        protected override void FromReader(SqlMapper.GridReader reader)
        {
            Ingredients = reader.Read<Ingredient>().ToList();
            Menus = reader.Read<Menu>().ToList();
        }
    }
}
