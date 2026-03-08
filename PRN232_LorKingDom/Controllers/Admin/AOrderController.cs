using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.DTOs.Shipping;
using BLL.Interfaces;
using BLL.Interfaces.Order;
using BLL.Worker;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    public class AOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;  // Retained for CreateShippingOrderAsync only
        private readonly IOrderQueryService _queryService;
        private readonly IOrderCommandService _commandService;
        private readonly IOrderRefundService _refundService;
        private readonly IOrderWebhookService _webhookService;
        private readonly IGHNShippingStatusService _ghnShippingStatusService;
        private readonly ShippingStatusSyncWorker _shippingSyncWorker;
        private readonly AspLorKingDomContext _context;

        public AOrderController(
            IOrderService orderService,
            IOrderQueryService queryService,
            IOrderCommandService commandService,
            IOrderRefundService refundService,
            IOrderWebhookService webhookService,
            IGHNShippingStatusService ghnShippingStatusService,
            ShippingStatusSyncWorker shippingSyncWorker,
            AspLorKingDomContext context)
        {
            _orderService = orderService;
            _queryService = queryService;
            _commandService = commandService;
            _refundService = refundService;
            _webhookService = webhookService;
            _ghnShippingStatusService = ghnShippingStatusService;
            _shippingSyncWorker = shippingSyncWorker;
            _context = context;
        }

        private int GetAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(accountIdClaim, out var accountId) ? accountId : 0;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            try
            {
                var result = await _queryService.GetAllOrdersAsync(pageNumber, pageSize, status);
                return Ok(new ApiResponse<PagedResult<OrderDto>>
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
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        // TODO: Migrate CreateShippingOrderAsync to IOrderCommandService to remove IOrderService dependency
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin/{orderId}/shipping")]
        public async Task<IActionResult> CreateShippingOrder(
            int orderId,
            [FromBody] CreateShippingOrderRequest request)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            request.OrderId = orderId;

            var result = await _orderService.CreateShippingOrderAsync(request, adminId);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("admin/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusRequest request)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                request.ChangedBy = adminId;
                await _commandService.UpdateOrderStatusAsync(orderId, request);

                var updatedOrder = await _queryService.GetOrderByIdAsync(orderId);
                return Ok(new ApiResponse<OrderDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Cập nhật trạng thái đơn hàng thành công",
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

        [AllowAnonymous]
        [HttpPost("webhook/shipping/ghn")]
        public async Task<IActionResult> HandleGHNWebhook([FromBody] GHNWebhookRequest webhookData)
        {
            try
            {
                if (webhookData?.Data == null || string.IsNullOrEmpty(webhookData.Data.OrderCode))
                    return BadRequest(new { message = "Invalid GHN webhook payload" });

                var rawPayload = JsonSerializer.Serialize(webhookData);
                var result = await _ghnShippingStatusService.ProcessStatusUpdateAsync(
                    providerOrderCode: webhookData.Data.OrderCode,
                    newGHNStatus: webhookData.Data.Status,
                    source: "Webhook",
                    rawPayload: rawPayload);

                if (!result.Success)
                    return StatusCode(422, new { message = result.Message, data = result });

                return Ok(new
                {
                    status = 200,
                    message = result.StatusUpdated ? "Status updated" : "Status unchanged",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Webhook processing failed", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin/shipping/{shippingId}/refresh")]
        public async Task<IActionResult> RefreshShippingStatusById(long shippingId)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                var result = await _shippingSyncWorker.SyncShippingByIdAsync(shippingId);

                if (!result.Success)
                {
                    return StatusCode(400, new
                    {
                        status = 400,
                        message = result.Message,
                        data = result
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = result.StatusUpdated
                        ? "Đã cập nhật trạng thái từ GHN"
                        : "Trạng thái chưa thay đổi",
                    data = new
                    {
                        shippingId,
                        oldStatus = result.OldStatus,
                        newStatus = result.NewStatus,
                        statusText = result.StatusText,
                        statusUpdated = result.StatusUpdated,
                        syncedAt = result.SyncedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Có lỗi xảy ra khi làm mới trạng thái",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin/{orderId}/shipping/refresh")]
        public async Task<IActionResult> RefreshShippingStatusByOrderId(int orderId)
        {
            var adminId = GetAccountId();
            if (adminId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                var result = await _shippingSyncWorker.SyncShippingByOrderIdAsync(orderId);

                if (!result.Success)
                {
                    return StatusCode(400, new
                    {
                        status = 400,
                        message = result.Message,
                        data = result
                    });
                }

                return Ok(new
                {
                    status = 200,
                    message = result.StatusUpdated
                        ? "Đã cập nhật trạng thái từ GHN"
                        : "Trạng thái chưa thay đổi",
                    data = new
                    {
                        orderId,
                        oldStatus = result.OldStatus,
                        newStatus = result.NewStatus,
                        statusText = result.StatusText,
                        statusUpdated = result.StatusUpdated,
                        syncedAt = result.SyncedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = 500,
                    message = "Có lỗi xảy ra khi làm mới trạng thái",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// [DEMO ONLY] Tạo một vận đơn GHN giả lập cho đơn hàng để test UI theo dõi vận chuyển.
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin/demo/seed-shipping/{orderId}")]
        public async Task<IActionResult> SeedDemoShipping(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.ShippingProviderTransactions)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.IsDeleted);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng" });

            if (order.ShippingProviderTransactions.Any(s => s.Provider == "GHN"))
                return BadRequest(new { message = "Đơn hàng đã có vận đơn GHN. Xóa trước rồi seed lại." });

            var fakeCode = $"DEMO{orderId:D6}{DateTime.UtcNow:HHmmss}";
            var shipping = new ShippingProviderTransaction
            {
                OrderId = orderId,
                Provider = "GHN",
                ProviderOrderCode = fakeCode,
                TrackingNumber = fakeCode,
                ServiceType = "Demo",
                Status = "ready_to_pick",
                ShippingFee = 35000,
                EstimatedDelivery = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow
            };

            _context.ShippingProviderTransactions.Add(shipping);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                message = "Tạo vận đơn demo thành công",
                data = new { orderId, trackingNumber = fakeCode, status = "ready_to_pick" }
            });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/refunds")]
        public async Task<IActionResult> GetRefundRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            try
            {
                var result = await _refundService.GetRefundRequestsPagedAsync(pageNumber, pageSize, status);
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
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/refunds/{refundId}")]
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
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("admin/refunds/approve")]
        public async Task<IActionResult> ApproveRefund([FromBody] ApproveRefundRequest request)
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

                var result = await _refundService.ProcessRefundAsync((int)request.RefundId, processRequest);
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
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook/payment/{provider}")]
        public async Task<IActionResult> HandlePaymentWebhook(
            string provider,
            [FromBody] object payload)
        {
            try
            {
                var payloadJson = JsonSerializer.Serialize(payload);
                var webhookData = JsonSerializer.Deserialize<Dictionary<string, string>>(payloadJson)
                    ?? new Dictionary<string, string>();

                var result = await _webhookService.HandlePaymentWebhookAsync(provider, webhookData);
                return Ok(new ApiResponse<object>
                {
                    Status = result.Success ? 200 : 400,
                    StatusMessage = result.Success ? "SUCCESS" : "FAILED",
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Error processing webhook: " + ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Staff,Shipper")]
        [HttpPost("{orderId}/cod-confirm")]
        public async Task<IActionResult> ConfirmCODPayment(int orderId)
        {
            var shipperId = GetAccountId();
            if (shipperId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            try
            {
                await _commandService.ConfirmCODPaymentAsync(orderId);
                return Ok(new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Xác nhận thanh toán COD thành công"
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
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }
    }
}