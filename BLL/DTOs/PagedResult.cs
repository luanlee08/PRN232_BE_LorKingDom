namespace BLL.DTOs
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
        public List<T> Items { get; set; } = new();
    }
}
