using BLL.DTOs;
using BLL.DTOs.Blog;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/blogs")]
    public class AdminBlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public AdminBlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        /* ================= SEARCH ================= */

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] SearchBlogAdminRequest request)
        {
            var result = await _blogService.SearchForAdminAsync(request);
            return StatusCode(result.Status, result);
        }

        /* ================= CREATE ================= */

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
       [FromForm] CreateBlogRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (!int.TryParse(accountIdClaim?.Value, out var accountId))
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = 401,
                    StatusMessage = "UNAUTHORIZED",
                    Message = "Token không hợp lệ"
                });
            }

            var result = await _blogService.CreateAsync(request, accountId);

            return StatusCode(result.Status, result);
        }

        /* ================= UPDATE ================= */

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateBlogRequest request)
        {
            var result = await _blogService
                .UpdateAsync(id, request);

            return StatusCode(result.Status, result);
        }
    }
}