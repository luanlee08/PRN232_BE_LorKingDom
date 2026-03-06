using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [Route("api/admin/media")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AMediaController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<AMediaController> _logger;

        public AMediaController(ICloudinaryService cloudinaryService, ILogger<AMediaController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        /// <summary>Upload a single image to Cloudinary and return its URL.</summary>
        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "campaigns")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file ảnh." });

            var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowed.Contains(file.ContentType.ToLower()))
                return BadRequest(new { message = "Chỉ chấp nhận file ảnh (JPG, PNG, WEBP, GIF)." });

            try
            {
                var url = await _cloudinaryService.UploadImageAsync(file, folder);
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Image upload failed");
                return StatusCode(500, new { message = "Upload thất bại: " + ex.Message });
            }
        }
    }
}
