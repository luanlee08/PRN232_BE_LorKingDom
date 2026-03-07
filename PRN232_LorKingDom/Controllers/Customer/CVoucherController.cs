using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/customer/vouchers")]
    public class CVoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public CVoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        /// <summary>
        /// Validate a voucher code and compute the discount for the given order amount.
        /// No authentication required — customers can validate before login.
        /// </summary>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateVoucher(
            [FromQuery] string code,
            [FromQuery] decimal orderAmount = 0)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new { status = 400, message = "Vui lòng nhập mã voucher" });

            var result = await _voucherService.ValidateVoucherForCustomerAsync(code.Trim().ToUpper(), orderAmount);
            return StatusCode(result.Status, result);
        }
    }
}
