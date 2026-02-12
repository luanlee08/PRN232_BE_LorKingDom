namespace BLL.DTOs.Moderation
{
    public class ReviewModerationRequest
    {
        public string ReviewText { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public int AccountId { get; set; }
        public int ProductId { get; set; }
    }
}
