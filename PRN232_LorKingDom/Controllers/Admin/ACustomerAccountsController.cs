using BLL.DTOs.Accounts;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/customer-accounts")]
    public class ACustomerAccountsController : ControllerBase
    {
        private readonly ICustomerAccountService _service;

        public ACustomerAccountsController(ICustomerAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CustomerAccountQuery query)
        {
            var result = await _service.GetCustomersAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetCustomerByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCustomerAccountRequest request)
        {
            var result = await _service.UpdateCustomerAsync(id, request);
            return StatusCode(result.Status, result);
        }
    }
}
