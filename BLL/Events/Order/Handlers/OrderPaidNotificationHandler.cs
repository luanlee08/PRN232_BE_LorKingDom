using BLL.DTOs.Notifications;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers
{
    /// <summary>
    /// Handles OrderPaidEvent by sending a payment success notification.
    /// Triggered after a payment callback is confirmed (VNPay, MoMo, COD, Wallet, etc.).
    /// </summary>
    public class OrderPaidNotificationHandler : IDomainEventHandler<OrderPaidEvent>
    {
        private readonly INotificationCommandService _notificationService;
        private readonly ILogger<OrderPaidNotificationHandler> _logger;

        public OrderPaidNotificationHandler(
            INotificationCommandService notificationService,
            ILogger<OrderPaidNotificationHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(OrderPaidEvent e, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Sending PAYMENT_SUCCESS notification for Order #{OrderId} to Account #{AccountId}",
                e.OrderId, e.AccountId);

            await _notificationService.SendNotificationAsync(
                new SendNotificationRequest
                {
                    TemplateCode = "PAYMENT_SUCCESS",
                    TargetType = "User",
                    TargetUserIds = new List<int> { e.AccountId },
                    Parameters = new Dictionary<string, string>
                    {
                        ["orderId"] = e.OrderId.ToString(),
                        ["amount"] = e.Amount.ToString("N0"),
                        ["paymentMethod"] = e.PaymentMethod
                    },
                    Payload = $"{{\"type\":\"payment\",\"orderId\":{e.OrderId},\"amount\":{e.Amount},\"link\":\"/orders/{e.OrderId}\"}}"
                },
                createdByAccountId: 0,
                isSystemGenerated: true);
        }
    }
}
