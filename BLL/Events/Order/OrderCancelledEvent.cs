using BLL.Events;

namespace BLL.Events.Order
{
    /// <summary>
    /// Raised after an order is cancelled (by customer or admin).
    /// Used to send cancellation confirmation and trigger refund notifications if applicable.
    /// </summary>
    public record OrderCancelledEvent : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public int OrderId { get; init; }
        public int AccountId { get; init; }
        public decimal TotalAmount { get; init; }
        public string? Reason { get; init; }
        public bool HasPaymentToRefund { get; init; }
    }
}
