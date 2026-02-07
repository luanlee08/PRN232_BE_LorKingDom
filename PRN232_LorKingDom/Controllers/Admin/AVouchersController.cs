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

        /// <summary>
        /// Get list of vouchers with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetVouchers([FromQuery] VoucherSearchRequest request)
        {
            var result = await _voucherService.GetVouchersAsync(request);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Create a new voucher
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {

            var result = await _voucherService.CreateVoucherAsync(request);
            return StatusCode(result.Status, result);

        }

        /// <summary>
        /// Update an existing voucher
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<VoucherResponse>> UpdateVoucher(int id, [FromBody] UpdateVoucherRequest updateDTO)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, updateDTO);
            return StatusCode(result.Status, result);
        }
    }
}
