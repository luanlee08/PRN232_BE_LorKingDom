using BLL.DTOs;
using BLL.DTOs.Campaigns;
using BLL.DTOs.Notifications;
using BLL.Interfaces;
using BLL.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/notifications")]
    [ApiController]
    public class CNotificationController : ControllerBase
    {
        private readonly INotificationQueryService _queryService;
        private readonly INotificationCommandService _commandService;
        private readonly ICampaignService _campaignService;
        private readonly ILogger<CNotificationController> _logger;

        public CNotificationController(
            INotificationQueryService queryService,
            INotificationCommandService commandService,
            ICampaignService campaignService,
            ILogger<CNotificationController> logger)
        {
            _queryService = queryService;
            _commandService = commandService;
            _campaignService = campaignService;
            _logger = logger;
        }

        private int? GetAccountId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out int accountId))
                return null;
            return accountId;
        }

        private ActionResult UnauthorizedResponse()
        {
            return Unauthorized(new ApiResponse<object>
            {
                Status = 401,
                StatusMessage = "UNAUTHORIZED",
                Message = "Không thể xác thực người dùng"
            });
        }

        /// <summary>
        /// Get notifications for the current user with filtering and pagination
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] UserNotificationQuery query)
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _queryService.GetUserNotificationsAsync(accountId.Value, query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get unread notification count for the current user
        /// </summary>
        [Authorize]
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _queryService.GetUnreadCountAsync(accountId.Value);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [Authorize]
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _commandService.MarkAsReadAsync(id, accountId.Value);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Mark all notifications as read for the current user
        /// </summary>
        [Authorize]
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _commandService.MarkAllAsReadAsync(accountId.Value);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(long id)
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _commandService.DeleteDeliveryAsync(id, accountId.Value);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Record a click or read action for campaign analytics.
        /// Called from FE when user clicks a notification action link.
        /// </summary>
        [Authorize]
        [HttpPost("action")]
        public async Task<IActionResult> RecordAction([FromBody] RecordActionRequest request)
        {
            var result = await _campaignService.RecordActionAsync(request);
            return StatusCode(result.Status, result);
        }
    }
}
