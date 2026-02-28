using BLL.Events;

namespace BLL.Events.Order
{
    /// <summary>
    /// Raised after an order status is changed by admin or system.
    /// Used to notify the customer of delivery/processing updates.
    /// </summary>
    public record OrderStatusChangedEvent : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public int OrderId { get; init; }
        public int AccountId { get; init; }
        public string OldStatus { get; init; } = string.Empty;
        public string NewStatus { get; init; } = string.Empty;
        public string? Note { get; init; }
        /// <summary>Tracking number from ShippingProviderTransaction (e.g. GHN order code)</summary>
        public string? TrackingNumber { get; init; }
        /// <summary>Shipping provider name (e.g. "GHN")</summary>
        public string? ShippingProvider { get; init; }
    }
}
