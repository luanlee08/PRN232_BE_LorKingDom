namespace BLL.DTOs.Wallet;

public class TopUpRequestDTO
{
    public decimal Amount { get; set; }
    public string Gateway { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
}
