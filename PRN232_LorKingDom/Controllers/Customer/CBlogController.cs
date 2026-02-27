using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/blogs")]
    public class CBlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public CBlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        /* ================= PUBLIC LIST ================= */

        [HttpGet]
        public async Task<IActionResult> GetPublicBlogs(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _blogService.GetPublicAsync(
                keyword,
                categoryId,
                page,
                pageSize
            );

            return StatusCode(result.Status, result);
        }

        /* ================= DETAIL ================= */

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogDetail(int id)
        {
            var result = await _blogService.GetPublicDetailAsync(id);
            return StatusCode(result.Status, result);
        }

        /* ================= RECENT ================= */

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentBlogs(
            [FromQuery] int limit = 5)
        {
            var result = await _blogService.GetRecentAsync(limit);
            return StatusCode(result.Status, result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedBlogs(
        [FromQuery] int limit = 5)
        {
            var result = await _blogService.GetFeaturedAsync(limit);
            return StatusCode(result.Status, result);
        }
    }
}