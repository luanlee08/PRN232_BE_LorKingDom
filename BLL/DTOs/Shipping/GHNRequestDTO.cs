namespace BLL.DTOs.Shipping;

public class GHNCreateOrderRequest
{
    public string PaymentTypeId { get; set; } = null!; // 1: Shop/Seller, 2: COD
    public string Note { get; set; } = "";
    public string RequiredNote { get; set; } = "KHONGCHOXEMHANG"; // CHOTHUHANG, CHOXEMHANGKHONGTHU, KHONGCHOXEMHANG
    public string ReturnPhone { get; set; } = null!;
    public string ReturnAddress { get; set; } = null!;
    public string ReturnDistrictId { get; set; } = null!;
    public string ReturnWardCode { get; set; } = null!;
    public string ClientOrderCode { get; set; } = null!;
    public string ToName { get; set; } = null!;
    public string ToPhone { get; set; } = null!;
    public string ToAddress { get; set; } = null!;
    public string ToWardCode { get; set; } = null!;
    public int ToDistrictId { get; set; }
    public int CodAmount { get; set; }
    public string Content { get; set; } = null!;
    public int Weight { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ServiceId { get; set; }
    public int ServiceTypeId { get; set; } // 2: Standard Express
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
