using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/origins")]
    public class AOriginsController : ControllerBase
    {
        private readonly IOriginService _service;

        public AOriginsController(IOriginService service)
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
