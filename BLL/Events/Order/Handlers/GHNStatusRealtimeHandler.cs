using BLL.Domain;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Events.Order.Handlers;

/// <summary>
/// Pushes a real-time shipping status update to connected NextJS clients
/// via IShippingRealtimeService (implemented by SignalR in the Web layer).
///
/// This handler fires for every GHN status transition — including
/// intermediate statuses like "transporting" that do not change the
/// Order state. Keeps the frontend live-updated without polling.
/// </summary>
public class GHNStatusRealtimeHandler : IDomainEventHandler<GHNShippingStatusChangedEvent>
{
    private readonly IShippingRealtimeService _realtimeService;
    private readonly ILogger<GHNStatusRealtimeHandler> _logger;

    public GHNStatusRealtimeHandler(
        IShippingRealtimeService realtimeService,
        ILogger<GHNStatusRealtimeHandler> logger)
    {
        _realtimeService = realtimeService;
        _logger = logger;
    }

    public async Task HandleAsync(GHNShippingStatusChangedEvent e, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Pushing realtime update for Order #{OrderId}: {Old} → {New} (source: {Source})",
            e.OrderId, e.OldGHNStatus, e.NewGHNStatus, e.Source);

        await _realtimeService.PushShippingStatusAsync(
            orderId: e.OrderId,
            ghnStatus: e.NewGHNStatus,
            displayText: GHNStatusMapper.GetDisplayText(e.NewGHNStatus),
            source: e.Source,
            occurredAt: e.OccurredAt,
            cancellationToken);
    }
}
