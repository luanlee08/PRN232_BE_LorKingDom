namespace BLL.DTOs.Orders;

public class OrderResponse
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public string RefundStatus { get; set; } = null!;
    public List<OrderDetailItemResponse> OrderDetails { get; set; } = new();
}
