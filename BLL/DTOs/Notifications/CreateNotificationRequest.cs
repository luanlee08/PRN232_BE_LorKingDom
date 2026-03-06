using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Request to send notifications to users (immediate or scheduled)
    /// </summary>
    public class SendNotificationRequest
    {
        /// <summary>
        /// Template code from Templates table (optional — auto-fills title/message from DB when provided)
        /// </summary>
        [MaxLength(50)]
        public string? TemplateCode { get; set; }

        /// <summary>
        /// Custom title (used if TemplateCode not provided, or to override template default)
        /// </summary>
        [MaxLength(255)]
        public string? Title { get; set; }

        /// <summary>
        /// Custom message (used if TemplateCode not provided, or to override template default)
        /// </summary>
        [MaxLength(500)]
        public string? Message { get; set; }

        /// <summary>
        /// JSON parameters to replace placeholders in template
        /// Example: {"customerName":"John","orderNumber":"ORD-123"}
        /// </summary>
        public Dictionary<string, string>? Parameters { get; set; }

        /// <summary>
        /// Additional JSON payload data (legacy field, kept for backward compat)
        /// </summary>
        [MaxLength(1000)]
        public string? Payload { get; set; }

        /// <summary>
        /// Optional image URL displayed in the notification card
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Action type for click navigation: 'product' | 'voucher' | 'url' | 'none'
        /// </summary>
        [MaxLength(20)]
        public string? ActionType { get; set; }

        /// <summary>
        /// Action target: product ID string, voucher code, or full URL (based on ActionType)
        /// </summary>
        [MaxLength(500)]
        public string? ActionTarget { get; set; }

        /// <summary>
        /// Target type: "All", "Role", "User", "Condition"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string TargetType { get; set; } = null!;

        /// <summary>
        /// Target role ID (required if TargetType = "Role")
        /// </summary>
        public int? TargetRoleId { get; set; }

        /// <summary>
        /// Target user IDs (required if TargetType = "User") — supports multi-select
        /// </summary>
        public List<int>? TargetUserIds { get; set; }

        /// <summary>
        /// JSON condition for complex targeting (if TargetType = "Condition") — reserved for future use
        /// </summary>
        public string? ConditionJson { get; set; }

        /// <summary>
        /// When to send (null = immediate, future datetime = scheduled via Hangfire)
        /// </summary>
        public DateTime? ScheduledAt { get; set; }

        /// <summary>
        /// Optional campaign ID — links all delivery records for analytics.
        /// Set internally by CampaignService; not exposed in admin direct-send form.
        /// </summary>
        public int? CampaignId { get; set; }
    }
}
