namespace BLL.DTOs.Orders;

public class OrderStatusHistoryResponse
{
    public int OrderStatusHistoryId { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public DateTime ChangedAt { get; set; }
    public int? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public string? Note { get; set; }
}
