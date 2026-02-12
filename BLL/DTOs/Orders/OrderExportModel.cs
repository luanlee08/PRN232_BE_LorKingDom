namespace BLL.DTOs.Orders;

public class OrderExportModel
{
    public int STT { get; set; }
    public string OrderCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string StatusName { get; set; } = null!;
    public string RefundStatus { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string Products { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
}
