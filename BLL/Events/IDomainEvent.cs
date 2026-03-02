namespace BLL.Events
{
    /// <summary>
    /// Marker interface for all Domain Events.
    /// Domain events represent something that happened in the domain.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>When the event occurred (UTC)</summary>
        DateTime OccurredAt { get; }
    }

    /// <summary>
    /// Handler for a specific domain event type.
    /// Register multiple handlers for the same event — they all get invoked.
    /// </summary>
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Dispatcher that resolves and invokes all registered handlers for an event.
    /// Decouples the publisher (OrderCommandService) from subscribers (NotificationHandlers).
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent;
    }
}
