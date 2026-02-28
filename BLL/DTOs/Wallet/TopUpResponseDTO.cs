namespace BLL.DTOs.Wallet;

public class TopUpResponseDTO
{
    public string PaymentUrl { get; set; } = null!;
    public string IdempotencyKey { get; set; } = null!;
    public long TransactionId { get; set; }
    public string Gateway { get; set; } = null!;
}
