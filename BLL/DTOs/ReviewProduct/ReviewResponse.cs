namespace BLL.DTOs.ReviewProduct
{
    public class ReviewResponse
    {
        public int ReviewProductId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string? AccountImage { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();

        public string Status { get; set; } = string.Empty; // Approved | Rejected | UnderReview
        public string? ModerationDetail { get; set; }

        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        public int EditCount { get; set; }
        public bool CanEdit { get; set; }

        public List<ReplyResponse> Replies { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
