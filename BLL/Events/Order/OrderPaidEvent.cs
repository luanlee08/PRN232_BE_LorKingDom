using BLL.Events;

namespace BLL.Events.Order
{
    /// <summary>
    /// Raised after payment for an order is confirmed (VNPay callback, MoMo callback, COD confirm, etc.).
    /// Used to trigger payment success notifications.
    /// </summary>
    public record OrderPaidEvent : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public int OrderId { get; init; }
        public int AccountId { get; init; }
        public decimal Amount { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
    }
}
