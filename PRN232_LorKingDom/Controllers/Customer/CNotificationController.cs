using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/notifications")]
    [ApiController]
    public class CNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<CNotificationController> _logger;

        public CNotificationController(
            INotificationService notificationService,
            ILogger<CNotificationController> logger)
        {
            _notificationService = notificationService;
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
        /// Get notifications for the current user
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] string? status = null,
            [FromQuery] int limit = 50)
        {
            var accountId = GetAccountId();
            if (accountId == null) return UnauthorizedResponse();

            var result = await _notificationService.GetUserNotificationsAsync(accountId.Value, status, limit);
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

            var result = await _notificationService.GetUnreadCountAsync(accountId.Value);
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

            var result = await _notificationService.MarkAsReadAsync(id, accountId.Value);
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

            var result = await _notificationService.MarkAllAsReadAsync(accountId.Value);
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

            var result = await _notificationService.DeleteDeliveryAsync(id, accountId.Value);
            return StatusCode(result.Status, result);
        }
    }
}
