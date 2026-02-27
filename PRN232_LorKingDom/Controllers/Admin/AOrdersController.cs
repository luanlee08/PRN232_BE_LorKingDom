using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// Get paginated list of orders with filtering and sorting
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQuery query)
        {
            var result = await _orderService.GetOrdersAsync(query);
            return StatusCode(result.Status, result);
        }

        /// Get detailed information about a specific order
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var result = await _orderService.GetOrderDetailAsync(id);
            return StatusCode(result.Status, result);
        }

        /// Update order status (Admin only)
        /// Validates status transitions and prevents updates on Refunded orders
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            //Extract admin ID from JWT claims
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var adminId = Convert.ToInt32(accountIdClaim);
            var result = await _orderService.UpdateOrderStatusAsync(id, request, adminId);
            return StatusCode(result.Status, result);
        }

        /// Export orders to Excel file based on current filters
        /// Maximum 5000 records
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel([FromQuery] OrderQuery query)
        {
            try
            {
                var fileBytes = await _orderService.ExportOrdersToExcelAsync(query);
                var fileName = $"Orders_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
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
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = $"Đã xảy ra lỗi khi export: {ex.Message}"
                });
            }
        }
    }
}
