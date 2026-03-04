using BLL.DTOs.Orders;
using BLL.DTOs.Shipping;
using BLL.Interfaces;
using BLL.Worker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]")]
    public class AOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ShippingStatusSyncWorker _shippingSyncWorker;

        public AOrderController(IOrderService orderService, ShippingStatusSyncWorker shippingSyncWorker)
        {
            _orderService = orderService;
            _shippingSyncWorker = shippingSyncWorker;
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
            var result = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, status);
            return StatusCode(result.Status, result);
        }


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

            // Override orderId from route
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

            var result = await _orderService.UpdateOrderStatusAsync(orderId, request, adminId);
            return StatusCode(result.Status, result);
        }


        [AllowAnonymous]
        [HttpPost("webhook/shipping/ghn")]
        public async Task<IActionResult> HandleGHNWebhook([FromBody] GHNWebhookRequest webhookData)
        {
            try
            {


                var result = await _orderService.HandleShippingWebhookAsync("GHN", webhookData);
                return StatusCode(result.Status, result);
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


        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/refunds")]
        public async Task<IActionResult> GetRefundRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            var result = await _orderService.GetRefundRequestsAsync(pageNumber, pageSize, status);
            return StatusCode(result.Status, result);
        }


        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/refunds/{refundId}")]
        public async Task<IActionResult> GetRefundById(long refundId)
        {
            var result = await _orderService.GetRefundByIdAsync(refundId);
            return StatusCode(result.Status, result);
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

            var result = await _orderService.ApproveRefundAsync(request, adminId);
            return StatusCode(result.Status, result);
        }


        [AllowAnonymous]
        [HttpPost("webhook/payment/{provider}")]
        public async Task<IActionResult> HandlePaymentWebhook(
            string provider,
            [FromBody] object payload)
        {
            var signature = Request.Headers["X-Signature"].FirstOrDefault() ?? "";
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);

            var result = await _orderService.HandlePaymentWebhookAsync(provider, payloadJson, signature);
            return StatusCode(result.Status, result);
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

            var result = await _orderService.ConfirmCODPaymentAsync(orderId, shipperId);
            return StatusCode(result.Status, result);
        }

    }


}