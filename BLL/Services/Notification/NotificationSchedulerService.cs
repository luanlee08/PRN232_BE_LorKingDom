using BLL.DTOs.Notifications;
using BLL.Helpers.Notification;
using BLL.Interfaces;
using BLL.Interfaces.Notification;
using DAL.Interface;
using DAL.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Notification
{

    public class NotificationSchedulerService : INotificationSchedulerService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly AspLorKingDomContext _context;
        private readonly NotificationContentHelper _contentHelper;
        private readonly NotificationTargetHelper _targetHelper;
        private readonly INotificationRealtimeService? _realtimeService;
        private readonly ILogger<NotificationSchedulerService> _logger;

        public NotificationSchedulerService(
            INotificationRepository notificationRepo,
            AspLorKingDomContext context,
            NotificationContentHelper contentHelper,
            NotificationTargetHelper targetHelper,
            ILogger<NotificationSchedulerService> logger,
            INotificationRealtimeService? realtimeService = null)
        {
            _notificationRepo = notificationRepo;
            _context = context;
            _contentHelper = contentHelper;
            _targetHelper = targetHelper;
            _realtimeService = realtimeService;
            _logger = logger;
        }


        public async Task ProcessScheduledNotificationJobAsync(SendNotificationRequest request, int createdBy, int? jobId)
        {
            _logger.LogInformation("Processing scheduled notification job. BackgroundJobId: {JobId}", jobId);

            try
            {
                var deliveryCount = await CreateDeliveriesForTargetsAsync(request, createdBy, jobId);

                // Update BackgroundJob status
                if (jobId.HasValue)
                {
                    await UpdateBackgroundJobStatusAsync(jobId.Value, NotificationConstants.JobStatus.Success, $"Đã gửi {deliveryCount} thông báo");
                }

                _logger.LogInformation("Successfully processed scheduled notification job #{JobId}, sent {DeliveryCount} deliveries", jobId, deliveryCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled notification job #{JobId}", jobId);

                // Update BackgroundJob status to FAILED
                if (jobId.HasValue)
                {
                    await UpdateBackgroundJobStatusAsync(jobId.Value, NotificationConstants.JobStatus.Failed, ex.Message);
                }

                throw; // Hangfire will retry
            }
        }


        public async Task<int> CreateDeliveriesForTargetsAsync(SendNotificationRequest request, int createdBy, int? jobId)
        {
            // Get template and prepare content
            var (templateCode, title, message, payload) = await _contentHelper.PrepareContentAsync(request);

            if (string.IsNullOrWhiteSpace(templateCode))
            {
                _logger.LogError("TemplateCode is null after preparation");
                return 0;
            }

            // Determine target users
            var targetUserIds = await _targetHelper.GetTargetUserIdsAsync(request);

            if (!targetUserIds.Any())
            {
                _logger.LogWarning("No target users found for notification. TargetType: {TargetType}", request.TargetType);
                return 0;
            }

            // Create delivery records
            var deliveries = targetUserIds.Select(userId => new Delivery
            {
                AccountId = userId,
                CreatedByJobId = jobId,
                TemplateCode = templateCode,
                Title = title,
                Message = message,
                Payload = payload,
                ImageUrl = request.ImageUrl,
                ActionType = string.IsNullOrWhiteSpace(request.ActionType) ? null : request.ActionType,
                ActionTarget = string.IsNullOrWhiteSpace(request.ActionTarget) ? null : request.ActionTarget,
                CampaignId = request.CampaignId,
                Status = NotificationConstants.DeliveryStatus.Unread,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _notificationRepo.CreateDeliveriesAsync(deliveries);

            // Push real-time to each recipient's personal group (best-effort)
            if (_realtimeService != null)
            {
                var pushPayload = new { title, message, templateCode, createdAt = DateTime.UtcNow };
                foreach (var userId in targetUserIds)
                {
                    _ = _realtimeService.PushToUserAsync(userId, pushPayload);
                }
            }

            _logger.LogInformation("Created {DeliveryCount} delivery records", deliveries.Count);

            return deliveries.Count;
        }


        public async Task<BackgroundJob> CreateBackgroundJobRecordAsync(SendNotificationRequest request, DateTime scheduledAt)
        {
            var jobName = !string.IsNullOrEmpty(request.TemplateCode)
                ? $"Scheduled: {request.TemplateCode}"
                : $"Scheduled: {(request.Title ?? "Custom").Substring(0, Math.Min(50, (request.Title ?? "").Length))}";

            var bgJob = new BackgroundJob
            {
                JobName = jobName,
                IsEnabled = true,
                NextRunTime = scheduledAt,
                LastRunStatus = NotificationConstants.JobStatus.Pending
            };

            _context.BackgroundJobs.Add(bgJob);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created BackgroundJob #{JobId}: {JobName}", bgJob.JobId, jobName);
            return bgJob;
        }


        private async Task UpdateBackgroundJobStatusAsync(int jobId, string status, string message)
        {
            var bgJob = await _context.BackgroundJobs.FindAsync(jobId);
            if (bgJob != null)
            {
                bgJob.LastRunTime = DateTime.UtcNow;
                bgJob.LastRunStatus = status;
                bgJob.LastRunMessage = message;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated BackgroundJob #{JobId} status: {Status}", jobId, status);
            }
        }
    }
}
