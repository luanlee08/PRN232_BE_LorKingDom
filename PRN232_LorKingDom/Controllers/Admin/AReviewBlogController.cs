using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/reviews-blog")]
    public class AdminReviewBlogController : ControllerBase
    {
        private readonly IReviewBlogService _service;

        public AdminReviewBlogController(
            IReviewBlogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return StatusCode(result.Status, result);
        }
        [HttpPut("{id}/block")]
        public async Task<IActionResult> Block(
            int id,
            [FromQuery] bool isBlocked)
        {
            var result = await _service
                .BlockAsync(id, isBlocked);

            return StatusCode(result.Status, result);
        }
    }
}
