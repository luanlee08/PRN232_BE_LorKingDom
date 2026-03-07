namespace BLL.Interfaces
{
    /// <summary>
    /// Abstraction for real-time notification delivery.
    /// Implemented by SignalRNotificationRealtimeService in the Web layer
    /// so BLL stays free of web/hub dependencies.
    /// </summary>
    public interface INotificationRealtimeService
    {
        /// <summary>
        /// Push a notification event to a specific user's SignalR group ("user-{accountId}").
        /// </summary>
        Task PushToUserAsync(int accountId, object payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Push a notification event to all connections in a role group ("role-{roleId}").
        /// </summary>
        Task PushToRoleAsync(int roleId, object payload, CancellationToken cancellationToken = default);
    }
}
