using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Helpers.Notification;
using BLL.Interfaces.Notification;
using DAL.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BLL.Services.Notification
{

    public class NotificationCommandService : INotificationCommandService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly INotificationSchedulerService _schedulerService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<NotificationCommandService> _logger;

        public NotificationCommandService(
            INotificationRepository notificationRepo,
            INotificationSchedulerService schedulerService,
            IBackgroundJobClient backgroundJobClient,
            ILogger<NotificationCommandService> logger)
        {
            _notificationRepo = notificationRepo;
            _schedulerService = schedulerService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }
        public async Task<ApiResponse<int>> SendNotificationAsync(SendNotificationRequest request, int createdByAccountId, bool isSystemGenerated = false)
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


            if (!isSystemGenerated && !string.IsNullOrWhiteSpace(request.TemplateCode))
            {
                if (NotificationConstants.SystemOnlyTemplateCodes.RestrictedCodes.Contains(request.TemplateCode))
                {
                    _logger.LogWarning("Admin (AccountId={AccountId}) attempted to create system-only notification with TemplateCode={TemplateCode}",
                        createdByAccountId, request.TemplateCode);

                    return new ApiResponse<int>
                    {
                        Status = 403,
                        StatusMessage = "FORBIDDEN",
                        Message = $"Loại thông báo '{request.TemplateCode}' chỉ được tạo tự động bởi hệ thống. " +
                                  "Admin chỉ có thể tạo: Khuyến mãi, Voucher, Thông báo hệ thống, Hỗ trợ khách hàng."
                    };
                }
            }

            // Validate payload JSON structure and content
            if (!string.IsNullOrWhiteSpace(request.Payload))
            {
                // Check payload max size (database constraint is 1000 chars)
                if (request.Payload.Length > 1000)
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Payload vượt quá giới hạn 1000 ký tự (hiện tại: {request.Payload.Length})"
                    };
                }

                // Validate JSON format
                try
                {
                    var payloadObj = JsonSerializer.Deserialize<JsonElement>(request.Payload);

                    // Check for suspicious URLs in payload
                    if (payloadObj.TryGetProperty("link", out var linkElement))
                    {
                        var link = linkElement.GetString();
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            // Validate link is relative (starts with /) or internal domain
                            if (!link.StartsWith("/") &&
                                (link.StartsWith("http://") || link.StartsWith("https://")))
                            {
                                // External URL detected - only allow for system-generated notifications
                                if (!isSystemGenerated)
                                {
                                    _logger.LogWarning("Admin (AccountId={AccountId}) attempted to create notification with external URL: {Link}",
                                        createdByAccountId, link);

                                    return new ApiResponse<int>
                                    {
                                        Status = 400,
                                        StatusMessage = "FAILED",
                                        Message = "Payload chỉ được chứa đường dẫn nội bộ (bắt đầu bằng /). Không cho phép URL bên ngoài."
                                    };
                                }
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    return new ApiResponse<int>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Payload không phải là JSON hợp lệ"
                    };
                }
            }

            // Validate target
            if (request.TargetType == NotificationConstants.TargetTypes.Role && !request.TargetRoleId.HasValue)
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TargetRoleId là bắt buộc khi TargetType = 'Role'"
                };
            }

            if (request.TargetType == NotificationConstants.TargetTypes.User && (request.TargetUserIds == null || !request.TargetUserIds.Any()))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TargetUserIds là bắt buộc và không được rỗng khi TargetType = 'User'"
                };
            }

            // Check if scheduled
            var scheduledAt = request.ScheduledAt ?? DateTime.UtcNow;

            if (scheduledAt > DateTime.UtcNow.AddMinutes(1)) // Scheduled for future
            {
                // Create BackgroundJob record for tracking
                var bgJob = await _schedulerService.CreateBackgroundJobRecordAsync(request, scheduledAt);

                // Use Hangfire to schedule the job
                var hangfireJobId = _backgroundJobClient.Schedule(
                    () => _schedulerService.ProcessScheduledNotificationJobAsync(request, createdByAccountId, bgJob.JobId),
                    scheduledAt);

                _logger.LogInformation("Scheduled Hangfire job {HangfireJobId} (BackgroundJob #{JobId}) for {ScheduledAt}", hangfireJobId, bgJob.JobId, scheduledAt);

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
                var deliveryCount = await _schedulerService.CreateDeliveriesForTargetsAsync(request, createdByAccountId, null);

                return new ApiResponse<int>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = $"Đã tạo {deliveryCount} delivery records",
                    Data = deliveryCount
                };
            }
        }

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

        public async Task SendReviewRejectionNotificationAsync(int reviewId, int accountId, string productName, string reason, string templateCode = "REVIEW_REJECTED")
        {
            try
            {
                _logger.LogInformation("Sending review rejection notification for ReviewId: {ReviewId}, AccountId: {AccountId}", reviewId, accountId);

                // Prepare notification content
                var parameters = new Dictionary<string, string>
                {
                    { "productName", productName },
                    { "reason", reason }
                };

                var request = new SendNotificationRequest
                {
                    TemplateCode = templateCode,
                    TargetType = NotificationConstants.TargetTypes.User,
                    TargetUserIds = new List<int> { accountId },
                    Parameters = parameters,
                    Payload = JsonSerializer.Serialize(new { reviewId, productName, reason }, new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = false
                    })
                };

                // Send immediately (not scheduled)
                await _schedulerService.CreateDeliveriesForTargetsAsync(request, accountId, null);

                _logger.LogInformation("Successfully sent review rejection notification for ReviewId: {ReviewId}", reviewId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send review rejection notification for ReviewId: {ReviewId}", reviewId);
                // Don't throw - notification failure shouldn't break the moderation process
            }
        }
    }
}
