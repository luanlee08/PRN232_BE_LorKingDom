using BLL.Domain;
using BLL.DTOs.Notifications;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers;

/// <summary>
/// Sends shipping-specific push notifications for intermediate GHN statuses
/// (picking, transporting, delivering, return) that do NOT trigger an
/// OrderStatusChangedEvent.
///
/// Order-terminal statuses (delivered, cancelled) are already handled by
/// OrderStatusChangedNotificationHandler via the OrderStatusChangedEvent
/// that GHNShippingStatusService dispatches alongside this event.
/// </summary>
public class GHNStatusNotificationHandler : IDomainEventHandler<GHNShippingStatusChangedEvent>
{
    private readonly INotificationCommandService _notificationService;
    private readonly ILogger<GHNStatusNotificationHandler> _logger;

    public GHNStatusNotificationHandler(
        INotificationCommandService notificationService,
        ILogger<GHNStatusNotificationHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(GHNShippingStatusChangedEvent e, CancellationToken cancellationToken = default)
    {
        // Only handle intermediate statuses not covered by OrderStatusChangedEvent
        var templateCode = GHNStatusMapper.GetNotificationTemplate(e.NewGHNStatus);
        if (templateCode == null) return;

        _logger.LogInformation(
            "Sending {Template} notification for Order #{OrderId} (GHN: {Old} → {New})",
            templateCode, e.OrderId, e.OldGHNStatus, e.NewGHNStatus);

        await _notificationService.SendNotificationAsync(
            new SendNotificationRequest
            {
                TemplateCode = templateCode,
                TargetType = "User",
                TargetUserIds = new List<int> { e.AccountId },
                Parameters = new Dictionary<string, string>
                {
                    ["orderId"] = e.OrderId.ToString(),
                    ["OrderId"] = e.OrderId.ToString(),
                    ["trackingCode"] = e.ProviderOrderCode,
                    ["TrackingCode"] = e.ProviderOrderCode,
                    ["ghnStatus"] = e.NewGHNStatus,
                    ["statusText"] = GHNStatusMapper.GetDisplayText(e.NewGHNStatus),
                },
                Payload = $"{{\"type\":\"shipping\",\"orderId\":{e.OrderId},\"ghnStatus\":\"{e.NewGHNStatus}\",\"link\":\"/profile?tab=orders&orderId={e.OrderId}\"}}"
            },
            createdByAccountId: 0,
            isSystemGenerated: true);
    }
}
