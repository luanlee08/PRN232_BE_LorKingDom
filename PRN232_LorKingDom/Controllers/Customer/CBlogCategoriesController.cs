using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/blog-categories")]
    public class CBlogCategoriesController : ControllerBase
    {
        private readonly IBlogCategoryService _service;

        public CBlogCategoriesController(IBlogCategoryService service)
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
