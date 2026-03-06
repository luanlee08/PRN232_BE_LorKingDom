namespace BLL.DTOs.Shipping;

public class GHNCreateOrderRequest
{
    public string PaymentTypeId { get; set; } = null!; // 1: Shop/Seller, 2: COD
    public string Note { get; set; } = "";
    public string RequiredNote { get; set; } = "KHONGCHOXEMHANG"; // CHOTHUHANG, CHOXEMHANGKHONGTHU, KHONGCHOXEMHANG

    // Sender/Pickup Information (Required by GHN)
    public string FromName { get; set; } = null!;
    public string FromPhone { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
    public string FromWardName { get; set; } = null!;
    public string FromDistrictName { get; set; } = null!;
    public string FromProvinceName { get; set; } = null!;
    public int FromDistrictId { get; set; } // REQUIRED - GHN needs this to identify pickup warehouse

    // Return Information
    public string ReturnPhone { get; set; } = null!;
    public string ReturnAddress { get; set; } = null!;
    public string ReturnDistrictId { get; set; } = null!;
    public string ReturnWardCode { get; set; } = null!;

    public string ClientOrderCode { get; set; } = null!;

    // Recipient Information
    public string ToName { get; set; } = null!;
    public string ToPhone { get; set; } = null!;
    public string ToAddress { get; set; } = null!;
    public string ToWardName { get; set; } = "";        // Text name - REQUIRED by GHN docs
    public string ToDistrictName { get; set; } = "";    // Text name - REQUIRED by GHN docs
    public string ToProvinceName { get; set; } = "";    // Text name - REQUIRED by GHN docs
    public string ToWardCode { get; set; } = "";        // Optional but helps accuracy
    public int ToDistrictId { get; set; }                // Optional but helps accuracy

    // Package Details
    public int CodAmount { get; set; }
    public string Content { get; set; } = null!;
    public int Weight { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ServiceId { get; set; }
    public int ServiceTypeId { get; set; } // 2: Standard Express
    public int? InsuranceValue { get; set; } // Optional: Insurance value for package
    public GHNItem[] Items { get; set; } = Array.Empty<GHNItem>();
}

public class GHNItem
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int Quantity { get; set; }
    public int Price { get; set; }
    public int Weight { get; set; }
}

public class GHNCreateOrderResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public GHNOrderData? Data { get; set; }
}

public class GHNOrderData
{
    public string OrderCode { get; set; } = null!;
    public string SortCode { get; set; } = null!;
    public string TransType { get; set; } = null!;
    public string WardEncode { get; set; } = null!;
    public string DistrictEncode { get; set; } = null!;
    public decimal Fee { get; set; }
    public decimal TotalFee { get; set; }
    public string ExpectedDeliveryTime { get; set; } = null!;
}

public class GHNStatusResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = null!;
    public GHNOrderStatus? Data { get; set; }
}

public class GHNOrderStatus
{
    public string OrderCode { get; set; } = null!;
    public string Status { get; set; } = null!; // ready_to_pick, picking, picked, storing, transporting, delivering, delivered, return
    public string StatusText { get; set; } = null!;
    public decimal Fee { get; set; }
    public string ExpectedDeliveryTime { get; set; } = null!;
}

public class GHNServiceRequest
{
    public int ShopId { get; set; }
    public int FromDistrict { get; set; }
    public int ToDistrict { get; set; }
}

public class GHNServiceResponse
{
    public int Code { get; set; }
    public GHNService[] Data { get; set; } = Array.Empty<GHNService>();
}

public class GHNService
{
    public int ServiceId { get; set; }
    public string ShortName { get; set; } = null!;
    public int ServiceTypeId { get; set; }
}

// Master data DTOs for dynamic province/district lookup
public class GHNProvinceListResponse
{
    public int Code { get; set; }
    public GHNProvinceData[]? Data { get; set; }
}

public class GHNProvinceData
{
    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; } = null!;
}

public class GHNDistrictListResponse
{
    public int Code { get; set; }
    public GHNDistrictData[]? Data { get; set; }
}

public class GHNDistrictData
{
    public int DistrictId { get; set; }
    public string DistrictName { get; set; } = null!;
}

// Ward Master Data DTOs
public class GHNWardListResponse
{
    public int Code { get; set; }
    public GHNWardData[]? Data { get; set; }
}

public class GHNWardData
{
    public string WardCode { get; set; } = null!;
    public int DistrictId { get; set; }
    public string WardName { get; set; } = null!;
}

// Full tracking detail for customer-facing UI
public class GHNTrackingDetail
{
    public string OrderCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string StatusText { get; set; } = null!;
    public string? ToName { get; set; }
    public string? ToPhone { get; set; }
    public string? ToAddress { get; set; }
    public decimal ShippingFee { get; set; }
    public string? ExpectedDeliveryTime { get; set; }
    public string? FinishDate { get; set; }
    public List<GHNTrackingLogItem> Log { get; set; } = new();
}

public class GHNTrackingLogItem
{
    public string Status { get; set; } = null!;
    public string StatusText { get; set; } = null!;
    public DateTime UpdatedDate { get; set; }
}
