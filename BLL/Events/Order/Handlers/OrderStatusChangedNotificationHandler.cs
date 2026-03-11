using BLL.DTOs.Notifications;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers
{
    /// <summary>
    /// Handles OrderStatusChangedEvent by notifying the customer of the new status.
    /// Maps known status names to their corresponding notification template codes.
    /// </summary>
    public class OrderStatusChangedNotificationHandler : IDomainEventHandler<OrderStatusChangedEvent>
    {
        private readonly INotificationCommandService _notificationService;
        private readonly ILogger<OrderStatusChangedNotificationHandler> _logger;

        // Map order status → notification template code
        private static readonly Dictionary<string, string> _templateMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Confirmed"] = "ORDER_CONFIRMED",
            ["Shipped"] = "ORDER_SHIPPED",
            ["Delivered"] = "ORDER_DELIVERED",
            ["Cancelled"] = "ORDER_CANCELLED",
        };

        public OrderStatusChangedNotificationHandler(
            INotificationCommandService notificationService,
            ILogger<OrderStatusChangedNotificationHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(OrderStatusChangedEvent e, CancellationToken cancellationToken = default)
        {
            if (!_templateMap.TryGetValue(e.NewStatus, out var templateCode))
            {
                // Status change has no dedicated template — skip silently
                return;
            }

            _logger.LogInformation(
                "Sending {TemplateCode} notification for Order #{OrderId} (status: {Old} → {New})",
                templateCode, e.OrderId, e.OldStatus, e.NewStatus);

            await _notificationService.SendNotificationAsync(
                new SendNotificationRequest
                {
                    TemplateCode = templateCode,
                    TargetType = "User",
                    TargetUserIds = new List<int> { e.AccountId },
                    Parameters = new Dictionary<string, string>
                    {
                        ["orderCode"] = $"ORD{e.OrderId:D6}",
                        ["orderId"] = e.OrderId.ToString(),
                        ["OrderId"] = e.OrderId.ToString(),
                        ["customerName"] = e.CustomerName ?? string.Empty,
                        ["oldStatus"] = e.OldStatus,
                        ["newStatus"] = e.NewStatus,
                        ["note"] = e.Note ?? string.Empty,
                        // Shipping fields — match both casing variants used in DB templates
                        ["trackingCode"] = e.TrackingNumber ?? string.Empty,
                        ["TrackingNumber"] = e.TrackingNumber ?? string.Empty,
                        ["shippingUnit"] = e.ShippingProvider ?? string.Empty,
                        ["ShippingUnit"] = e.ShippingProvider ?? string.Empty,
                    },
                    Payload = $"{{\"type\":\"order\",\"orderId\":{e.OrderId},\"status\":\"{e.NewStatus}\",\"link\":\"/profile?tab=orders&orderId={e.OrderId}\"}}"
                },
                createdByAccountId: 0,
                isSystemGenerated: true);
        }
    }
}
