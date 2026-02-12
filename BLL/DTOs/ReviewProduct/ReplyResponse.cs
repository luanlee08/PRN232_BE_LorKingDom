namespace BLL.DTOs.ReviewProduct
{
    public class ReplyResponse
    {
        public int ReviewProductReplyId { get; set; }
        public int ReviewProductId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string? AccountImage { get; set; }
        public string ReplyText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
