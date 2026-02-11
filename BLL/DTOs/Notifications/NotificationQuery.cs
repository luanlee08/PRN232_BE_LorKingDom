namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Query parameters for filtering deliveries
    /// </summary>
    public class DeliveryQuery
    {
        public int? AccountId { get; set; }
        public string? TemplateCode { get; set; }
        public string? Status { get; set; } // Unread, Read
        public string? Keyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
