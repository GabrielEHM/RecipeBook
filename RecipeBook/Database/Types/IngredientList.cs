using Microsoft.Data.SqlClient.Server;
using RecipeBook.Models;

namespace RecipeBook.Database.Types
{
    class IngredientListRow : SqlDataRecord
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
    }
    class IngredientList : List<IngredientListRow>
    {
        public IngredientList(List<Ingredient> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                var row = new IngredientListRow { Name = ingredient.Name, Description = ingredient.Description, Quantity = ingredient.Quantity, Unit = ingredient.Unit };
                this.Add(row);
            }
        }
    }
}
