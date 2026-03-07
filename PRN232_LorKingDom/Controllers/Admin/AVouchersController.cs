using BLL.DTOs.Vouchers;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/vouchers")]
    [ApiController]
    public class AVouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public AVouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVouchers([FromQuery] VoucherQuery request)
        {
            var result = await _voucherService.GetVouchersAsync(request);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {

            var result = await _voucherService.CreateVoucherAsync(request);
            return StatusCode(result.Status, result);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, [FromBody] UpdateVoucherRequest request)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetVoucherTypes()
        {
            var result = await _voucherService.GetVoucherTypesAsync();
            return StatusCode(result.Status, result);
        }
    }
}
