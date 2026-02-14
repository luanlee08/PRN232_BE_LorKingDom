namespace BLL.DTOs.Shipping;

public class GoShipCreateOrderRequest
{
    public string OrderId { get; set; } = null!;
    public GoShipProductInfo[] Products { get; set; } = Array.Empty<GoShipProductInfo>();
    public GoShipOrderInfo Order { get; set; } = null!;
}

public class GoShipProductInfo
{
    public string Name { get; set; } = null!;
    public int Weight { get; set; } // gram
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class GoShipOrderInfo
{
    public string Id { get; set; } = null!;

    // Sender Information
    public string SenderName { get; set; } = null!;
    public string SenderPhone { get; set; } = null!;
    public string SenderAddress { get; set; } = null!;
    public string SenderCity { get; set; } = null!;
    public string SenderDistrict { get; set; } = null!;
    public string SenderWard { get; set; } = null!;

    // Receiver Information
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string ReceiverAddress { get; set; } = null!;
    public string ReceiverCity { get; set; } = null!;
    public string ReceiverDistrict { get; set; } = null!;
    public string ReceiverWard { get; set; } = null!;

    // Order Details
    public decimal CodAmount { get; set; } = 0; // Cash on Delivery amount
    public string Note { get; set; } = "";
    public string ServiceType { get; set; } = "standard"; // "standard" or "express"
    public bool IsFreeship { get; set; } = false;
}

public class GoShipCreateOrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public GoShipOrderData? Data { get; set; }
}

public class GoShipOrderData
{
    public string OrderCode { get; set; } = null!;
    public string TrackingNumber { get; set; } = null!;
    public decimal ShippingFee { get; set; }
    public decimal InsuranceFee { get; set; }
    public string EstimatedPickupTime { get; set; } = null!;
    public string EstimatedDeliveryTime { get; set; } = null!;
    public string QrCode { get; set; } = null!; // URL to QR code image
}

public class GoShipStatusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public GoShipOrderStatus? Data { get; set; }
}

public class GoShipOrderStatus
{
    public string OrderCode { get; set; } = null!;
    public string TrackingNumber { get; set; } = null!;
    public string Status { get; set; } = null!; // "pending", "picked_up", "in_transit", "delivered", "cancelled", "returned"
    public string StatusDescription { get; set; } = null!;
    public decimal ShippingFee { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class GoShipFeeRequest
{
    public string SenderCity { get; set; } = null!;
    public string SenderDistrict { get; set; } = null!;
    public string ReceiverCity { get; set; } = null!;
    public string ReceiverDistrict { get; set; } = null!;
    public string ReceiverWard { get; set; } = null!;
    public int Weight { get; set; } // gram
    public decimal Value { get; set; } // giá trị hàng hóa
    public string ServiceType { get; set; } = "standard";
}

public class GoShipFeeResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public GoShipFeeData? Data { get; set; }
}

public class GoShipFeeData
{
    public decimal ShippingFee { get; set; }
    public decimal InsuranceFee { get; set; }
    public decimal TotalFee { get; set; }
}
