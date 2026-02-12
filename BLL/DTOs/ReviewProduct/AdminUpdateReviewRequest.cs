namespace BLL.DTOs.ReviewProduct
{
    public class AdminUpdateReviewRequest
    {
        public string? Status { get; set; } // Approved | Rejected | Pending
        public string? Visibility { get; set; } // Public | AuthorOnly
        public string? ModerationDetail { get; set; }
    }
}
