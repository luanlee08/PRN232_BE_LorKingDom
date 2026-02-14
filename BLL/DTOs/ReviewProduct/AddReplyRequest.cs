namespace BLL.DTOs.ReviewProduct
{
    public class AddReplyRequest
    {
        public int ReviewProductId { get; set; }
        public string ReplyText { get; set; } = string.Empty;
    }
}
