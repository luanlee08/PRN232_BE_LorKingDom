using BLL.Events;

namespace BLL.Events.Order;

/// <summary>
/// Raised after a GHN provider-level shipping status changes.
/// This is distinct from OrderStatusChangedEvent:
///   - GHN has 10+ intermediate statuses; Order has 7 business states.
///   - Not every GHN status change triggers an order state transition.
///   - This event drives: SignalR realtime push + intermediate shipping notifications.
///   - When GHN status maps to an Order status, BOTH events are dispatched so
///     existing OrderStatusChangedEvent notification handlers are not bypassed.
/// </summary>
public record GHNShippingStatusChangedEvent : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public long ShippingTransactionId { get; init; }
    public int OrderId { get; init; }
    public int AccountId { get; init; }

    /// <summary>GHN order code (e.g. "GHNXXXXXXXX").</summary>
    public string ProviderOrderCode { get; init; } = string.Empty;

    /// <summary>Previous GHN raw status (e.g. "transporting").</summary>
    public string OldGHNStatus { get; init; } = string.Empty;

    /// <summary>New GHN raw status (e.g. "delivering").</summary>
    public string NewGHNStatus { get; init; } = string.Empty;

    /// <summary>How this update was triggered: Polling | Webhook | ManualSync | Demo</summary>
    public string Source { get; init; } = "Polling";

    /// <summary>
    /// When non-null, the GHN status also triggered an Order-level status transition.
    /// E.g. GHN "delivered" → Order "Delivered".
    /// OrderStatusChangedEvent is also dispatched in that case.
    /// </summary>
    public string? MappedOrderStatus { get; init; }
}
