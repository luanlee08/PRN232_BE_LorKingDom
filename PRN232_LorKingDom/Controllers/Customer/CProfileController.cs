using BLL.DTOs.Profile;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class CProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public CProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản" });
            }

            var result = await _profileService.GetProfileAsync(accountId);
            return StatusCode(result.Status, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản" });
            }

            var result = await _profileService.UpdateProfileAsync(accountId, request);
            return StatusCode(result.Status, result);
        }

        [HttpPost("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản" });
            }

            var result = await _profileService.UpdateProfileAvatarAsync(accountId, file);
            return StatusCode(result.Status, result);
        }
    }
}
