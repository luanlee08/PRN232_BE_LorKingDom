namespace BLL.DTOs.Shipping;

/// <summary>
/// Result of syncing a single shipping status from provider (GHN)
/// </summary>
public class ShippingSyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? StatusText { get; set; }
    public bool StatusUpdated { get; set; }
    public DateTime SyncedAt { get; set; }
}

/// <summary>
/// Result of batch syncing multiple shippings
/// </summary>
public class BatchSyncResult
{
    public int TotalChecked { get; set; }
    public int Updated { get; set; }
    public int Errors { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public DateTime SyncedAt { get; set; }
}
