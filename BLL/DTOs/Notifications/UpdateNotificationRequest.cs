namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Request to mark delivery(ies) as read
    /// </summary>
    public class MarkAsReadRequest
    {
        /// <summary>
        /// Specific delivery ID to mark as read (if provided)
        /// </summary>
        public long? DeliveryId { get; set; }

        /// <summary>
        /// If true, mark all unread deliveries as read for the user
        /// </summary>
        public bool MarkAllAsRead { get; set; } = false;
    }
}
