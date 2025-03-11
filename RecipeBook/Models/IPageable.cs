namespace RecipeBook.Models
{
    public interface IPageable
    {
        static abstract string[] GetTableHeaders();
        abstract string[] ToTableRow();
    }
}
