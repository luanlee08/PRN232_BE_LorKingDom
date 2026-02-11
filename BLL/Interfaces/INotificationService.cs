using BLL.DTOs;
using BLL.DTOs.Notifications;

namespace BLL.Interfaces
{
    public interface INotificationService
    {
        // Admin queries
        Task<ApiResponse<PagedResult<DeliveryResponse>>> GetDeliveriesAsync(DeliveryQuery query);
        Task<ApiResponse<DeliveryResponse>> GetDeliveryByIdAsync(long id);
        Task<ApiResponse<DeliveryStatsResponse>> GetStatsAsync();

        // User queries  
        Task<ApiResponse<List<DeliveryResponse>>> GetUserNotificationsAsync(int accountId, string? status, int limit);
        Task<ApiResponse<int>> GetUnreadCountAsync(int accountId);

        // Send notifications (creates delivery records)
        Task<ApiResponse<int>> SendNotificationAsync(SendNotificationRequest request, int createdByAccountId);

        // Mark as read
        Task<ApiResponse<bool>> MarkAsReadAsync(long deliveryId, int accountId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int accountId);

        // Delete
        Task<ApiResponse<bool>> DeleteDeliveryAsync(long deliveryId, int accountId);

        // Background job methods (called by Hangfire)
        Task ProcessScheduledNotificationJobAsync(SendNotificationRequest request, int createdBy, int? jobId);
    }
}
