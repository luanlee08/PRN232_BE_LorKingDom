using BLL.DTOs.Location;
using BLL.Interfaces;
using DAL.Infrastructure.Redis;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BLL.Services;

/// <summary>
/// Service for managing GHN location master data with Redis caching
/// Caches provinces, districts, and wards for 7 days to reduce API calls
/// </summary>
public class LocationService : ILocationService
{
    private readonly IGHNService _ghnService;
    private readonly IRedisService _redis;
    private readonly ILogger<LocationService> _logger;

    // Cache keys
    private const string CACHE_KEY_PROVINCES = "ghn:provinces";
    private const string CACHE_KEY_DISTRICTS = "ghn:districts:{0}"; // {provinceId}
    private const string CACHE_KEY_WARDS = "ghn:wards:{0}"; // {districtId}

    // Cache TTL
    private static readonly TimeSpan ProvincesCacheTTL = TimeSpan.FromDays(7);
    private static readonly TimeSpan DistrictsCacheTTL = TimeSpan.FromDays(7);
    private static readonly TimeSpan WardsCacheTTL = TimeSpan.FromDays(3);

    public LocationService(
        IGHNService ghnService,
        IRedisService redis,
        ILogger<LocationService> logger)
    {
        _ghnService = ghnService;
        _redis = redis;
        _logger = logger;
    }

