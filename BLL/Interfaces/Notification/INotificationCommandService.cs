using BLL.DTOs;
using BLL.DTOs.Notifications;

namespace BLL.Interfaces.Notification
{
    /// <summary>
    /// Service for notification commands (Write operations)
    /// </summary>
    public interface INotificationCommandService
    {
        /// <summary>
        /// Send notification (creates delivery records, supports scheduling)
        /// </summary>
        /// <param name="request">Notification request</param>
        /// <param name="createdByAccountId">Account ID of creator</param>
        /// <param name="isSystemGenerated">If true, bypasses admin restrictions (for internal system use only)</param>
        Task<ApiResponse<int>> SendNotificationAsync(SendNotificationRequest request, int createdByAccountId, bool isSystemGenerated = false);

        /// <summary>
        /// Mark a delivery as read
        /// </summary>
        Task<ApiResponse<bool>> MarkAsReadAsync(long deliveryId, int accountId);

        /// <summary>
        /// Mark all user's deliveries as read
        /// </summary>
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int accountId);

        /// <summary>
        /// Delete a delivery (with ownership check)
        /// </summary>
        Task<ApiResponse<bool>> DeleteDeliveryAsync(long deliveryId, int accountId);

        /// <summary>
        /// Send notification when a review is rejected (called by ReviewProductService)
        /// </summary>
        Task SendReviewRejectionNotificationAsync(int reviewId, int accountId, string productName, string reason, string templateCode = "REVIEW_REJECTED");
    }
}
