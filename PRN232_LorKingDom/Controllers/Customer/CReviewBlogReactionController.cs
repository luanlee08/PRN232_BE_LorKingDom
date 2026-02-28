using BLL.DTOs;
using BLL.DTOs.ReviewBlogReaction;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/blog-review-reactions")]
    public class CReviewBlogReactionController : ControllerBase
    {
        private readonly IReviewBlogReactionService _service;

        public CReviewBlogReactionController(
            IReviewBlogReactionService service)
        {
            _service = service;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> React(
            [FromBody] CreateReviewBlogReactionRequest request)
        {
            var accountIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (!int.TryParse(accountIdClaim?.Value, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Token không hợp lệ"
                });
            }

            var result = await _service
                .ReactAsync(accountId, request);

            return StatusCode(result.Status, result);
        }
        [Authorize]
        [HttpDelete("{reviewBlogId}")]
        public async Task<IActionResult> Remove(int reviewBlogId)
        {
            var accountIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (!int.TryParse(accountIdClaim?.Value, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Token không hợp lệ"
                });
            }

            var result = await _service
                .RemoveAsync(accountId, reviewBlogId);

            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpGet("{reviewBlogId}")]
        public async Task<IActionResult> GetSummary(int reviewBlogId)
        {
            var accountIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            int? accountId = null;

            if (int.TryParse(accountIdClaim?.Value, out var id))
            {
                accountId = id;
            }

            var result = await _service
                .GetSummaryAsync(accountId, reviewBlogId);

            return StatusCode(result.Status, result);
        }
    }
}
