using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Interfaces.Order;
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
        private readonly IOrderRefundService _refundService;
        private readonly ILogger<ARefundController> _logger;

        public ARefundController(IOrderRefundService refundService, ILogger<ARefundController> logger)
        {
            _refundService = refundService;
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
            try
            {
                var result = await _refundService.GetRefundRequestsPagedAsync(pageNumber, pageSize, statusFilter);
                return Ok(new ApiResponse<PagedResult<OrderRefundDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách yêu cầu hoàn tiền thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund requests");
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [HttpGet("{refundId}")]
        public async Task<IActionResult> GetRefundById(long refundId)
        {
            try
            {
                var result = await _refundService.GetRefundByIdAsync((int)refundId);
                return Ok(new ApiResponse<OrderRefundDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy thông tin hoàn tiền thành công",
                    Data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy yêu cầu hoàn tiền"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund {RefundId}", refundId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [HttpPut("{refundId}/process")]
        public async Task<IActionResult> ProcessRefund(long refundId, [FromBody] ProcessRefundByAdminRequest request)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                var processRequest = new ProcessRefundRequest
                {
                    IsApproved = request.IsApproved,
                    ApprovedBy = adminId,
                    Note = request.Note
                };

                var result = await _refundService.ProcessRefundAsync((int)refundId, processRequest);
                return Ok(new ApiResponse<OrderRefundDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = request.IsApproved
                        ? "Duyệt và xử lý hoàn tiền thành công"
                        : "Đã từ chối yêu cầu hoàn tiền",
                    Data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy yêu cầu hoàn tiền"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund {RefundId}", refundId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }
    }

    public class ProcessRefundByAdminRequest
    {
        public bool IsApproved { get; set; }
        public string? Note { get; set; }
    }
}
