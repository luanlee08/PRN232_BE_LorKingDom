using BLL.DTOs;
using BLL.DTOs.Location;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers;

/// <summary>
/// Public API for GHN location master data (provinces, districts, wards)
/// Used by frontend for address dropdowns with proper GHN IDs
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationController> _logger;

    public LocationController(
        ILocationService locationService,
        ILogger<LocationController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all provinces from GHN (cached)
    /// </summary>
    /// <returns>List of provinces with GHN IDs and names</returns>
    [HttpGet("provinces")]
    [ProducesResponseType(typeof(ApiResponse<List<GHNProvinceDTO>>), 200)]
    public async Task<IActionResult> GetProvinces()
    {
        try
        {
            var provinces = await _locationService.GetProvincesAsync();

            return Ok(new ApiResponse<List<GHNProvinceDTO>>
            {
                Status = 200,
                StatusMessage = "Success",
                Message = $"Loaded {provinces.Count} provinces",
                Data = provinces
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationController] Error getting provinces");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "Internal Server Error",
                Message = $"Lỗi khi tải danh sách tỉnh/thành: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Get all districts for a province (cached)
    /// </summary>
    /// <param name="provinceId">GHN Province ID</param>
    /// <returns>List of districts</returns>
    [HttpGet("provinces/{provinceId}/districts")]
    [ProducesResponseType(typeof(ApiResponse<List<GHNDistrictDTO>>), 200)]
    public async Task<IActionResult> GetDistricts(int provinceId)
    {
        try
        {
            var districts = await _locationService.GetDistrictsAsync(provinceId);

            if (districts.Count == 0)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "Not Found",
                    Message = $"Không tìm thấy quận/huyện cho tỉnh {provinceId}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<GHNDistrictDTO>>
            {
                Status = 200,
                StatusMessage = "Success",
                Message = $"Loaded {districts.Count} districts",
                Data = districts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[LocationController] Error getting districts for province {provinceId}");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "Internal Server Error",
                Message = $"Lỗi khi tải danh sách quận/huyện: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Get all wards for a district (cached)
    /// </summary>
    /// <param name="districtId">GHN District ID</param>
    /// <returns>List of wards</returns>
    [HttpGet("districts/{districtId}/wards")]
    [ProducesResponseType(typeof(ApiResponse<List<GHNWardDTO>>), 200)]
    public async Task<IActionResult> GetWards(int districtId)
    {
        try
        {
            var wards = await _locationService.GetWardsAsync(districtId);

            if (wards.Count == 0)
            {
                return NotFound(new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "Not Found",
                    Message = $"Không tìm thấy phường/xã cho quận {districtId}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<GHNWardDTO>>
            {
                Status = 200,
                StatusMessage = "Success",
                Message = $"Loaded {wards.Count} wards",
                Data = wards
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[LocationController] Error getting wards for district {districtId}");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "Internal Server Error",
                Message = $"Lỗi khi tải danh sách phường/xã: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Validate an address combination (province, district, ward)
    /// </summary>
    /// <param name="provinceId">GHN Province ID</param>
    /// <param name="districtId">GHN District ID</param>
    /// <param name="wardCode">GHN Ward Code (optional)</param>
    /// <returns>Validation result</returns>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(ApiResponse<AddressValidationResult>), 200)]
    public async Task<IActionResult> ValidateAddress(
        [FromQuery] int? provinceId,
        [FromQuery] int? districtId,
        [FromQuery] string? wardCode)
    {
        try
        {
            var result = await _locationService.ValidateAddressAsync(provinceId, districtId, wardCode);

            return Ok(new ApiResponse<AddressValidationResult>
            {
                Status = result.IsValid ? 200 : 400,
                StatusMessage = result.IsValid ? "Valid" : "Invalid",
                Message = result.Message,
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationController] Error validating address");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "Internal Server Error",
                Message = $"Lỗi khi xác thực địa chỉ: {ex.Message}",
                Data = null
            });
        }
    }

    /// <summary>
    /// Refresh the location cache (admin only - requires authentication)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("refresh-cache")]
    // [Authorize(Roles = "Admin")] // Uncomment in production
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> RefreshCache()
    {
        try
        {
            await _locationService.RefreshCacheAsync();

            return Ok(new ApiResponse<object>
            {
                Status = 200,
                StatusMessage = "Success",
                Message = "Location cache refreshed successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationController] Error refreshing cache");
            return StatusCode(500, new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "Internal Server Error",
                Message = $"Lỗi khi làm mới cache: {ex.Message}",
                Data = null
            });
        }
    }
}
