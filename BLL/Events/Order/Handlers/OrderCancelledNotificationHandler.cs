using BLL.DTOs.Notifications;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers
{
    /// <summary>
    /// Handles OrderCancelledEvent.
    /// Sends a cancellation confirmation and, if a refund is due, a refund-pending notification.
    /// </summary>
    public class OrderCancelledNotificationHandler : IDomainEventHandler<OrderCancelledEvent>
    {
        private readonly INotificationCommandService _notificationService;
        private readonly ILogger<OrderCancelledNotificationHandler> _logger;

        public OrderCancelledNotificationHandler(
            INotificationCommandService notificationService,
            ILogger<OrderCancelledNotificationHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(OrderCancelledEvent e, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Sending ORDER_CANCELLED notification for Order #{OrderId} to Account #{AccountId}",
                e.OrderId, e.AccountId);

            await _notificationService.SendNotificationAsync(
                new SendNotificationRequest
                {
                    TemplateCode = "ORDER_CANCELLED",
                    TargetType = "User",
                    TargetUserId = e.AccountId,
                    Parameters = new Dictionary<string, string>
                    {
                        ["orderId"] = e.OrderId.ToString(),
                        ["totalAmount"] = e.TotalAmount.ToString("N0"),
                        ["reason"] = e.Reason ?? "Không có lý do"
                    },
                    Payload = $"{{\"type\":\"order\",\"orderId\":{e.OrderId},\"status\":\"Cancelled\",\"link\":\"/orders/{e.OrderId}\"}}"
                },
                createdByAccountId: 0,
                isSystemGenerated: true);

            // If the customer already paid, send a separate refund notification
            if (e.HasPaymentToRefund)
            {
                await _notificationService.SendNotificationAsync(
                    new SendNotificationRequest
                    {
                        TemplateCode = "ORDER_UPDATE",
                        TargetType = "User",
                        TargetUserId = e.AccountId,
                        Parameters = new Dictionary<string, string>
                        {
                            ["orderId"] = e.OrderId.ToString(),
                            ["refundNote"] = $"Hoàn tiền {e.TotalAmount:N0} VND đang được xử lý"
                        },
                        Payload = $"{{\"type\":\"payment\",\"orderId\":{e.OrderId},\"link\":\"/orders/{e.OrderId}\"}}"
                    },
                    createdByAccountId: 0,
                    isSystemGenerated: true);
            }
        }
    }
}
