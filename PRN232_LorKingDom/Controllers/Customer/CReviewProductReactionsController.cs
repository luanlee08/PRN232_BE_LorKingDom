using BLL.DTOs;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Route("api/reviews/reactions")]
    [ApiController]
    public class CReviewProductReactionsController : ControllerBase
    {
        private readonly IReviewReactionService _reactionProductService;

        public CReviewProductReactionsController(IReviewReactionService reactionProductService)
        {
            _reactionProductService = reactionProductService;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionRequest request)
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

            var result = await _reactionProductService.ToggleReactionAsync(request, Convert.ToInt32(accountIdClaim));
            return StatusCode(result.Status, result);
        }
    }
}
