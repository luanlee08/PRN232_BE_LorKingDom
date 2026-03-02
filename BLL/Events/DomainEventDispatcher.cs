using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BLL.Events
{
    /// <summary>
    /// Resolves all registered IDomainEventHandler&lt;TEvent&gt; via IServiceProvider
    /// and invokes them in sequence.
    ///
    /// Design note:
    ///   - Handlers are resolved from the current DI scope, so they share the same
    ///     DbContext/UnitOfWork as the originating command — no double-commit needed.
    ///   - To scale to a message bus later, swap this class for a RabbitMQ/Azure
    ///     Service Bus publisher without touching event definitions or handlers.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(
            IServiceProvider serviceProvider,
            ILogger<DomainEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvent
        {
            var eventName = typeof(TEvent).Name;
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>().ToList();

            if (!handlers.Any())
            {
                _logger.LogDebug("No handlers registered for domain event {EventName}", eventName);
                return;
            }

            _logger.LogInformation(
                "Dispatching domain event {EventName} to {HandlerCount} handler(s)",
                eventName, handlers.Count);

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log but do not re-throw — a notification failure must NOT
                    // roll back the Order transaction.
                    _logger.LogError(
                        ex,
                        "Handler {HandlerType} failed for domain event {EventName}",
                        handler.GetType().Name, eventName);
                }
            }
        }
    }
}
