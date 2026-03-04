namespace BLL.DTOs.Wallet;

public class WalletResponseDTO
{
    public int WalletId { get; set; }
    public int AccountId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "VND";
    public string Status { get; set; } = null!;
    public DateTime? LastTransactionAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
