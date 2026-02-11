namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Response DTO for a delivered notification
    /// </summary>
    public class DeliveryResponse
    {
        public long DeliveryId { get; set; }
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? CreatedByJobId { get; set; }
        public string? JobName { get; set; }
        public string TemplateCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public string Status { get; set; } = null!; // Unread, Read
        public DateTime CreatedAt { get; set; }
    }
}
