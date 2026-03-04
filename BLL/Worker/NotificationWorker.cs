using Hangfire;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{

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
