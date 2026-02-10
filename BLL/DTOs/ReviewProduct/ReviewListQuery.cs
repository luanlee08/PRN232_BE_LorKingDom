namespace BLL.DTOs.ReviewProduct
{
    public class ReviewListQuery
    {
        public int ProductId { get; set; }
        public string? Status { get; set; } = "Approved";
        public int? Rating { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
