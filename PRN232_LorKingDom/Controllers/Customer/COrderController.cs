using BLL.DTOs.Orders;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class COrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IGHNService _ghnService;
        private readonly AspLorKingDomContext _context;

        public COrderController(IOrderService orderService, IGHNService ghnService, AspLorKingDomContext context)
        {
            _orderService = orderService;
            _ghnService = ghnService;
            _context = context;
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

        /// <summary>
        /// Get GHN tracking detail (status log) for an order the current user owns
        /// </summary>
        [Authorize]
        [HttpGet("{orderId}/tracking")]
        public async Task<IActionResult> GetOrderTracking(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == 0)
                return Unauthorized(new { message = "Unauthorized" });

            var shipping = await _context.ShippingProviderTransactions
                .Where(s => s.OrderId == orderId && s.Provider == "GHN")
                .Join(_context.Orders, s => s.OrderId, o => o.OrderId, (s, o) => new { s, o })
                .Where(x => x.o.AccountId == accountId && !x.o.IsDeleted)
                .Select(x => x.s)
                .FirstOrDefaultAsync();

            if (shipping == null)
                return NotFound(new { message = "Không tìm thấy thông tin vận chuyển cho đơn hàng này" });

            if (string.IsNullOrEmpty(shipping.TrackingNumber))
                return NotFound(new { message = "Đơn hàng chưa có mã vận đơn GHN" });

            var tracking = await _ghnService.GetOrderTrackingAsync(shipping.TrackingNumber);
            if (tracking == null)
                return StatusCode(502, new { message = "Không thể lấy thông tin từ GHN" });

            return Ok(new { status = 200, message = "Success", data = tracking });
        }

    }
}