using Dapper;

namespace RecipeBook.Models
{
    class Storable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual DynamicParameters ToDynamicParameters()
        {
            var parameters = new DynamicParameters();
            parameters.Add("Name", Name);
            parameters.Add("Description", Description);
            return parameters;
        }
    }
}
