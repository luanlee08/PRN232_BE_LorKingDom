using BLL.DTOs.Location;

namespace BLL.Interfaces;

/// <summary>
/// Service for managing GHN location master data with Redis caching
/// Provides province, district, and ward information for shipping integration
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Get all provinces from GHN (cached)
    /// </summary>
    /// <returns>List of provinces with GHN IDs</returns>
    Task<List<GHNProvinceDTO>> GetProvincesAsync();

    /// <summary>
    /// Get all districts for a province (cached)
    /// </summary>
    /// <param name="provinceId">GHN Province ID</param>
    /// <returns>List of districts</returns>
    Task<List<GHNDistrictDTO>> GetDistrictsAsync(int provinceId);

    /// <summary>
    /// Get all wards for a district (cached)
    /// </summary>
    /// <param name="districtId">GHN District ID</param>
    /// <returns>List of wards</returns>
    Task<List<GHNWardDTO>> GetWardsAsync(int districtId);

    /// <summary>
    /// Validate if a province/district/ward combination exists in GHN
    /// </summary>
    /// <param name="provinceId">GHN Province ID</param>
    /// <param name="districtId">GHN District ID</param>
    /// <param name="wardCode">GHN Ward Code (optional)</param>
    /// <returns>Validation result</returns>
    Task<AddressValidationResult> ValidateAddressAsync(int? provinceId, int? districtId, string? wardCode);

    /// <summary>
    /// Find province ID by name (for migration/legacy data)
    /// </summary>
    /// <param name="provinceName">Province name (e.g., "Hồ Chí Minh")</param>
    /// <returns>Province ID or null if not found</returns>
    Task<int?> FindProvinceIdByNameAsync(string provinceName);

    /// <summary>
    /// Clear all cached location data (for manual refresh)
    /// </summary>
    Task ClearCacheAsync();

    /// <summary>
    /// Refresh cache with latest data from GHN API
    /// </summary>
    Task RefreshCacheAsync();
}
