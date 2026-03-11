using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.DTOs.Shipping;
using BLL.Interfaces;
using BLL.Interfaces.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AOrdersController : ControllerBase
    {
        private readonly IOrderQueryService _queryService;
        private readonly IOrderCommandService _commandService;
        private readonly IOrderExportService _exportService;
        private readonly IOrderService _orderService;

        public AOrdersController(
            IOrderQueryService queryService,
            IOrderCommandService commandService,
            IOrderExportService exportService,
            IOrderService orderService)
        {
            _queryService = queryService;
            _commandService = commandService;
            _exportService = exportService;
            _orderService = orderService;
        }

        /// Get paginated list of orders with filtering and sorting
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQuery query)
        {
            try
            {
                var result = await _queryService.GetAdminOrdersPagedAsync(query);
                return Ok(new ApiResponse<PagedResult<OrderResponse>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách đơn hàng thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Có lỗi xảy ra khi lấy danh sách đơn hàng"
                });
            }
        }

        /// Get detailed information about a specific order
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            try
            {
                var result = await _queryService.GetAdminOrderDetailAsync(id);
                return Ok(new ApiResponse<OrderDetailResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy chi tiết đơn hàng thành công",
                    Data = result
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy đơn hàng"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Có lỗi xảy ra khi lấy chi tiết đơn hàng"
                });
            }
        }

        /// Update order status (Admin only)
        /// Validates status transitions and prevents updates on Refunded orders
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
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

            try
            {
                var adminId = Convert.ToInt32(accountIdClaim);
                request.ChangedBy = adminId;
                await _commandService.UpdateOrderStatusAsync(id, request);

                // Auto-create GHN shipping order when transitioning to Shipped (statusId=3)
                string responseMessage = "Cập nhật trạng thái đơn hàng thành công";
                if (request.AutoCreateShipping && request.StatusId == 3)
                {
                    var shippingRequest = new CreateShippingOrderRequest
                    {
                        OrderId = id,
                        Provider = "GHN",
                        ServiceId = request.ShippingServiceId ?? 53321,
                        ServiceTypeId = 2,
                        Note = request.ShippingNote ?? "Đơn hàng từ LorKingdom",
                        RequiredNote = request.ShippingRequiredNote ?? "KHONGCHOXEMHANG"
                    };
                    var shippingResult = await _orderService.CreateShippingOrderAsync(shippingRequest, adminId);
                    responseMessage += shippingResult.Status == 200
                        ? $" · GHN: {shippingResult.Data?.OrderCode}"
                        : $" ⚠️ Tạo vận đơn GHN thất bại: {shippingResult.Message}";
                }

                var updatedOrder = await _queryService.GetOrderByIdAsync(id);
                return Ok(new ApiResponse<OrderDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = responseMessage,
                    Data = updatedOrder
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy đơn hàng"
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
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        /// Export orders to Excel file based on current filters
        /// Maximum 5000 records
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel([FromQuery] OrderQuery query)
        {
            try
            {
                var fileBytes = await _exportService.ExportOrdersToExcelAsync(query);
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
