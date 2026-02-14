namespace BLL.DTOs.Orders;

public class OrderQuery
{
    public string? Keyword { get; set; }
    public int? StatusId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "OrderDate";
    public bool SortDesc { get; set; } = true;
}
