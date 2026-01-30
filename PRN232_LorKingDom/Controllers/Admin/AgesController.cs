using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/ages")]
    public class AAgesController : ControllerBase
    {
        private readonly IAgeService _service;

        public AAgesController(IAgeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return StatusCode(result.Status, result);
        }
    }
}
