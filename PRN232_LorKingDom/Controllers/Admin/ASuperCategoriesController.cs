using BLL.DTOs.SuperCategories;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/super-categories")]
    public class ASuperCategoriesController : ControllerBase
    {
        private readonly ISuperCategoryService _service;

        public ASuperCategoriesController(ISuperCategoryService service)
        {
            _service = service;
        }

       
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SuperCategoryQuery query)
        {
            var result = await _service.GetAsync(query);
            return StatusCode(result.Status, result);
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSuperCategoryRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSuperCategoryRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _service.GetActiveAsync();
            return StatusCode(result.Status, result);
        }
    }
}
