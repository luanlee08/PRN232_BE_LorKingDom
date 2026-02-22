namespace BLL.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    public int StatusId { get; set; }
    public string? Note { get; set; }
    public int? ChangedBy { get; set; }

    /// <summary>
    /// Auto-create GHN shipping order when status changes to Processing
    /// </summary>
    public bool AutoCreateShipping { get; set; } = false;

    /// <summary>
    /// Service ID for GHN shipping (default: 53321 = Standard)
    /// </summary>
    public int? ShippingServiceId { get; set; } = 53321;

    /// <summary>
    /// Required note for shipping: CHOTHUHANG | CHOXEMHANGKHONGTHU | KHONGCHOXEMHANG
    /// </summary>
    public string? ShippingRequiredNote { get; set; } = "KHONGCHOXEMHANG";

    /// <summary>
    /// Additional shipping note
    /// </summary>
    public string? ShippingNote { get; set; }
}
