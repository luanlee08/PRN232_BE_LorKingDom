using BLL.DTOs.Materials;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/materials")]
    public class AMaterialsController : ControllerBase
    {
        private readonly IMaterialService _service;

        public AMaterialsController(IMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] MaterialQuery query)
        {
            var result = await _service.GetAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMaterialRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateMaterialRequest request)
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
