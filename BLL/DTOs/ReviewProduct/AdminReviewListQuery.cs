namespace BLL.DTOs.ReviewProduct
{
    public class AdminReviewListQuery
    {
        public int? ProductId { get; set; }
        public string? Status { get; set; } // Approved | Rejected | Pending | All
        public int? Rating { get; set; }
        public string? SearchKeyword { get; set; } // Search in comment, account name, product name
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
