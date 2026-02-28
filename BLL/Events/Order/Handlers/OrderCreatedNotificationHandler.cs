using BLL.DTOs.Notifications;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers
{
    /// <summary>
    /// Handles OrderCreatedEvent by sending an order confirmation notification.
    ///
    /// This handler deliberately does NOT know about Order internals — it only
    /// receives what the event carries. Notification logic is fully decoupled
    /// from Order business logic.
    /// </summary>
    public class OrderCreatedNotificationHandler : IDomainEventHandler<OrderCreatedEvent>
    {
        private readonly INotificationCommandService _notificationService;
        private readonly ILogger<OrderCreatedNotificationHandler> _logger;

        public OrderCreatedNotificationHandler(
            INotificationCommandService notificationService,
            ILogger<OrderCreatedNotificationHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(OrderCreatedEvent e, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Sending ORDER_CREATED notification for Order #{OrderId} to Account #{AccountId}",
                e.OrderId, e.AccountId);

            await _notificationService.SendNotificationAsync(
                new SendNotificationRequest
                {
                    TemplateCode = "ORDER_CREATED",
                    TargetType = "User",
                    TargetUserId = e.AccountId,
                    Parameters = new Dictionary<string, string>
                    {
                        ["orderId"] = e.OrderId.ToString(),
                        ["totalAmount"] = e.TotalAmount.ToString("N0"),
                        ["paymentMethod"] = e.PaymentMethod
                    },
                    Payload = $"{{\"type\":\"order\",\"orderId\":{e.OrderId},\"link\":\"/orders/{e.OrderId}\"}}"
                },
                createdByAccountId: 0,  // 0 = system-generated
                isSystemGenerated: true);
        }
    }
}
