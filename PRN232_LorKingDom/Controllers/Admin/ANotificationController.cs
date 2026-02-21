using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<ANotificationController> _logger;

        public ANotificationController(
            INotificationQueryService queryService,
            INotificationCommandService commandService,
            ILogger<ANotificationController> logger)
        {
            _queryService = queryService;
            _commandService = commandService;
            _logger = logger;
        }

        /// <summary>
        /// Get all delivered notifications with filtering and pagination (Admin view)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<DeliveryResponse>>>> GetNotifications([FromQuery] DeliveryQuery query)
        {
            var result = await _queryService.GetDeliveriesAsync(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get delivery by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DeliveryResponse>>> GetNotificationById(long id)
        {
            var result = await _queryService.GetDeliveryByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Send notification (creates delivery records for target users)
        /// </summary>
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

        /// <summary>
        /// Get notification delivery statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<DeliveryStatsResponse>>> GetStats()
        {
            var result = await _queryService.GetStatsAsync();
            return StatusCode(result.Status, result);
        }
    }
}