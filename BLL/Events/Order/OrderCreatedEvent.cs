using BLL.Events;

namespace BLL.Events.Order
{
    /// <summary>
    /// Raised after an order is successfully created and persisted.
    /// Used to trigger order confirmation notifications.
    /// </summary>
    public record OrderCreatedEvent : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public int OrderId { get; init; }
        public int AccountId { get; init; }
        public decimal TotalAmount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string ShippingName { get; init; } = string.Empty;
    }
}
