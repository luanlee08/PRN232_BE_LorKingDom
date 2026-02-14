using BLL.DTOs.Orders;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(accountIdClaim, out var accountId) ? accountId : 0;
        }

        #region Payment Methods

        /// <summary>
        /// Get available payment methods with details
        /// </summary>
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _orderService.GetAvailablePaymentMethodsAsync();
            return StatusCode(result.Status, result);
        }

        #endregion

        #region Customer Endpoints

        /// <summary>
        /// Create new order from cart
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.CreateOrderAsync(request, accountId);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get order by ID (customer can only view their own orders)
        /// </summary>
        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.GetOrderByIdAsync(orderId, accountId);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get my orders with pagination and filter
        /// </summary>
        [Authorize]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.GetMyOrdersAsync(accountId, pageNumber, pageSize, status);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Cancel order (only Pending/Processing orders)
        /// </summary>
        [Authorize]
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelOrderRequest? request)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.CancelOrderAsync(orderId, accountId, request?.Reason);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Create refund request for delivered order
        /// </summary>
        [Authorize]
        [HttpPost("refund/request")]
        public async Task<IActionResult> CreateRefundRequest([FromBody] CreateRefundRequest request)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.CreateRefundRequestAsync(request, accountId);
            return StatusCode(result.Status, result);
        }

        #endregion

        #region Admin Endpoints

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
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

        /// <summary>
        /// Update order status (Admin only)
        /// </summary>
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

        /// <summary>
        /// Get all refund requests (Admin only)
        /// </summary>
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

        /// <summary>
        /// Get refund by ID (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("admin/refunds/{refundId}")]
        public async Task<IActionResult> GetRefundById(long refundId)
        {
            var result = await _orderService.GetRefundByIdAsync(refundId);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Approve or reject refund request (Admin only)
        /// </summary>
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

        #endregion

        #region Payment & Shipping Endpoints

        /// <summary>
        /// Handle payment webhook from payment gateway (VNPay, MoMo, etc.)
        /// </summary>
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

        /// <summary>
        /// Confirm COD payment (Shipper confirms cash collected)
        /// </summary>
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

        #endregion
    }

    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
    }
}
