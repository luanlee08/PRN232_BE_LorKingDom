namespace BLL.DTOs.Shipping;

/// <summary>
/// Webhook payload from GHN when order status changes
/// </summary>
public class GHNWebhookPayload
{
    public string OrderCode { get; set; } = null!;
    public string Status { get; set; } = null!; // ready_to_pick, picking, picked, storing, transporting, delivering, delivered, return, returned, exception
    public string? StatusText { get; set; }
    public string? Reason { get; set; }
    public string? ReasonCode { get; set; }
    public decimal? CodAmount { get; set; }
    public decimal? Fee { get; set; }
    public string? Time { get; set; }
    public string? ClientOrderCode { get; set; }
}

public class GHNWebhookRequest
{
    public string Type { get; set; } = null!; // "Update Order Status"
    public GHNWebhookPayload Data { get; set; } = null!;
}
