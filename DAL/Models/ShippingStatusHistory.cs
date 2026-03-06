namespace DAL.Models;

/// <summary>
/// Stores GHN-provider-level shipping status audit trail.
/// Intentionally separated from OrderStatusHistories, which tracks
/// business-level order states (Pending → Delivered → Completed).
/// This table records every raw GHN status transition regardless of
/// whether the order status changed.
/// </summary>
public class ShippingStatusHistory
{
    public long HistoryId { get; set; }

    public long ShippingTxId { get; set; }

    public int OrderId { get; set; }

    /// <summary>Previous GHN raw status string (e.g. "transporting").</summary>
    public string PreviousStatus { get; set; } = string.Empty;

    /// <summary>New GHN raw status string (e.g. "delivering").</summary>
    public string NewStatus { get; set; } = string.Empty;

    /// <summary>How this update was triggered: Polling | Webhook | ManualSync | Demo</summary>
    public string Source { get; set; } = "Polling";

    /// <summary>Raw JSON payload from GHN webhook or API response. Nullable.</summary>
    public string? RawPayload { get; set; }

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ShippingProviderTransaction ShippingTxn { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
}
