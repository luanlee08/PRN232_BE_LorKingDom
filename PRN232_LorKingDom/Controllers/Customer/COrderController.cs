using BLL.DTOs.Orders;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class COrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public COrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        private int GetAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(accountIdClaim, out var accountId) ? accountId : 0;
        }
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var result = await _orderService.GetAvailablePaymentMethodsAsync();
            return StatusCode(result.Status, result);
        }

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

        /// <summary>
        /// Get my refund requests
        /// </summary>
        [Authorize]
        [HttpGet("refund/my-refunds")]
        public async Task<IActionResult> GetMyRefunds(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.GetMyRefundsAsync(accountId, pageNumber, pageSize);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get refund by ID
        /// </summary>
        [Authorize]
        [HttpGet("refund/{refundId}")]
        public async Task<IActionResult> GetRefundById(long refundId)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var result = await _orderService.GetRefundByIdAsync(refundId);
            return StatusCode(result.Status, result);
        }

    }
}