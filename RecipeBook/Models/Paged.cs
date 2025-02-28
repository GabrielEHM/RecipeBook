namespace RecipeBook.Models
{
    public class Paged<T> where T : IPageable<T>
    {
        public List<T> Entities { get; set; }
        public PaginationInfo Pagination { get; set; }

        public Paged(List<T> entities, int page, int pageSize, int totalItems)
        {
            Entities = entities;
            Pagination = new PaginationInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }

    public class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int PageTotal
        {
            get
            {
                return (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }
    }
}
