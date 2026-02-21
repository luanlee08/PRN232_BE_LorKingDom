using BLL.DTOs.Notifications;
using DAL.Models;

namespace BLL.Interfaces.Notification
{
    /// <summary>
    /// Service for scheduled notification jobs (Background processing)
    /// </summary>
    public interface INotificationSchedulerService
    {
        /// <summary>
        /// Process scheduled notification job (called by Hangfire)
        /// </summary>
        Task ProcessScheduledNotificationJobAsync(SendNotificationRequest request, int createdBy, int? jobId);

        /// <summary>
        /// Create deliveries for target users
        /// </summary>
        Task<int> CreateDeliveriesForTargetsAsync(SendNotificationRequest request, int createdBy, int? jobId);

        /// <summary>
        /// Create background job record for tracking scheduled notification
        /// </summary>
        Task<BackgroundJob> CreateBackgroundJobRecordAsync(SendNotificationRequest request, DateTime scheduledAt);
    }
}
