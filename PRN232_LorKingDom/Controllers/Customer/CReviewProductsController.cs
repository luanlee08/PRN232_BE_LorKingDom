using BLL.DTOs;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/reviews")]
    [ApiController]
    public class CReviewProductsController : ControllerBase
    {
        private readonly IReviewProductService _reviewProductService;

        public CReviewProductsController(IReviewProductService reviewProductService)
        {
            _reviewProductService = reviewProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewListQuery query)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? accountId = null;
            if (!string.IsNullOrEmpty(accountIdClaim) && int.TryParse(accountIdClaim, out int parsedId))
            {
                accountId = parsedId;
            }
            var result = await _reviewProductService.GetReviewsAsync(query, accountId);
            return StatusCode(result.Status, result);
        }

        /// Lấy thống kê review của sản phẩm 
        [HttpGet("summary/{productId}")]
        public async Task<IActionResult> GetReviewSummary(int productId)
        {
            var result = await _reviewProductService.GetReviewSummaryAsync(productId);
            return StatusCode(result.Status, result);
        }

        /// Lấy lịch sử review của customer trên cùng sản phẩm (bao gồm review cũ)
        [HttpGet("history")]
        public async Task<IActionResult> GetMyReviewHistory([FromQuery] int productId)
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

            var result = await _reviewProductService.GetMyReviewHistoryAsync(productId, Convert.ToInt32(accountIdClaim));
            return StatusCode(result.Status, result);
        }

        // Customer tạo review cho sản phẩm đã mua
        /// **Quy tắc:**
        /// - Phải đăng nhập
        /// - Phải mua sản phẩm (OrderDetail hợp lệ)
        /// - Mỗi OrderDetail chỉ review 1 lần
        /// - Review sẽ chạy qua 3 tầng moderation
        /// - Chỉ hiển thị public khi Approved
        [HttpPost]
        public async Task<IActionResult> AddReview([FromForm] AddReviewRequest request)
        {
            //var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(accountIdClaim))
            //{
            //    return Unauthorized(new ApiResponse<object>
            //    {
            //        Status = 401,
            //        StatusMessage = "UNAUTHORIZED",
            //        Message = "Không thể xác thực người dùng"
            //    });
            //}

            var result = await _reviewProductService.AddReviewAsync(request, 4);
            return StatusCode(result.Status, result);
        }

        // Customer chỉnh sửa review của mình
        /// **Quy tắc:**
        /// - Chỉ được sửa 1 lần duy nhất
        /// - Chỉ trong 3 ngày đầu từ lúc tạo
        /// - Phải chạy lại 3 tầng moderation
        /// - Nếu bị Reject sau khi sửa → không hiển thị public
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> EditReview(int reviewId, [FromForm] EditReviewRequest request)
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

            var result = await _reviewProductService.EditReviewAsync(reviewId, request, Convert.ToInt32(accountIdClaim));
            return StatusCode(result.Status, result);
        }
    }
}
