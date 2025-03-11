using Dapper;

namespace RecipeBook.Models
{
    abstract class Storable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public abstract DynamicParameters ToDynamicParameters();
    }
}
