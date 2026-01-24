using BLL.DTOs.Categories;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/categories")]
    public class ACategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public ACategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CategoryQuery query)
        {
            var result = await _service.GetAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCategoryRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive(
            [FromQuery] int? superCategoryId)
        {
            var result = await _service.GetActiveAsync(superCategoryId);
            return StatusCode(result.Status, result);
        }
    }
}
