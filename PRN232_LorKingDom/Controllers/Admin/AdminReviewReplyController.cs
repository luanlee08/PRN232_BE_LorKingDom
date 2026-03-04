using BLL.DTOs.ReviewBlogReply;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/review-replies")]
    public class AdminReviewReplyController : ControllerBase
    {
        private readonly IReviewBlogReplyService _service;

        public AdminReviewReplyController(
            IReviewBlogReplyService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReviewBlogReplyRequest request)
        {
            var accountIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (!int.TryParse(accountIdClaim?.Value, out var accountId))
            {
                return Unauthorized();
            }

            var result = await _service
                .CreateAsync(accountId, request);

            return StatusCode(result.Status, result);
        }
    }
}
