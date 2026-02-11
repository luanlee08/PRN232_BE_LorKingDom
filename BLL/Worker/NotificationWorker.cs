using BLL.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// NotificationWorker - No longer needed for Delivery-based notifications
    /// Hangfire now schedules individual jobs directly via BackgroundJob.Schedule()
    /// This class is kept for backward compatibility but does nothing
    /// </summary>
    public class NotificationWorker
    {
        private readonly ILogger<NotificationWorker> _logger;

        public NotificationWorker(ILogger<NotificationWorker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Legacy method - no longer used
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task ProcessScheduledNotificationsJob()
        {
            _logger.LogInformation("NotificationWorker is deprecated. Notifications are now scheduled directly via Hangfire.");
            await Task.CompletedTask;
        }
    }
}
