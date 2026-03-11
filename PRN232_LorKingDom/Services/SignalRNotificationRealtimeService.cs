using BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PRN232_LorKingDom.Hubs;

namespace PRN232_LorKingDom.Services;

/// <summary>
/// Implements INotificationRealtimeService using ASP.NET Core SignalR.
/// Pushes the "notificationReceived" event to the appropriate group so
/// connected clients can refresh their notification badge / list instantly.
/// </summary>
public class SignalRNotificationRealtimeService : INotificationRealtimeService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationRealtimeService> _logger;

    public SignalRNotificationRealtimeService(
        IHubContext<NotificationHub> hubContext,
        ILogger<SignalRNotificationRealtimeService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task PushToUserAsync(int accountId, object payload, CancellationToken cancellationToken = default)
    {
        var groupName = $"user-{accountId}";
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("notificationReceived", payload, cancellationToken);

            _logger.LogDebug("NotificationHub pushed to user group {Group}", groupName);
        }
        catch (Exception ex)
        {
            // Real-time failure must never break the calling flow
            _logger.LogWarning(ex, "SignalR push failed for user group {Group} — clients will update on next poll", groupName);
        }
    }

    public async Task PushToRoleAsync(int roleId, object payload, CancellationToken cancellationToken = default)
    {
        var groupName = $"role-{roleId}";
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("notificationReceived", payload, cancellationToken);

            _logger.LogDebug("NotificationHub pushed to role group {Group}", groupName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR push failed for role group {Group} — clients will update on next poll", groupName);
        }
    }
}
