using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Interfaces.Notification;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/notifications")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ANotificationController : ControllerBase
    {
        private readonly INotificationQueryService _queryService;
        private readonly INotificationCommandService _commandService;
        private readonly AspLorKingDomContext _dbContext;
        private readonly ILogger<ANotificationController> _logger;

        public ANotificationController(
            INotificationQueryService queryService,
            INotificationCommandService commandService,
            AspLorKingDomContext dbContext,
            ILogger<ANotificationController> logger)
        {
            _queryService = queryService;
            _commandService = commandService;
            _dbContext = dbContext;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<DeliveryResponse>>>> GetNotifications([FromQuery] DeliveryQuery query)
        {
            var result = await _queryService.GetDeliveriesAsync(query);
            return StatusCode(result.Status, result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DeliveryResponse>>> GetNotificationById(long id)
        {
            var result = await _queryService.GetDeliveryByIdAsync(id);
            return StatusCode(result.Status, result);
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> SendNotification([FromBody] SendNotificationRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int createdBy))
            {
                return Unauthorized(new ApiResponse<int>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không tìm thấy thông tin người dùng"
                });
            }

            var result = await _commandService.SendNotificationAsync(request, createdBy);
            return StatusCode(result.Status, result);
        }


        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<DeliveryStatsResponse>>> GetStats()
        {
            var result = await _queryService.GetStatsAsync();
            return StatusCode(result.Status, result);
        }

        [HttpGet("templates")]
        public async Task<ActionResult<ApiResponse<List<TemplateDto>>>> GetAdminTemplates()
        {
            var templates = await _dbContext.Templates
                .Where(t => t.IsActive)
                .OrderBy(t => t.TemplateCode)
                .Select(t => new TemplateDto
                {
                    TemplateCode = t.TemplateCode,
                    Title = t.TitleTemplate,
                    Message = t.MessageTemplate
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<TemplateDto>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Data = templates
            });
        }


        [HttpGet("users/search")]
        public async Task<ActionResult<ApiResponse<List<AccountSearchResult>>>> SearchUsers(
            [FromQuery] string? q,
            [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);

            var keyword = (q ?? string.Empty).Trim().ToLower();

            var query = _dbContext.Accounts
                .Where(a => !a.IsDeleted && a.Status == "Active");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.AccountName.ToLower().Contains(keyword) ||
                    a.Email.ToLower().Contains(keyword) ||
                    (a.PhoneNumber != null && a.PhoneNumber.Contains(keyword)));
            }

            var results = await query
                .OrderBy(a => a.AccountName)
                .Take(pageSize)
                .Select(a => new AccountSearchResult
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    Image = a.Image
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<AccountSearchResult>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Data = results
            });
        }
    }
}