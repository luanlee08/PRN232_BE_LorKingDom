namespace BLL.DTOs.Shipping;

/// <summary>
/// Request to create shipping order (GHN) for an existing order
/// </summary>
public class CreateShippingOrderRequest
{
    public int OrderId { get; set; }
    public string Provider { get; set; } = "GHN"; // GHN
    public int ServiceId { get; set; }
    public int ServiceTypeId { get; set; } = 2; // 2: Standard Express (GHN default)
    public string? Note { get; set; }
    public string RequiredNote { get; set; } = "KHONGCHOXEMHANG"; // CHOTHUHANG, CHOXEMHANGKHONGTHU, KHONGCHOXEMHANG
}

public class CreateShippingOrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? OrderCode { get; set; }
    public string? TrackingNumber { get; set; }
    public decimal? Fee { get; set; }
    public string? ExpectedDeliveryTime { get; set; }
    public string? Provider { get; set; }
}
