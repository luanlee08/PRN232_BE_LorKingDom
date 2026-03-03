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
                    TargetUserIds = new List<int> { e.AccountId },
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

            // If the customer already paid, send a refund-pending notification
            if (e.HasPaymentToRefund)
            {
                await _notificationService.SendNotificationAsync(
                    new SendNotificationRequest
                    {
                        // Use ORDER_CANCELLED template for now — replace with REFUND_PENDING
                        // when that template is seeded in the DB
                        TemplateCode = "ORDER_CANCELLED",
                        Title = $"Đơn hàng #{e.OrderId:D6} — Hoàn tiền đang xử lý",
                        Message = $"Số tiền {e.TotalAmount:N0}₫ sẽ được hoàn vào ví trong vòng 1-3 ngày làm việc.",
                        TargetType = "User",
                        TargetUserIds = new List<int> { e.AccountId },
                        ActionType = "url",
                        ActionTarget = $"/orders/{e.OrderId}",
                        Parameters = new Dictionary<string, string>
                        {
                            ["orderId"] = e.OrderId.ToString(),
                            ["totalAmount"] = e.TotalAmount.ToString("N0")
                        },
                        Payload = $"{{\"type\":\"payment\",\"orderId\":{e.OrderId},\"link\":\"/orders/{e.OrderId}\"}}"
                    },
                    createdByAccountId: 0,
                    isSystemGenerated: true);
            }
        }
    }
}
