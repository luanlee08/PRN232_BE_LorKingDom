using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PRN232_LorKingDom.Hubs;

namespace PRN232_LorKingDom.Services;

/// <summary>
/// Implements IShippingRealtimeService using ASP.NET Core SignalR.
/// Lives in the Web project so BLL stays free of web dependencies.
///
/// Pushes the "shippingStatusUpdated" event to all connections in
/// group "order-{orderId}".
/// </summary>
public class SignalRShippingRealtimeService : IShippingRealtimeService
{
    private readonly IHubContext<ShippingHub> _hubContext;
    private readonly ILogger<SignalRShippingRealtimeService> _logger;

    public SignalRShippingRealtimeService(
        IHubContext<ShippingHub> hubContext,
        ILogger<SignalRShippingRealtimeService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task PushShippingStatusAsync(
        int orderId,
        string ghnStatus,
        string displayText,
        string source,
        DateTime occurredAt,
        CancellationToken cancellationToken = default)
    {
        var groupName = $"order-{orderId}";

        var payload = new
        {
            orderId,
            ghnStatus,
            displayText,
            source,
            occurredAt = occurredAt.ToString("O"), // ISO 8601
        };

        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("shippingStatusUpdated", payload, cancellationToken);

            _logger.LogDebug(
                "SignalR pushed 'shippingStatusUpdated' to group {Group}: {Status}",
                groupName, ghnStatus);
        }
        catch (Exception ex)
        {
            // Real-time push failure must never break the main flow
            _logger.LogWarning(ex,
                "SignalR push failed for group {Group} — clients will get update on next poll", groupName);
        }
    }
}
