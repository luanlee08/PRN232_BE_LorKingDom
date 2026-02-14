namespace BLL.DTOs.PaymentGateway;

public class MoMoRequest
{
    public string OrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string NotifyUrl { get; set; } = null!;
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
}

public class MoMoResponse
{
    public string PayUrl { get; set; } = null!;
    public string DeepLink { get; set; } = null!;
    public string QRCodeUrl { get; set; } = null!;
}

public class MoMoCallbackRequest
{
    public string partnerCode { get; set; } = null!;
    public string orderId { get; set; } = null!;
    public string requestId { get; set; } = null!;
    public long amount { get; set; }
    public string orderInfo { get; set; } = null!;
    public string orderType { get; set; } = null!;
    public string transId { get; set; } = null!;
    public int resultCode { get; set; }
    public string message { get; set; } = null!;
    public string payType { get; set; } = null!;
    public long responseTime { get; set; }
    public string extraData { get; set; } = null!;
    public string signature { get; set; } = null!;
}
