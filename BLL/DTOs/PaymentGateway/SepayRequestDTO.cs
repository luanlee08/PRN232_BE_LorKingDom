namespace BLL.DTOs.PaymentGateway;

public class SepayRequest
{
    public string OrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
    public string NotifyUrl { get; set; } = null!;
    public string BankCode { get; set; } = ""; // Optional: specific bank
}

public class SepayResponse
{
    public string PaymentUrl { get; set; } = null!;
    public string QRCodeUrl { get; set; } = null!;
    public string TransactionId { get; set; } = null!;
}

public class SepayCallbackRequest
{
    public string order_id { get; set; } = null!;
    public string transaction_id { get; set; } = null!;
    public string reference_number { get; set; } = null!;
    public decimal amount { get; set; }
    public string content { get; set; } = null!;
    public string status { get; set; } = null!; // "success", "failed", "pending"
    public string bank_code { get; set; } = null!;
    public string account_number { get; set; } = null!;
    public long timestamp { get; set; }
    public string signature { get; set; } = null!;
}

public class SepayQueryRequest
{
    public string OrderId { get; set; } = null!;
    public string TransactionId { get; set; } = null!;
}

public class SepayQueryResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Message { get; set; } = null!;
    public SepayTransactionData? Data { get; set; }
}

public class SepayTransactionData
{
    public string TransactionId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
