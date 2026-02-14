namespace BLL.DTOs.PaymentGateway;

public class VNPayRequest
{
    public string OrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public DateTime CreateDate { get; set; } = DateTime.Now;
}

public class VNPayResponse
{
    public string PaymentUrl { get; set; } = null!;
}

public class VNPayCallbackRequest
{
    public string vnp_TmnCode { get; set; } = null!;
    public string vnp_Amount { get; set; } = null!;
    public string vnp_BankCode { get; set; } = null!;
    public string vnp_BankTranNo { get; set; } = null!;
    public string vnp_CardType { get; set; } = null!;
    public string vnp_PayDate { get; set; } = null!;
    public string vnp_OrderInfo { get; set; } = null!;
    public string vnp_TransactionNo { get; set; } = null!;
    public string vnp_ResponseCode { get; set; } = null!;
    public string vnp_TransactionStatus { get; set; } = null!;
    public string vnp_TxnRef { get; set; } = null!;
    public string vnp_SecureHashType { get; set; } = null!;
    public string vnp_SecureHash { get; set; } = null!;
}
