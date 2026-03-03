using BLL.DTOs;
using BLL.DTOs.Campaigns;
using BLL.DTOs.Notifications;
using BLL.Interfaces;
using BLL.Interfaces.Notification;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly AspLorKingDomContext _context;
        private readonly INotificationCommandService _notificationCommandService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(
            AspLorKingDomContext context,
            INotificationCommandService notificationCommandService,
            IBackgroundJobClient backgroundJobClient,
            ILogger<CampaignService> logger)
        {
            _context = context;
            _notificationCommandService = notificationCommandService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────
        // Queries
        // ─────────────────────────────────────────────────────────────

        public async Task<ApiResponse<PagedResult<CampaignResponse>>> GetCampaignsAsync(CampaignQuery query)
        {
            var q = _context.Campaigns
                .Include(c => c.CreatedByAccount)
                .Include(c => c.CampaignTargets)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
                q = q.Where(c => c.CampaignName.Contains(query.Keyword));

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(c => c.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.SourceType))
                q = q.Where(c => c.SourceType == query.SourceType);

            if (query.FromDate.HasValue)
                q = q.Where(c => c.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                q = q.Where(c => c.CreatedAt <= query.ToDate.Value);

            var total = await q.CountAsync();

            var campaigns = await q
                .OrderByDescending(c => c.CreatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var campaignIds = campaigns.Select(c => c.CampaignId).ToList();

            // Aggregated delivery stats per campaign
            var stats = await _context.Deliveries
                .Where(d => d.CampaignId.HasValue && campaignIds.Contains(d.CampaignId!.Value))
                .GroupBy(d => d.CampaignId!.Value)
                .Select(g => new
                {
                    CampaignId = g.Key,
                    TotalSent = g.Count(),
                    TotalRead = g.Count(d => d.Status == "Read")
                })
                .ToListAsync();

            // Click stats per campaign
            var clickStats = await _context.DeliveryActions
                .Where(da => da.ActionType == "Click" &&
                             _context.Deliveries.Any(d => d.DeliveryId == da.DeliveryId &&
                                                          d.CampaignId.HasValue &&
                                                          campaignIds.Contains(d.CampaignId!.Value)))
                .Join(_context.Deliveries.Where(d => d.CampaignId.HasValue && campaignIds.Contains(d.CampaignId!.Value)),
                      da => da.DeliveryId,
                      d => d.DeliveryId,
                      (da, d) => new { d.CampaignId, da.ActionId })
                .GroupBy(x => x.CampaignId!.Value)
                .Select(g => new { CampaignId = g.Key, TotalClicked = g.Count() })
                .ToListAsync();

            var items = campaigns.Select(c =>
            {
                var s = stats.FirstOrDefault(x => x.CampaignId == c.CampaignId);
                var cs = clickStats.FirstOrDefault(x => x.CampaignId == c.CampaignId);
                return MapToResponse(c, s?.TotalSent ?? 0, s?.TotalRead ?? 0, cs?.TotalClicked ?? 0);
            }).ToList();

            return new ApiResponse<PagedResult<CampaignResponse>>
            {
                Status = 200,
                StatusMessage = "OK",
                Data = new PagedResult<CampaignResponse>
                {
                    Items = items,
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<CampaignDetailResponse>> GetCampaignByIdAsync(int id)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.CreatedByAccount)
                .Include(c => c.CampaignTargets)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (campaign == null)
                return new ApiResponse<CampaignDetailResponse> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Chiến dịch không tồn tại" };

            // Delivery analytics
            var deliveries = await _context.Deliveries
                .Include(d => d.Account)
                .Where(d => d.CampaignId == id)
                .AsNoTracking()
                .ToListAsync();

            var deliveryIds = deliveries.Select(d => d.DeliveryId).ToList();

            var actions = await _context.DeliveryActions
                .Where(da => deliveryIds.Contains(da.DeliveryId))
                .AsNoTracking()
                .ToListAsync();

            // Build recipient rows
            var recipients = deliveries.Select(d =>
            {
                var readAction = actions
                    .Where(a => a.DeliveryId == d.DeliveryId && a.ActionType == "Read")
                    .OrderBy(a => a.OccurredAt)
                    .FirstOrDefault();
                var clickAction = actions
                    .Where(a => a.DeliveryId == d.DeliveryId && a.ActionType == "Click")
                    .OrderBy(a => a.OccurredAt)
                    .FirstOrDefault();

                return new RecipientRow
                {
                    DeliveryId = d.DeliveryId,
                    AccountId = d.AccountId,
                    AccountName = d.Account?.AccountName,
                    AccountEmail = d.Account?.Email,
                    DeliveryStatus = d.Status,
                    DeliveredAt = d.CreatedAt,
                    ReadAt = readAction?.OccurredAt,
                    ClickedAt = clickAction?.OccurredAt
                };
            }).ToList();

            // Click timeline (group by date)
            var clickTimeline = actions
                .Where(a => a.ActionType == "Click")
                .GroupBy(a => a.OccurredAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new TimelinePoint
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Clicks = g.Count(),
                    Reads = 0 // filled below
                })
                .ToList();

            // Merge reads into timeline
            var readsByDate = actions
                .Where(a => a.ActionType == "Read")
                .GroupBy(a => a.OccurredAt.Date)
                .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Count());

            foreach (var point in clickTimeline)
                if (readsByDate.TryGetValue(point.Date, out var readCount))
                    point.Reads = readCount;

            // Add read-only dates not in clicks
            foreach (var (date, cnt) in readsByDate)
                if (!clickTimeline.Any(p => p.Date == date))
                    clickTimeline.Add(new TimelinePoint { Date = date, Reads = cnt, Clicks = 0 });

            clickTimeline = [.. clickTimeline.OrderBy(p => p.Date)];

            int totalSent = deliveries.Count;
            int totalRead = deliveries.Count(d => d.Status == "Read");
            int totalClicked = actions.Count(a => a.ActionType == "Click");

            var detail = new CampaignDetailResponse
            {
                Recipients = recipients,
                ClickTimeline = clickTimeline
            };

            // Copy scalar fields from base mapping
            var baseResponse = MapToResponse(campaign, totalSent, totalRead, totalClicked);
            detail.CampaignId = baseResponse.CampaignId;
            detail.CampaignName = baseResponse.CampaignName;
            detail.TemplateCode = baseResponse.TemplateCode;
            detail.TitleOverride = baseResponse.TitleOverride;
            detail.MessageOverride = baseResponse.MessageOverride;
            detail.SourceType = baseResponse.SourceType;
            detail.TargetType = baseResponse.TargetType;
            detail.Status = baseResponse.Status;
            detail.ScheduledAt = baseResponse.ScheduledAt;
            detail.EventKey = baseResponse.EventKey;
            detail.ImageUrl = baseResponse.ImageUrl;
            detail.ActionType = baseResponse.ActionType;
            detail.ActionTarget = baseResponse.ActionTarget;
            detail.CreatedByAccountId = baseResponse.CreatedByAccountId;
            detail.CreatedByAccountName = baseResponse.CreatedByAccountName;
            detail.CreatedAt = baseResponse.CreatedAt;
            detail.UpdatedAt = baseResponse.UpdatedAt;
            detail.TotalRecipients = baseResponse.TotalRecipients;
            detail.TotalSent = baseResponse.TotalSent;
            detail.TotalRead = baseResponse.TotalRead;
            detail.TotalClicked = baseResponse.TotalClicked;
            detail.TargetValues = baseResponse.TargetValues;

            return new ApiResponse<CampaignDetailResponse> { Status = 200, StatusMessage = "OK", Data = detail };
        }

        // ─────────────────────────────────────────────────────────────
        // Commands
        // ─────────────────────────────────────────────────────────────

        public async Task<ApiResponse<CampaignResponse>> CreateCampaignAsync(CreateCampaignRequest req, int createdByAccountId)
        {
            // Validate: need template OR both overrides
            if (string.IsNullOrWhiteSpace(req.TemplateCode) &&
                (string.IsNullOrWhiteSpace(req.TitleOverride) || string.IsNullOrWhiteSpace(req.MessageOverride)))
            {
                return new ApiResponse<CampaignResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Phải chọn template hoặc nhập cả TitleOverride và MessageOverride"
                };
            }

            var campaign = new Campaign
            {
                CampaignName = req.CampaignName,
                TemplateCode = req.TemplateCode,
                TitleOverride = req.TitleOverride,
                MessageOverride = req.MessageOverride,
                SourceType = req.SourceType,
                TargetType = req.TargetType,
                Status = req.ScheduledAt.HasValue && req.ScheduledAt > DateTime.UtcNow.AddMinutes(1)
                    ? "Scheduled" : "Draft",
                ScheduledAt = req.ScheduledAt,
                EventKey = req.EventKey,
                ImageUrl = req.ImageUrl,
                ActionType = req.ActionType,
                ActionTarget = req.ActionTarget,
                CreatedByAccountId = createdByAccountId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            // Save targets
            if (req.TargetValues.Any())
            {
                var targets = req.TargetValues.Select(v => new CampaignTarget
                {
                    CampaignId = campaign.CampaignId,
                    TargetValue = v
                }).ToList();
                _context.CampaignTargets.AddRange(targets);
                await _context.SaveChangesAsync();
            }

            // If status is Scheduled, enqueue Hangfire job
            if (campaign.Status == "Scheduled" && req.ScheduledAt.HasValue)
            {
                _backgroundJobClient.Schedule(
                    () => ProcessCampaignJobAsync(campaign.CampaignId, createdByAccountId),
                    req.ScheduledAt.Value);
                _logger.LogInformation("Scheduled campaign #{CampaignId} for {ScheduledAt}", campaign.CampaignId, req.ScheduledAt.Value);
            }

            var account = await _context.Accounts.FindAsync(createdByAccountId);
            campaign.CreatedByAccount = account!;
            campaign.CampaignTargets = _context.CampaignTargets.Where(t => t.CampaignId == campaign.CampaignId).ToList();

            return new ApiResponse<CampaignResponse>
            {
                Status = 201,
                StatusMessage = "CREATED",
                Message = "Tạo chiến dịch thành công",
                Data = MapToResponse(campaign, 0, 0, 0)
            };
        }

        public async Task<ApiResponse<CampaignResponse>> UpdateCampaignAsync(int id, UpdateCampaignRequest req)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.CampaignTargets)
                .Include(c => c.CreatedByAccount)
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (campaign == null)
                return new ApiResponse<CampaignResponse> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Chiến dịch không tồn tại" };

            if (campaign.Status != "Draft")
                return new ApiResponse<CampaignResponse> { Status = 400, StatusMessage = "FAILED", Message = "Chỉ có thể chỉnh sửa chiến dịch ở trạng thái Draft" };

            if (req.CampaignName != null) campaign.CampaignName = req.CampaignName;
            if (req.TemplateCode != null) campaign.TemplateCode = req.TemplateCode;
            if (req.TitleOverride != null) campaign.TitleOverride = req.TitleOverride;
            if (req.MessageOverride != null) campaign.MessageOverride = req.MessageOverride;
            if (req.TargetType != null) campaign.TargetType = req.TargetType;
            if (req.ScheduledAt.HasValue) campaign.ScheduledAt = req.ScheduledAt;
            if (req.EventKey != null) campaign.EventKey = req.EventKey;
            if (req.ImageUrl != null) campaign.ImageUrl = req.ImageUrl;
            if (req.ActionType != null) campaign.ActionType = req.ActionType;
            if (req.ActionTarget != null) campaign.ActionTarget = req.ActionTarget;
            campaign.UpdatedAt = DateTime.UtcNow;

            if (req.TargetValues != null)
            {
                _context.CampaignTargets.RemoveRange(campaign.CampaignTargets);
                campaign.CampaignTargets = req.TargetValues.Select(v => new CampaignTarget
                {
                    CampaignId = id,
                    TargetValue = v
                }).ToList();
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<CampaignResponse>
            {
                Status = 200,
                StatusMessage = "OK",
                Message = "Cập nhật chiến dịch thành công",
                Data = MapToResponse(campaign, 0, 0, 0)
            };
        }

        public async Task<ApiResponse<bool>> DeleteCampaignAsync(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return new ApiResponse<bool> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Chiến dịch không tồn tại" };

            if (campaign.Status == "Processing")
                return new ApiResponse<bool> { Status = 400, StatusMessage = "FAILED", Message = "Không thể xóa chiến dịch đang xử lý" };

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Status = 200, StatusMessage = "OK", Message = "Đã xóa chiến dịch", Data = true };
        }

        public async Task<ApiResponse<bool>> SendCampaignAsync(int id, int triggeredByAccountId)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.CampaignTargets)
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (campaign == null)
                return new ApiResponse<bool> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Chiến dịch không tồn tại" };

            if (campaign.Status == "Processing" || campaign.Status == "Completed")
                return new ApiResponse<bool> { Status = 400, StatusMessage = "FAILED", Message = $"Chiến dịch đã ở trạng thái {campaign.Status}" };

            // Build send request from campaign data
            var sendRequest = BuildSendRequest(campaign);

            campaign.Status = "Processing";
            campaign.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            try
            {
                var result = await _notificationCommandService.SendNotificationAsync(sendRequest, triggeredByAccountId, isSystemGenerated: true);

                campaign.Status = result.Status is 200 or 201 or 202 ? "Completed" : "Failed";
                campaign.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Status = 200,
                    StatusMessage = "OK",
                    Message = $"Đã gửi chiến dịch, tạo {result.Data} bản ghi thông báo",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                campaign.Status = "Failed";
                campaign.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogError(ex, "Failed to send campaign #{CampaignId}", id);
                return new ApiResponse<bool> { Status = 500, StatusMessage = "ERROR", Message = "Gửi chiến dịch thất bại: " + ex.Message };
            }
        }

        /// <summary>Hangfire-invocable method for scheduled campaigns</summary>
        public async Task ProcessCampaignJobAsync(int campaignId, int triggeredByAccountId)
        {
            _logger.LogInformation("Processing scheduled campaign #{CampaignId}", campaignId);
            await SendCampaignAsync(campaignId, triggeredByAccountId);
        }

        public async Task<ApiResponse<CampaignResponse>> DuplicateCampaignAsync(int id, int createdByAccountId)
        {
            var original = await _context.Campaigns
                .Include(c => c.CampaignTargets)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CampaignId == id);

            if (original == null)
                return new ApiResponse<CampaignResponse> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Chiến dịch không tồn tại" };

            var cloneRequest = new CreateCampaignRequest
            {
                CampaignName = $"[Copy] {original.CampaignName}",
                TemplateCode = original.TemplateCode,
                TitleOverride = original.TitleOverride,
                MessageOverride = original.MessageOverride,
                SourceType = original.SourceType,
                TargetType = original.TargetType,
                TargetValues = original.CampaignTargets.Select(t => t.TargetValue).ToList(),
                EventKey = original.EventKey,
                ImageUrl = original.ImageUrl,
                ActionType = original.ActionType,
                ActionTarget = original.ActionTarget
            };

            return await CreateCampaignAsync(cloneRequest, createdByAccountId);
        }

        public async Task<ApiResponse<bool>> RecordActionAsync(RecordActionRequest request)
        {
            var delivery = await _context.Deliveries.FindAsync(request.DeliveryId);
            if (delivery == null)
                return new ApiResponse<bool> { Status = 404, StatusMessage = "NOT_FOUND", Message = "Delivery không tồn tại" };

            _context.DeliveryActions.Add(new DeliveryAction
            {
                DeliveryId = request.DeliveryId,
                AccountId = delivery.AccountId,
                ActionType = request.ActionType,
                ActionTarget = request.ActionTarget,
                OccurredAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return new ApiResponse<bool> { Status = 200, StatusMessage = "OK", Data = true };
        }

        // ─────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────

        private static CampaignResponse MapToResponse(Campaign c, int totalSent, int totalRead, int totalClicked)
        {
            return new CampaignResponse
            {
                CampaignId = c.CampaignId,
                CampaignName = c.CampaignName,
                TemplateCode = c.TemplateCode,
                TitleOverride = c.TitleOverride,
                MessageOverride = c.MessageOverride,
                SourceType = c.SourceType,
                TargetType = c.TargetType,
                Status = c.Status,
                ScheduledAt = c.ScheduledAt,
                EventKey = c.EventKey,
                ImageUrl = c.ImageUrl,
                ActionType = c.ActionType,
                ActionTarget = c.ActionTarget,
                CreatedByAccountId = c.CreatedByAccountId,
                CreatedByAccountName = c.CreatedByAccount?.AccountName,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                TotalRecipients = c.CampaignTargets?.Count ?? 0,
                TotalSent = totalSent,
                TotalRead = totalRead,
                TotalClicked = totalClicked,
                TargetValues = c.CampaignTargets?.Select(t => t.TargetValue).ToList() ?? []
            };
        }

        private SendNotificationRequest BuildSendRequest(Campaign campaign)
        {
            // Map TargetType from campaign schema to notification schema
            var notifTargetType = campaign.TargetType switch
            {
                "ALL" => "All",
                "SINGLE" or "CUSTOM" => "User",
                "GROUP" => "Role",
                _ => "All"
            };

            List<int>? targetUserIds = null;
            if (notifTargetType == "User")
            {
                targetUserIds = campaign.CampaignTargets
                    .Where(t => int.TryParse(t.TargetValue, out _))
                    .Select(t => int.Parse(t.TargetValue))
                    .ToList();
            }

            return new SendNotificationRequest
            {
                TemplateCode = campaign.TemplateCode,
                Title = campaign.TitleOverride,
                Message = campaign.MessageOverride,
                ImageUrl = campaign.ImageUrl,
                ActionType = campaign.ActionType,
                ActionTarget = campaign.ActionTarget,
                TargetType = notifTargetType,
                TargetUserIds = targetUserIds,
                CampaignId = campaign.CampaignId
            };
        }
    }
}
