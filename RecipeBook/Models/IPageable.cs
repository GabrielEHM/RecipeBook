namespace RecipeBook.Models
{
    public interface IPageable<T>
    {
        static abstract string[] GetTableHeaders();
        abstract string[] ToTableRow();
    }
}
