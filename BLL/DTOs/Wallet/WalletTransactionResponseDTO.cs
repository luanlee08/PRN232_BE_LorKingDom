namespace BLL.DTOs.Wallet;

public class WalletTransactionResponseDTO
{
    public long WalletTransactionId { get; set; }
    public string TxnType { get; set; } = null!;   // "Payment", "Refund", "TopUp"
    public string Direction { get; set; } = null!;  // "In", "Out"
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Method { get; set; }             // "VNPay", "MoMo", "Sepay"
    public string? ExternalRef { get; set; }
    public string Status { get; set; } = null!;     // "Pending", "Completed", "Failed"
    public string? Reason { get; set; }
    public int? RelatedOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
