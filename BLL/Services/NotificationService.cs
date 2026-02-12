using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly AspLorKingDomContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepo,
            ITemplateRepository templateRepo,
            AspLorKingDomContext context,
            IBackgroundJobClient backgroundJobClient,
            ILogger<NotificationService> logger)
        {
            _notificationRepo = notificationRepo;
            _templateRepo = templateRepo;
            _context = context;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        #region Admin Queries

        public async Task<ApiResponse<PagedResult<DeliveryResponse>>> GetDeliveriesAsync(DeliveryQuery query)
        {
            var (items, total) = await _notificationRepo.GetDeliveriesAsync(
                query.AccountId,
                query.TemplateCode,
                query.Status,
                query.Keyword,
                query.FromDate,
                query.ToDate,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<DeliveryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách deliveries thành công",
                Data = new PagedResult<DeliveryResponse>
                {
                    Items = items.Select(MapToResponse).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<DeliveryResponse>> GetDeliveryByIdAsync(long id)
        {
            var delivery = await _notificationRepo.GetDeliveryByIdAsync(id);

            if (delivery == null)
            {
                return new ApiResponse<DeliveryResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy delivery"
                };
            }

            return new ApiResponse<DeliveryResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin delivery thành công",
                Data = MapToResponse(delivery)
            };
        }

        public async Task<ApiResponse<DeliveryStatsResponse>> GetStatsAsync()
        {
            var stats = await _notificationRepo.GetStatsAsync();

            return new ApiResponse<DeliveryStatsResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thống kê thành công",
                Data = new DeliveryStatsResponse
                {
                    TotalDeliveries = stats.TotalDeliveries,
                    UnreadDeliveries = stats.UnreadDeliveries,
                    ReadDeliveries = stats.ReadDeliveries,
                    TodayDeliveries = stats.TodayDeliveries,
                    DeliveriesByTemplate = stats.DeliveriesByTemplate
                }
            };
        }

        #endregion

        #region User Queries

        public async Task<ApiResponse<List<DeliveryResponse>>> GetUserNotificationsAsync(int accountId, string? status, int limit)
        {
            var deliveries = await _notificationRepo.GetUserDeliveriesAsync(accountId, status, limit);

            return new ApiResponse<List<DeliveryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông báo của user thành công",
                Data = deliveries.Select(MapToResponse).ToList()
            };
        }

        public async Task<ApiResponse<int>> GetUnreadCountAsync(int accountId)
        {
            var count = await _notificationRepo.GetUnreadCountAsync(accountId);

            return new ApiResponse<int>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy số lượng thông báo chưa đọc thành công",
                Data = count
            };
        }

        #endregion

        #region Send Notifications

        public async Task<ApiResponse<int>> SendNotificationAsync(SendNotificationRequest request, int createdByAccountId)
        {
            // Validate: Either TemplateCode or both Title & Message must be provided
            if (string.IsNullOrWhiteSpace(request.TemplateCode) &&
                (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message)))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Vui lòng cung cấp TemplateCode hoặc cả Title và Message"
                };
            }

            // Validate target
            if (request.TargetType == "Role" && !request.TargetRoleId.HasValue)
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TargetRoleId là bắt buộc khi TargetType = 'Role'"
                };
            }

            if (request.TargetType == "User" && !request.TargetUserId.HasValue)
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TargetUserId là bắt buộc khi TargetType = 'User'"
                };
            }

            // Check if scheduled
            var scheduledAt = request.ScheduledAt ?? DateTime.UtcNow;

            if (scheduledAt > DateTime.UtcNow.AddMinutes(1)) // Scheduled for future
            {
                // Create BackgroundJob record for tracking
                var bgJob = await CreateBackgroundJobRecord(request, scheduledAt);

                // Use Hangfire to schedule the job
                var hangfireJobId = _backgroundJobClient.Schedule(
                    () => ProcessScheduledNotificationJobAsync(request, createdByAccountId, bgJob.JobId),
                    scheduledAt);

                _logger.LogInformation($"Scheduled Hangfire job {hangfireJobId} (BackgroundJob #{bgJob.JobId}) for {scheduledAt}");

                return new ApiResponse<int>
                {
                    Status = 202,
                    StatusMessage = "SCHEDULED",
                    Message = $"Thông báo đã được lên lịch gửi vào {scheduledAt}",
                    Data = bgJob.JobId
                };
            }
            else
            {
                // Send immediately
                var deliveryCount = await CreateDeliveriesForTargets(request, createdByAccountId, null);

                return new ApiResponse<int>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = $"Đã tạo {deliveryCount} delivery records",
                    Data = deliveryCount
                };
            }
        }

        #endregion

        #region Mark as Read

        public async Task<ApiResponse<bool>> MarkAsReadAsync(long deliveryId, int accountId)
        {
            await _notificationRepo.MarkAsReadAsync(deliveryId, accountId);

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đã đánh dấu là đã đọc",
                Data = true
            };
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int accountId)
        {
            await _notificationRepo.MarkAllAsReadAsync(accountId);

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đã đánh dấu tất cả là đã đọc",
                Data = true
            };
        }

        #endregion

        #region Delete

        public async Task<ApiResponse<bool>> DeleteDeliveryAsync(long deliveryId, int accountId)
        {
            // Check if delivery belongs to user
            var delivery = await _notificationRepo.GetDeliveryByIdAsync(deliveryId);

            if (delivery == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy delivery",
                    Data = false
                };
            }

            if (delivery.AccountId != accountId)
            {
                return new ApiResponse<bool>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền xóa delivery này",
                    Data = false
                };
            }

            await _notificationRepo.DeleteDeliveryAsync(deliveryId);

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đã xóa delivery",
                Data = true
            };
        }

        #endregion

        #region Background Jobs

        /// <summary>
        /// Called by Hangfire to process scheduled notification
        /// </summary>
        public async Task ProcessScheduledNotificationJobAsync(SendNotificationRequest request, int createdBy, int? jobId)
        {
            _logger.LogInformation($"Processing scheduled notification job. BackgroundJobId: {jobId}");

            try
            {
                var deliveryCount = await CreateDeliveriesForTargets(request, createdBy, jobId);

                // Update BackgroundJob status
                if (jobId.HasValue)
                {
                    await UpdateBackgroundJobStatus(jobId.Value, "SUCCESS", $"Đã gửi {deliveryCount} thông báo");
                }

                _logger.LogInformation($"Successfully processed scheduled notification job #{jobId}, sent {deliveryCount} deliveries");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing scheduled notification job #{jobId}");

                // Update BackgroundJob status to FAILED
                if (jobId.HasValue)
                {
                    await UpdateBackgroundJobStatus(jobId.Value, "FAILED", ex.Message);
                }

                throw; // Hangfire will retry
            }
        }

        #endregion

        #region Review Moderation Notifications

        /// <summary>
        /// Send notification to user when their review is rejected
        /// </summary>
        public async Task SendReviewRejectionNotificationAsync(int reviewId, int accountId, string productName, string reason)
        {
            try
            {
                _logger.LogInformation($"Sending review rejection notification for ReviewId: {reviewId}, AccountId: {accountId}");

                // Prepare notification content
                var parameters = new Dictionary<string, string>
                {
                    { "productName", productName },
                    { "reason", reason }
                };

                var request = new SendNotificationRequest
                {
                    TemplateCode = "REVIEW_REJECTED",
                    TargetType = "User",
                    TargetUserId = accountId,
                    Parameters = parameters,
                    Payload = JsonSerializer.Serialize(new { reviewId, productName, reason }, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = false
                    })
                };

                // Send immediately (not scheduled)
                await CreateDeliveriesForTargets(request, accountId, null);

                _logger.LogInformation($"Successfully sent review rejection notification for ReviewId: {reviewId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send review rejection notification for ReviewId: {reviewId}");
                // Don't throw - notification failure shouldn't break the moderation process
            }
        }

        #endregion

        #region Helper Methods

        private async Task<DAL.Models.BackgroundJob> CreateBackgroundJobRecord(SendNotificationRequest request, DateTime scheduledAt)
        {
            var jobName = !string.IsNullOrEmpty(request.TemplateCode)
                ? $"Scheduled: {request.TemplateCode}"
                : $"Scheduled: {(request.Title ?? "Custom").Substring(0, Math.Min(50, (request.Title ?? "").Length))}";

            var bgJob = new DAL.Models.BackgroundJob
            {
                JobName = jobName,
                IsEnabled = true,
                NextRunTime = scheduledAt,
                LastRunStatus = "PENDING"
            };

            _context.BackgroundJobs.Add(bgJob);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created BackgroundJob #{bgJob.JobId}: {jobName}");
            return bgJob;
        }

        private async Task UpdateBackgroundJobStatus(int jobId, string status, string message)
        {
            var bgJob = await _context.BackgroundJobs.FindAsync(jobId);
            if (bgJob != null)
            {
                bgJob.LastRunTime = DateTime.UtcNow;
                bgJob.LastRunStatus = status;
                bgJob.LastRunMessage = message;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated BackgroundJob #{jobId} status: {status}");
            }
        }

        private async Task<int> CreateDeliveriesForTargets(SendNotificationRequest request, int createdBy, int? jobId)
        {
            // Get template and prepare content
            var (templateCode, title, message, payload) = await PrepareNotificationContent(request);

            if (string.IsNullOrWhiteSpace(templateCode))
            {
                _logger.LogError("TemplateCode is null after preparation");
                return 0;
            }

            // Determine target users
            var targetUserIds = await GetTargetUserIds(request);

            if (!targetUserIds.Any())
            {
                _logger.LogWarning($"No target users found for notification. TargetType: {request.TargetType}");
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
                Status = "Unread",
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _notificationRepo.CreateDeliveriesAsync(deliveries);

            _logger.LogInformation($"Created {deliveries.Count} delivery records");

            return deliveries.Count;
        }

        private async Task<(string templateCode, string title, string message, string payload)> PrepareNotificationContent(SendNotificationRequest request)
        {
            string templateCode;
            string title;
            string message;
            string payload = request.Payload ?? "{}";

            // If using template
            if (!string.IsNullOrWhiteSpace(request.TemplateCode))
            {
                var template = await _templateRepo.GetByCodeAsync(request.TemplateCode);

                if (template == null)
                {
                    _logger.LogWarning($"Template not found: {request.TemplateCode}");
                    // Fallback to custom content or throw
                    if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.Message))
                    {
                        templateCode = "CUSTOM";
                        title = request.Title;
                        message = request.Message;
                    }
                    else
                    {
                        throw new Exception($"Template '{request.TemplateCode}' not found and no fallback content provided");
                    }
                }
                else
                {
                    templateCode = template.TemplateCode;

                    // Use template, allow override from request
                    title = request.Title ?? template.TitleTemplate;
                    message = request.Message ?? template.MessageTemplate;

                    // Replace parameters if provided
                    if (request.Parameters != null && request.Parameters.Any())
                    {
                        title = ReplaceTemplateParameters(title, request.Parameters);
                        message = ReplaceTemplateParameters(message, request.Parameters);
                    }
                }
            }
            else
            {
                // Custom content
                templateCode = "CUSTOM";
                title = request.Title!;
                message = request.Message!;
            }

            return (templateCode, title, message, payload);
        }

        private async Task<List<int>> GetTargetUserIds(SendNotificationRequest request)
        {
            return request.TargetType switch
            {
                "All" => await _context.Accounts
                    .Where(a => !a.IsDeleted && a.Status == "Active")
                    .Select(a => a.AccountId)
                    .ToListAsync(),

                "Role" => await _context.Accounts
                    .Where(a => !a.IsDeleted && a.Status == "Active" && a.RoleId == request.TargetRoleId)
                    .Select(a => a.AccountId)
                    .ToListAsync(),

                "User" => new List<int> { request.TargetUserId!.Value },

                "Condition" => await GetUsersByCondition(request.ConditionJson),

                _ => new List<int>()
            };
        }

        private async Task<List<int>> GetUsersByCondition(string? conditionJson)
        {
            // TODO: Implement complex condition handling
            // For now, return empty list
            _logger.LogWarning("Condition-based targeting not yet implemented");
            return await Task.FromResult(new List<int>());
        }

        private string ReplaceTemplateParameters(string text, Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
                return text;

            foreach (var param in parameters)
            {
                // Support both {{key}} and {key} formats
                text = text.Replace($"{{{{{param.Key}}}}}", param.Value);
                text = text.Replace($"{{{param.Key}}}", param.Value);
            }

            return text;
        }

        private DeliveryResponse MapToResponse(Delivery delivery)
        {
            return new DeliveryResponse
            {
                DeliveryId = delivery.DeliveryId,
                AccountId = delivery.AccountId,
                AccountName = delivery.Account?.AccountName,
                AccountEmail = delivery.Account?.Email,
                CreatedByJobId = delivery.CreatedByJobId,
                JobName = delivery.CreatedByJob?.JobName,
                TemplateCode = delivery.TemplateCode,
                Title = delivery.Title,
                Message = delivery.Message,
                Payload = delivery.Payload,
                Status = delivery.Status,
                CreatedAt = delivery.CreatedAt
            };
        }

        #endregion
    }
}
