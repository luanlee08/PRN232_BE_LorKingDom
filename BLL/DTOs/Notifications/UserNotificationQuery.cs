namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Query parameters for filtering user notifications with pagination
    /// </summary>
    public class UserNotificationQuery
    {
        public string? Status { get; set; }           // "Unread", "Read"
        public string? TemplateCode { get; set; }     // Filter by category: PROMOTION, ORDER_UPDATE, WALLET_UPDATE, SYSTEM, etc.
        public string? Keyword { get; set; }           // Search in title/message
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
