using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/blog-categories")]
    public class ABlogCategoriesController : ControllerBase
    {
        private readonly IBlogCategoryService _service;

        public ABlogCategoriesController(IBlogCategoryService service)
        {
            _service = service;
        }

        // GET: /api/blog-categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
