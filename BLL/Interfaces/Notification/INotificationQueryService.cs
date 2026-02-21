using BLL.DTOs;
using BLL.DTOs.Notifications;

namespace BLL.Interfaces.Notification
{
    /// <summary>
    /// Service for querying notification deliveries (Read operations)
    /// </summary>
    public interface INotificationQueryService
    {
        /// <summary>
        /// Get deliveries with filtering and pagination (Admin)
        /// </summary>
        Task<ApiResponse<PagedResult<DeliveryResponse>>> GetDeliveriesAsync(DeliveryQuery query);

        /// <summary>
        /// Get delivery by ID
        /// </summary>
        Task<ApiResponse<DeliveryResponse>> GetDeliveryByIdAsync(long id);

        /// <summary>
        /// Get notification statistics
        /// </summary>
        Task<ApiResponse<DeliveryStatsResponse>> GetStatsAsync();

        /// <summary>
        /// Get user's notifications with filtering and pagination
        /// </summary>
        Task<ApiResponse<PagedResult<DeliveryResponse>>> GetUserNotificationsAsync(int accountId, UserNotificationQuery query);

        /// <summary>
        /// Get count of unread notifications for user
        /// </summary>
        Task<ApiResponse<int>> GetUnreadCountAsync(int accountId);
    }
}
