using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PRN232_LorKingDom.Hubs;

/// <summary>
/// SignalR hub for real-time notification push to admin/staff/customer inboxes.
///
/// NextJS client usage:
/// <code>
///   const conn = new HubConnectionBuilder()
///     .withUrl('/hubs/notifications', { accessTokenFactory: () => token })
///     .withAutomaticReconnect()
///     .build();
///
///   await conn.start();
///   await conn.invoke('JoinUserGroup', accountId.toString());
///   conn.on('notificationReceived', (payload) => { /* refresh badge/list */ });
/// </code>
///
/// Group naming:
///   "user-{accountId}" — personal inbox
///   "role-{roleId}"    — role-wide broadcast (e.g. all admins = "role-1")
/// Auth: JWT token via query string (?access_token=) or Authorization header.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Subscribe the connection to this user's personal notification group.
    /// Call immediately after connecting.
    /// </summary>
    public async Task JoinUserGroup(string accountId)
    {
        var groupName = $"user-{accountId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("NotificationHub: Connection {ConnId} joined user group {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Subscribe the connection to a role-wide notification group.
    /// Admins call this with roleId = "1" to receive store-wide alerts.
    /// </summary>
    public async Task JoinRoleGroup(string roleId)
    {
        var groupName = $"role-{roleId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("NotificationHub: Connection {ConnId} joined role group {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Unsubscribe from personal group (e.g., on logout).
    /// </summary>
    public async Task LeaveUserGroup(string accountId)
    {
        var groupName = $"user-{accountId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogDebug("NotificationHub: Connection {ConnId} left user group {Group}", Context.ConnectionId, groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("NotificationHub: Client connected — {ConnId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("NotificationHub: Client disconnected — {ConnId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