    public async Task<List<GHNProvinceDTO>> GetProvincesAsync()
    {
        try
        {
            // Try cache first
            var cached = await _redis.GetAsync(CACHE_KEY_PROVINCES);
            if (!string.IsNullOrEmpty(cached))
            {
                var provinces = JsonSerializer.Deserialize<List<GHNProvinceDTO>>(cached);
                if (provinces != null && provinces.Count > 0)
                {
                    _logger.LogInformation($"[LocationService] Loaded {provinces.Count} provinces from cache");
                    return provinces;
                }
            }

            // Fetch from GHN API
            _logger.LogInformation("[LocationService] Fetching provinces from GHN API...");
            var result = await _ghnService.GetProvincesAsync();

            if (result == null || result.Count == 0)
            {
                _logger.LogWarning("[LocationService] No provinces returned from GHN API");
                return new List<GHNProvinceDTO>();
            }

            // Cache the result
            var json = JsonSerializer.Serialize(result);
            await _redis.SetAsync(CACHE_KEY_PROVINCES, json, ProvincesCacheTTL);
            _logger.LogInformation($"[LocationService] Cached {result.Count} provinces (TTL: {ProvincesCacheTTL.TotalDays} days)");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationService] Error fetching provinces");
            return new List<GHNProvinceDTO>();
        }
    }

    public async Task<List<GHNDistrictDTO>> GetDistrictsAsync(int provinceId)
    {
        try
        {
            var cacheKey = string.Format(CACHE_KEY_DISTRICTS, provinceId);

            // Try cache first
            var cached = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var districts = JsonSerializer.Deserialize<List<GHNDistrictDTO>>(cached);
                if (districts != null && districts.Count > 0)
                {
                    _logger.LogInformation($"[LocationService] Loaded {districts.Count} districts for province {provinceId} from cache");
                    return districts;
                }
            }

            // Fetch from GHN API
            _logger.LogInformation($"[LocationService] Fetching districts for province {provinceId} from GHN API...");
            var result = await _ghnService.GetDistrictsAsync(provinceId);

            if (result == null || result.Count == 0)
            {
                _logger.LogWarning($"[LocationService] No districts returned for province {provinceId}");
                return new List<GHNDistrictDTO>();
            }

            // Cache the result
            var json = JsonSerializer.Serialize(result);
            await _redis.SetAsync(cacheKey, json, DistrictsCacheTTL);
            _logger.LogInformation($"[LocationService] Cached {result.Count} districts for province {provinceId}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[LocationService] Error fetching districts for province {provinceId}");
            return new List<GHNDistrictDTO>();
        }
    }

    public async Task<List<GHNWardDTO>> GetWardsAsync(int districtId)
    {
        try
        {
            var cacheKey = string.Format(CACHE_KEY_WARDS, districtId);

            // Try cache first
            var cached = await _redis.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                var wards = JsonSerializer.Deserialize<List<GHNWardDTO>>(cached);
                if (wards != null && wards.Count > 0)
                {
                    _logger.LogInformation($"[LocationService] Loaded {wards.Count} wards for district {districtId} from cache");
                    return wards;
                }
            }

            // Fetch from GHN API
            _logger.LogInformation($"[LocationService] Fetching wards for district {districtId} from GHN API...");
            var result = await _ghnService.GetWardsAsync(districtId);

            if (result == null || result.Count == 0)
            {
                _logger.LogWarning($"[LocationService] No wards returned for district {districtId}");
                return new List<GHNWardDTO>();
            }

            // Cache the result
            var json = JsonSerializer.Serialize(result);
            await _redis.SetAsync(cacheKey, json, WardsCacheTTL);
            _logger.LogInformation($"[LocationService] Cached {result.Count} wards for district {districtId}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[LocationService] Error fetching wards for district {districtId}");
            return new List<GHNWardDTO>();
        }
    }

    public async Task<AddressValidationResult> ValidateAddressAsync(int? provinceId, int? districtId, string? wardCode)
    {
        try
        {
            // Validate province exists
            if (provinceId.HasValue)
            {
                var provinces = await GetProvincesAsync();
                if (!provinces.Any(p => p.ProvinceId == provinceId.Value))
                {
                    return new AddressValidationResult
                    {
                        IsValid = false,
                        Message = $"Province ID {provinceId} không tồn tại trong hệ thống GHN"
                    };
                }
            }

            // Validate district exists and belongs to province
            if (districtId.HasValue)
            {
                if (!provinceId.HasValue)
                {
                    return new AddressValidationResult
                    {
                        IsValid = false,
                        Message = "Province ID bắt buộc khi có District ID"
                    };
                }

                var districts = await GetDistrictsAsync(provinceId.Value);
                var district = districts.FirstOrDefault(d => d.DistrictId == districtId.Value);

                if (district == null)
                {
                    return new AddressValidationResult
                    {
                        IsValid = false,
                        Message = $"District ID {districtId} không tồn tại trong tỉnh {provinceId}"
                    };
                }
            }

            // Validate ward exists and belongs to district
            if (!string.IsNullOrEmpty(wardCode))
            {
                if (!districtId.HasValue)
                {
                    return new AddressValidationResult
                    {
                        IsValid = false,
                        Message = "District ID bắt buộc khi có Ward Code"
                    };
                }

                var wards = await GetWardsAsync(districtId.Value);
                var ward = wards.FirstOrDefault(w => w.WardCode == wardCode);

                if (ward == null)
                {
                    return new AddressValidationResult
                    {
                        IsValid = false,
                        Message = $"Ward Code {wardCode} không tồn tại trong quận {districtId}"
                    };
                }
            }

            return new AddressValidationResult
            {
                IsValid = true,
                Message = "Địa chỉ hợp lệ",
                ProvinceId = provinceId,
                DistrictId = districtId,
                WardCode = wardCode
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationService] Error validating address");
            return new AddressValidationResult
            {
                IsValid = false,
                Message = $"Lỗi xác thực địa chỉ: {ex.Message}"
            };
        }
    }

    public async Task<int?> FindProvinceIdByNameAsync(string provinceName)
    {
        try
        {
            var provinces = await GetProvincesAsync();
            var normalized = NormalizeName(provinceName);

            var matched = provinces.FirstOrDefault(p =>
                NormalizeName(p.ProvinceName).Contains(normalized) ||
                normalized.Contains(NormalizeName(p.ProvinceName)));

            if (matched != null)
            {
                _logger.LogInformation($"[LocationService] Matched '{provinceName}' → Province ID {matched.ProvinceId} ({matched.ProvinceName})");
                return matched.ProvinceId;
            }

            _logger.LogWarning($"[LocationService] Cannot find province for '{provinceName}'");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[LocationService] Error finding province by name: {provinceName}");
            return null;
        }
    }

    public async Task ClearCacheAsync()
    {
        try
        {
            _logger.LogInformation("[LocationService] Clearing all location cache...");

            // Clear provinces
            await _redis.DeleteAsync(CACHE_KEY_PROVINCES);

            // Note: We can't easily clear all district/ward caches without keys
            // In production, consider using Redis SCAN or key patterns

            _logger.LogInformation("[LocationService] ✅ Location cache cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationService] Error clearing cache");
        }
    }

    public async Task RefreshCacheAsync()
    {
        try
        {
            _logger.LogInformation("[LocationService] Refreshing location cache...");

            // Clear and reload provinces
            await _redis.DeleteAsync(CACHE_KEY_PROVINCES);
            var provinces = await GetProvincesAsync();

            _logger.LogInformation($"[LocationService] ✅ Refreshed cache with {provinces.Count} provinces");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LocationService] Error refreshing cache");
        }
    }

    private string NormalizeName(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        return text.ToLowerInvariant()
            .Replace("đ", "d").Replace("Đ", "d")
            .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("ă", "a").Replace("â", "a")
            .Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
            .Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
            .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
            .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ơ", "o")
            .Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
            .Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
            .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
            .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
            .Replace("thanh pho ", "").Replace("tinh ", "").Replace("quan ", "").Replace("huyen ", "")
            .Replace("phuong ", "").Replace("xa ", "").Replace("thi xa ", "").Replace("thi tran ", "")
            .Trim();
    }
}
