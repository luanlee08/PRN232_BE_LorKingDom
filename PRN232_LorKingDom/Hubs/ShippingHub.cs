using Microsoft.AspNetCore.SignalR;

namespace PRN232_LorKingDom.Hubs;

/// <summary>
/// SignalR hub for real-time shipping status updates.
///
/// NextJS client usage:
/// <code>
///   const conn = new HubConnectionBuilder()
///     .withUrl('/hubs/shipping', { accessTokenFactory: () => token })
///     .withAutomaticReconnect()
///     .build();
///
///   await conn.start();
///   await conn.invoke('JoinOrderGroup', orderId.toString());
///   conn.on('shippingStatusUpdated', (data) => { /* update UI */ });
/// </code>
///
/// Group naming: "order-{orderId}"
/// Auth: JWT token via query string (?access_token=) or Authorization header.
/// </summary>
public class ShippingHub : Hub
{
    private readonly ILogger<ShippingHub> _logger;

    public ShippingHub(ILogger<ShippingHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Subscribe the connection to receive updates for a specific order.
    /// Call immediately after connecting on the order detail page.
    /// </summary>
    public async Task JoinOrderGroup(string orderId)
    {
        var groupName = $"order-{orderId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("SignalR: Connection {ConnId} joined group {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Unsubscribe from order updates (e.g., when navigating away).
    /// </summary>
    public async Task LeaveOrderGroup(string orderId)
    {
        var groupName = $"order-{orderId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("SignalR: Connection {ConnId} left group {Group}", Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("SignalR: Client connected — {ConnId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("SignalR: Client disconnected — {ConnId} ({Reason})",
            Context.ConnectionId, exception?.Message ?? "clean");
        await base.OnDisconnectedAsync(exception);
    }
}
