using BLL.DTOs;
using BLL.DTOs.Cart;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/cart")]
    [ApiController]
    public class CCartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CCartController> _logger;

        public CCartController(ICartService cartService, ILogger<CCartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // GET: api/cart
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Account ID claim value: {AccountIdClaim}", accountIdClaim);

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.GetCartAsync(accountId);
            return StatusCode(result.Status, result);
        }

        // POST: api/cart/add
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.AddToCartAsync(request, accountId);
            return StatusCode(result.Status, result);
        }

        // PUT: api/cart/update
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.UpdateCartItemAsync(request, accountId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/cart/increment/{cartItemId}
        [Authorize]
        [HttpPatch("increment/{cartItemId}")]
        public async Task<IActionResult> IncrementCartItem(int cartItemId)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.IncrementCartItemAsync(cartItemId, accountId);
            return StatusCode(result.Status, result);
        }

        // PATCH: api/cart/decrement/{cartItemId}
        [Authorize]
        [HttpPatch("decrement/{cartItemId}")]
        public async Task<IActionResult> DecrementCartItem(int cartItemId)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.DecrementCartItemAsync(cartItemId, accountId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/cart/remove/{cartItemId}
        [Authorize]
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.RemoveCartItemAsync(cartItemId, accountId);
            return StatusCode(result.Status, result);
        }

        // DELETE: api/cart/clear
        [Authorize]
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Không thể xác thực người dùng"
                });
            }

            var result = await _cartService.ClearCartAsync(accountId);
            return StatusCode(result.Status, result);
        }
    }
}
