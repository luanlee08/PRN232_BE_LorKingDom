using BLL.DTOs.Orders;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/refunds")]
    [Authorize(Roles = "Admin,Staff")]
    public class ARefundController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<ARefundController> _logger;

        public ARefundController(IOrderService orderService, ILogger<ARefundController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        private int GetAccountId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetRefunds(
            [FromQuery] string? statusFilter = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetRefundRequestsAsync(pageNumber, pageSize, statusFilter);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{refundId}")]
        public async Task<IActionResult> GetRefundById(long refundId)
        {
            var result = await _orderService.GetRefundByIdAsync(refundId);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{refundId}/process")]
        public async Task<IActionResult> ProcessRefund(long refundId, [FromBody] ProcessRefundByAdminRequest request)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var approveRequest = new ApproveRefundRequest
            {
                RefundId = refundId,
                IsApproved = request.IsApproved,
                Note = request.Note
            };

            var result = await _orderService.ApproveRefundAsync(approveRequest, adminId);
            return StatusCode(result.Status, result);
        }
    }

    public class ProcessRefundByAdminRequest
    {
        public bool IsApproved { get; set; }
        public string? Note { get; set; }
    }
}
