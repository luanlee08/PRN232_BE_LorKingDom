namespace BLL.Interfaces;

/// <summary>
/// Abstraction over real-time push transport (SignalR).
/// BLL depends on this interface; the concrete implementation
/// (SignalRShippingRealtimeService) lives in the Web project and
/// uses IHubContext&lt;ShippingHub&gt;.
/// This keeps BLL free of ASP.NET Core web dependencies.
/// </summary>
public interface IShippingRealtimeService
{
    /// <summary>
    /// Push a shipping status update to all SignalR clients watching the given order.
    /// Group name convention: "order-{orderId}".
    /// </summary>
    Task PushShippingStatusAsync(
        int orderId,
        string ghnStatus,
        string displayText,
        string source,
        DateTime occurredAt,
        CancellationToken cancellationToken = default);
}
