using BLL.DTOs.Brands;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/brands")]
    public class ABrandsController : ControllerBase
    {
        private readonly IBrandService _service;

        public ABrandsController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BrandQuery query)
        {
            var result = await _service.GetAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateBrandRequest request)
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
