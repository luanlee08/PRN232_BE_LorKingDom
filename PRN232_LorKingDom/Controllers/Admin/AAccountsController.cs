using BLL.DTOs.Accounts;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/accounts")]
    public class AAccountsController : ControllerBase
    {
        private readonly IAccountService _service;

        public AAccountsController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AccountQuery query)
        {
            var result = await _service.GetAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAccountRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }
    }
}
