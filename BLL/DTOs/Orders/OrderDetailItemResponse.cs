namespace BLL.DTOs.Orders;

public class OrderDetailItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string? ImageUrl { get; set; }
}
