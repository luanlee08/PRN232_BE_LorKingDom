namespace BLL.DTOs.Orders;

public class OrderDetailResponse : OrderResponse
{
    public int AccountId { get; set; }
    public string AccountEmail { get; set; } = null!;
    public int? VoucherId { get; set; }
    public string? VoucherCode { get; set; }
    public decimal? VoucherDiscount { get; set; }
    public string? ShippingMethod { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal PaidByWalletAmount { get; set; }
    public decimal PaidByExternalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderStatusHistoryResponse> StatusHistories { get; set; } = new();
    public ShippingInfoDto? ShippingInfo { get; set; }
}
