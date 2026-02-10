namespace BLL.DTOs.ReviewProduct
{
    public class ReactionRequest
    {
        public int ReviewProductId { get; set; }
        public string ReactionType { get; set; } = string.Empty; // Like | Dislike
    }
}
