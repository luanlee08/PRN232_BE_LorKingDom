using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/price-ranges")]
    public class APriceRangesController : ControllerBase
    {
        private readonly IPriceRangeService _service;

        public APriceRangesController(IPriceRangeService service)
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
