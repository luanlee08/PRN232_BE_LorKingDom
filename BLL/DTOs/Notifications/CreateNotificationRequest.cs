using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Request to send notifications to users (immediate or scheduled)
    /// </summary>
    public class SendNotificationRequest
    {
        /// <summary>
        /// Template code from Templates table (recommended approach)
        /// </summary>
        [MaxLength(50)]
        public string? TemplateCode { get; set; }

        /// <summary>
        /// Custom title (used if TemplateCode not provided, or to override template)
        /// </summary>
        [MaxLength(255)]
        public string? Title { get; set; }

        /// <summary>
        /// Custom message (used if TemplateCode not provided, or to override template)
        /// </summary>
        [MaxLength(500)]
        public string? Message { get; set; }

        /// <summary>
        /// JSON parameters to replace placeholders in template
        /// Example: {"customerName":"John","orderNumber":"ORD-123"}
        /// </summary>
        public Dictionary<string, string>? Parameters { get; set; }

        /// <summary>
        /// Additional JSON payload data
        /// </summary>
        [MaxLength(1000)]
        public string? Payload { get; set; }

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
        /// Target user ID (required if TargetType = "User")
        /// </summary>
        public int? TargetUserId { get; set; }

        /// <summary>
        /// JSON condition for complex targeting (if TargetType = "Condition")
        /// Example: {"hasOrders":true,"registeredAfter":"2024-01-01"}
        /// </summary>
        public string? ConditionJson { get; set; }

        /// <summary>
        /// When to send (null = immediate, future datetime = scheduled)
        /// </summary>
        public DateTime? ScheduledAt { get; set; }
    }
}
