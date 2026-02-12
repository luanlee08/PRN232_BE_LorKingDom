using BLL.DTOs;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/reviews")]
    [ApiController]
    public class AReviewProductsController : ControllerBase
    {
        private readonly IReviewProductService _reviewProductService;

        public AReviewProductsController(IReviewProductService reviewProductService)
        {
            _reviewProductService = reviewProductService;
        }

        /// <summary>
        /// Admin xem danh sách tất cả review với filter nâng cao
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAdminReviews([FromQuery] AdminReviewListQuery query)
        {
            var result = await _reviewProductService.GetAdminReviewsAsync(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Admin cập nhật trạng thái review (Approve/Reject/Pending)
        /// Hoặc thay đổi visibility (Public/AuthorOnly)
        /// </summary>
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] AdminUpdateReviewRequest request)
        {
            var result = await _reviewProductService.AdminUpdateReviewAsync(reviewId, request);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Admin xóa mềm review
        /// </summary>
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> SoftDeleteReview(int reviewId)
        {
            var result = await _reviewProductService.AdminSoftDeleteReviewAsync(reviewId);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Admin trả lời review của customer
        /// </summary>
        [HttpPost("replies")]
        public async Task<IActionResult> AddReply([FromBody] AddReplyRequest request)
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

            var result = await _reviewProductService.AddReplyAsync(request, Convert.ToInt32(accountIdClaim));
            return StatusCode(result.Status, result);
        }
    }
}
