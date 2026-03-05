using BLL.DTOs.Shipping;

namespace BLL.Interfaces;

/// <summary>
/// Single source of truth for all GHN shipping status update logic.
/// Used by polling worker, webhook controller, manual sync, and demo mode.
/// Adding a real GHN webhook only requires calling ProcessStatusUpdateAsync —
/// no business logic changes needed.
/// </summary>
public interface IGHNShippingStatusService
{
    /// <summary>
    /// Process a known status change for a GHN shipment.
    /// Idempotent: returns NoChange if the status has not changed.
    /// Used by: Webhook, Demo mode.
    /// </summary>
    /// <param name="providerOrderCode">GHN order code.</param>
    /// <param name="newGHNStatus">New GHN raw status string.</param>
    /// <param name="source">Trigger source label: Webhook | Demo | ManualSync</param>
    /// <param name="rawPayload">Optional raw JSON from GHN webhook for audit trail.</param>
    Task<ShippingSyncResult> ProcessStatusUpdateAsync(
        string providerOrderCode,
        string newGHNStatus,
        string source = "ManualSync",
        string? rawPayload = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetch current status from GHN API then call ProcessStatusUpdateAsync.
    /// Used by: ManualSync endpoint, on-demand admin refresh.
    /// </summary>
    Task<ShippingSyncResult> SyncFromGHNApiAsync(
        long shippingTransactionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch-sync all active GHN shipments from the GHN API.
    /// Used by: Hangfire recurring polling job.
    /// </summary>
    Task<BatchSyncResult> SyncActiveShipmentsAsync(
        CancellationToken cancellationToken = default);
}
