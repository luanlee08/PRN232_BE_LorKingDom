namespace BLL.DTOs.Location;

/// <summary>
/// GHN Province data from master-data API
/// </summary>
public class GHNProvinceDTO
{
    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; } = null!;
    public string Code { get; set; } = null!;
}

/// <summary>
/// GHN District data from master-data API
/// </summary>
public class GHNDistrictDTO
{
    public int DistrictId { get; set; }
    public int ProvinceId { get; set; }
    public string DistrictName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int SupportType { get; set; }
}

/// <summary>
/// GHN Ward data from master-data API
/// </summary>
public class GHNWardDTO
{
    public string WardCode { get; set; } = null!;
    public int DistrictId { get; set; }
    public string WardName { get; set; } = null!;
}

/// <summary>
/// Response wrapper for GHN Province list
/// </summary>
public class GHNProvinceListResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public GHNProvinceDTO[] Data { get; set; } = Array.Empty<GHNProvinceDTO>();
}

/// <summary>
/// Response wrapper for GHN District list
/// </summary>
public class GHNDistrictListResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public GHNDistrictDTO[] Data { get; set; } = Array.Empty<GHNDistrictDTO>();
}

/// <summary>
/// Response wrapper for GHN Ward list
/// </summary>
public class GHNWardListResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public GHNWardDTO[] Data { get; set; } = Array.Empty<GHNWardDTO>();
}

/// <summary>
/// Simplified location data for frontend dropdowns
/// </summary>
public class LocationOptionDTO
{
    public int? Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
}

/// <summary>
/// Address validation result
/// </summary>
public class AddressValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = null!;
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public string? WardCode { get; set; }
}
