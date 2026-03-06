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
        public string? Payload { get; set; }   // nullable — admin-sent notifs may have no payload
        public string? ImageUrl { get; set; }
        public string? ActionType { get; set; }
        public string? ActionTarget { get; set; }
        public string Status { get; set; } = null!; // Unread, Read
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Lightweight template DTO for admin dropdown selection
    /// </summary>
    public class TemplateDto
    {
        public string TemplateCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    /// <summary>
    /// Lightweight account search result for user targeting
    /// </summary>
    public class AccountSearchResult
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Image { get; set; }
    }
}
