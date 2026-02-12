namespace BLL.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    public int StatusId { get; set; }
    public string? Note { get; set; }
}
