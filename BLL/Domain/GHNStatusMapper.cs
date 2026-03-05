using BLL.DTOs.Orders;

namespace BLL.Domain;

/// <summary>
/// Maps GHN raw status strings to internal domain concepts.
/// Single source of truth — referenced by polling worker, webhook processor, and demo worker.
/// </summary>
public static class GHNStatusMapper
{
    // -------------------------------------------------------
    // Status constants (GHN raw values)
    // -------------------------------------------------------
    public const string ReadyToPick = "ready_to_pick";
    public const string Picking = "picking";
    public const string Picked = "picked";
    public const string Storing = "storing";
    public const string Transporting = "transporting";
    public const string Delivering = "delivering";
    public const string Delivered = "delivered";
    public const string Return = "return";
    public const string Returned = "returned";
    public const string Exception = "exception";
    public const string Cancel = "cancel";

    /// <summary>
    /// GHN statuses that represent a final/terminal state.
    /// Once a shipment reaches these, polling stops.
    /// </summary>
    public static readonly HashSet<string> TerminalStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        Delivered, Returned, Exception, Cancel
    };

    /// <summary>
    /// Demo flow auto-advance sequence (happy path).
    /// </summary>
    public static readonly IReadOnlyList<string> DemoFlowSequence = new[]
    {
        ReadyToPick, Picking, Transporting, Delivering, Delivered
    };

    /// <summary>
    /// Maps a GHN status to the corresponding Order status name.
    /// Returns null when the GHN status does not trigger an order-level transition.
    /// </summary>
    private static readonly Dictionary<string, string> _orderStatusMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [Delivered] = OrderStatusNames.Delivered,
            [Returned] = OrderStatusNames.Cancelled,
        };

    /// <summary>
    /// GHN-level statuses that have a dedicated shipping notification template.
    /// Order-level transitions (Delivered) are covered by OrderStatusChangedEvent.
    /// </summary>
    private static readonly Dictionary<string, string> _notificationTemplateMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [Picking] = "SHIPPING_PICKING",
            [Transporting] = "SHIPPING_TRANSPORTING",
            [Delivering] = "SHIPPING_DELIVERING",
            [Return] = "SHIPPING_RETURNING",
        };

    /// <summary>Vietnamese display text for each GHN status.</summary>
    private static readonly Dictionary<string, string> _displayTexts =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [ReadyToPick] = "Chờ lấy hàng",
            [Picking] = "Đang lấy hàng",
            [Picked] = "Đã lấy hàng",
            [Storing] = "Đang lưu kho",
            [Transporting] = "Đang vận chuyển",
            [Delivering] = "Đang giao hàng",
            [Delivered] = "Đã giao hàng",
            [Return] = "Đang hoàn hàng",
            [Returned] = "Đã hoàn hàng",
            [Exception] = "Ngoại lệ",
            [Cancel] = "Đã hủy",
        };

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public static bool IsTerminal(string? ghnStatus)
        => ghnStatus != null && TerminalStatuses.Contains(ghnStatus);

    /// <summary>
    /// Returns the matching Order status name if this GHN status should
    /// trigger an order-level transition, otherwise null.
    /// </summary>
    public static string? MapToOrderStatus(string? ghnStatus)
        => ghnStatus != null && _orderStatusMap.TryGetValue(ghnStatus, out var s) ? s : null;

    /// <summary>
    /// Returns the notification template code for intermediate GHN statuses, or null.
    /// Order-terminal statuses (delivered) are handled by OrderStatusChangedEvent.
    /// </summary>
    public static string? GetNotificationTemplate(string? ghnStatus)
        => ghnStatus != null && _notificationTemplateMap.TryGetValue(ghnStatus, out var t) ? t : null;

    public static string GetDisplayText(string? ghnStatus)
        => ghnStatus != null && _displayTexts.TryGetValue(ghnStatus, out var d) ? d : ghnStatus ?? "Unknown";

    /// <summary>
    /// Returns the next GHN status in the demo flow sequence after the current one.
    /// Returns null if already at terminal or not found.
    /// </summary>
    public static string? GetNextDemoStatus(string? currentStatus)
    {
        if (currentStatus == null) return DemoFlowSequence[0];
        var idx = DemoFlowSequence.ToList()
            .FindIndex(s => string.Equals(s, currentStatus, StringComparison.OrdinalIgnoreCase));
        if (idx < 0 || idx >= DemoFlowSequence.Count - 1) return null;
        return DemoFlowSequence[idx + 1];
    }
}
