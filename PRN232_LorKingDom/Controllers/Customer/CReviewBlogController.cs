using BLL.DTOs;
using BLL.DTOs.ReviewBlog;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/blog-reviews")]
    public class CReviewBlogController : ControllerBase
    {
        private readonly IReviewBlogService _service;

        public CReviewBlogController(IReviewBlogService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReviewBlogRequest request)
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
                .CreateAsync(accountId, request);

            return StatusCode(result.Status, result);
        }

        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult>
            GetByBlog(int blogId)
        {
            var result = await _service
                .GetByBlogIdAsync(blogId);

            return StatusCode(result.Status, result);
        }
    }
}
