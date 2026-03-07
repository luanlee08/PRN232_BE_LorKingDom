using BLL.Domain;
using BLL.DTOs.Shipping;
using BLL.Events;
using BLL.Events.Order;
using BLL.Interfaces;
using DAL.Infrastructure;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Core service that handles all GHN shipping status updates.
/// Used by polling worker, GHN webhook, admin manual-sync, and demo mode.
///
/// Design:
///   1. Idempotent — same status update is a no-op.
///   2. Single unit-of-work — ShippingProviderTransaction + ShippingStatusHistory
///      + optional Order + OrderStatusHistory are committed in one atomic transaction.
///   3. Domain events dispatched AFTER commit — failure in a handler never
///      rolls back the shipping update.
///   4. Optimistic concurrency via RowVersion — if two workers race, the
///      loser gets DbUpdateConcurrencyException → logged and skipped.
/// </summary>
public class GHNShippingStatusService : IGHNShippingStatusService
{
    private readonly AspLorKingDomContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGHNService _ghnService;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<GHNShippingStatusService> _logger;

    // System account ID used when order status is auto-updated by the polling system
    private const int SystemAccountId = 0;
    // After this many consecutive errors, stop polling the shipment
    private const int MaxRetryCount = 5;

    public GHNShippingStatusService(
        AspLorKingDomContext context,
        IUnitOfWork unitOfWork,
        IGHNService ghnService,
        IDomainEventDispatcher eventDispatcher,
        ILogger<GHNShippingStatusService> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _ghnService = ghnService;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
    }

    // -------------------------------------------------------
    // IGHNShippingStatusService
    // -------------------------------------------------------

    /// <inheritdoc/>
    public async Task<ShippingSyncResult> ProcessStatusUpdateAsync(
        string providerOrderCode,
        string newGHNStatus,
        string source = "ManualSync",
        string? rawPayload = null,
        CancellationToken cancellationToken = default)
    {
        var result = new ShippingSyncResult { SyncedAt = DateTime.UtcNow };

        // 1. Load shipping transaction with order + order status
        var shipping = await _context.ShippingProviderTransactions
            .Include(s => s.Order)
                .ThenInclude(o => o.Status)
            .FirstOrDefaultAsync(s =>
                s.Provider == "GHN" &&
                s.ProviderOrderCode == providerOrderCode,
                cancellationToken);

        if (shipping == null)
        {
            result.Success = false;
            result.Message = $"No GHN shipping found for order code '{providerOrderCode}'";
            _logger.LogWarning("GHNShippingStatusService: {Message}", result.Message);
            return result;
        }

        result.OldStatus = shipping.Status;
        result.NewStatus = newGHNStatus;

        // 2. Idempotency guard — no-op if status unchanged or already terminal
        if (string.Equals(shipping.Status, newGHNStatus, StringComparison.OrdinalIgnoreCase))
        {
            result.Success = true;
            result.StatusUpdated = false;
            result.Message = "Status unchanged";
            return result;
        }

        if (GHNStatusMapper.IsTerminal(shipping.Status))
        {
            result.Success = true;
            result.StatusUpdated = false;
            result.Message = $"Shipment already in terminal status '{shipping.Status}'";
            return result;
        }

        // 3. Determine whether this GHN status should also update Order status
        var mappedOrderStatus = GHNStatusMapper.MapToOrderStatus(newGHNStatus);
        var oldOrderStatus = shipping.Order.Status?.StatusName;
        StatusOrder? newOrderStatusEntity = null;

        if (mappedOrderStatus != null)
        {
            newOrderStatusEntity = await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == mappedOrderStatus, cancellationToken);
        }

        // 4. Persist inside a transaction
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var now = DateTime.UtcNow;

            // 4a. Update ShippingProviderTransaction
            shipping.Status = newGHNStatus;
            shipping.UpdatedAt = now;
            shipping.LastPolledAt = now;
            shipping.RetryCount = 0;
            shipping.LastErrorMessage = null;
            if (string.Equals(newGHNStatus, GHNStatusMapper.Delivered, StringComparison.OrdinalIgnoreCase))
                shipping.ActualDelivery = now;

            // 4b. Insert ShippingStatusHistory (audit trail)
            var history = new ShippingStatusHistory
            {
                ShippingTxId = shipping.ShippingTransactionId,
                OrderId = shipping.OrderId,
                PreviousStatus = result.OldStatus ?? string.Empty,
                NewStatus = newGHNStatus,
                Source = source,
                RawPayload = rawPayload,
                ProcessedAt = now,
            };
            await _context.ShippingStatusHistories.AddAsync(history, cancellationToken);

            // 4c. Update Order status if mapped
            if (newOrderStatusEntity != null && shipping.Order.StatusId != newOrderStatusEntity.StatusId)
            {
                shipping.Order.StatusId = newOrderStatusEntity.StatusId;
                shipping.Order.UpdatedAt = now;

                var orderStatusHistory = new OrderStatusHistory
                {
                    OrderId = shipping.OrderId,
                    StatusId = newOrderStatusEntity.StatusId,
                    ChangedAt = now,
                    ChangedBy = SystemAccountId,
                    Note = $"Auto-updated by GHN {source}: {newGHNStatus}",
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                await _context.OrderStatusHistories.AddAsync(orderStatusHistory, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogWarning(ex,
                "Concurrency conflict for ShippingTx {ShippingId} (Order {OrderId}). Skipped — another process already updated this record.",
                shipping.ShippingTransactionId, shipping.OrderId);
            result.Success = false;
            result.Message = "Concurrency conflict — skipped safely";
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            // Record the error on the transaction for retry tracking
            await RecordPollingErrorAsync(shipping.ShippingTransactionId, ex.Message);
            _logger.LogError(ex,
                "Failed to persist GHN status update for ShippingTx {ShippingId}", shipping.ShippingTransactionId);
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }

        result.Success = true;
        result.StatusUpdated = true;
        result.StatusText = GHNStatusMapper.GetDisplayText(newGHNStatus);
        result.Message = "Status updated successfully";

        _logger.LogInformation(
            "[GHN {Source}] Order#{OrderId} (ShippingTx#{ShippingId}) {Old} → {New}{OrderNote}",
            source, shipping.OrderId, shipping.ShippingTransactionId,
            result.OldStatus, newGHNStatus,
            mappedOrderStatus != null ? $" | Order → {mappedOrderStatus}" : string.Empty);

        // 5. Dispatch domain events AFTER commit
        var ghnEvent = new GHNShippingStatusChangedEvent
        {
            ShippingTransactionId = shipping.ShippingTransactionId,
            OrderId = shipping.OrderId,
            AccountId = shipping.Order.AccountId,
            ProviderOrderCode = providerOrderCode,
            OldGHNStatus = result.OldStatus ?? string.Empty,
            NewGHNStatus = newGHNStatus,
            Source = source,
            MappedOrderStatus = mappedOrderStatus,
        };
        await _eventDispatcher.DispatchAsync(ghnEvent, cancellationToken);

        // Also dispatch OrderStatusChangedEvent so existing notification handlers run
        if (mappedOrderStatus != null && oldOrderStatus != null)
        {
            var orderEvent = new OrderStatusChangedEvent
            {
                OrderId = shipping.OrderId,
                AccountId = shipping.Order.AccountId,
                CustomerName = shipping.Order.Account?.AccountName,
                OldStatus = oldOrderStatus,
                NewStatus = mappedOrderStatus,
                Note = $"Auto-updated by GHN ({newGHNStatus})",
                TrackingNumber = shipping.ProviderOrderCode,
                ShippingProvider = "GHN",
            };
            await _eventDispatcher.DispatchAsync(orderEvent, cancellationToken);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<ShippingSyncResult> SyncFromGHNApiAsync(
        long shippingTransactionId,
        CancellationToken cancellationToken = default)
    {
        var result = new ShippingSyncResult { SyncedAt = DateTime.UtcNow };

        var shipping = await _context.ShippingProviderTransactions
            .FirstOrDefaultAsync(s => s.ShippingTransactionId == shippingTransactionId, cancellationToken);

        if (shipping == null)
            return new ShippingSyncResult { Success = false, Message = "Shipping not found", SyncedAt = DateTime.UtcNow };

        if (shipping.Provider != "GHN")
            return new ShippingSyncResult { Success = false, Message = "Only GHN shipments are supported", SyncedAt = DateTime.UtcNow };

        if (string.IsNullOrEmpty(shipping.ProviderOrderCode))
            return new ShippingSyncResult { Success = false, Message = "Shipping has no GHN order code", SyncedAt = DateTime.UtcNow };

        if (shipping.RetryCount >= MaxRetryCount)
        {
            _logger.LogWarning(
                "ShippingTx {ShippingId} has exceeded max retry count ({Max}). Skipping poll.",
                shippingTransactionId, MaxRetryCount);
            return new ShippingSyncResult { Success = false, Message = $"Max retry count ({MaxRetryCount}) exceeded", SyncedAt = DateTime.UtcNow };
        }

        // Call GHN API
        GHNStatusResponse? statusResponse;
        try
        {
            statusResponse = await _ghnService.GetOrderStatusAsync(shipping.ProviderOrderCode);
        }
        catch (Exception ex)
        {
            await RecordPollingErrorAsync(shippingTransactionId, ex.Message);
            _logger.LogError(ex, "GHN API call failed for ShippingTx {ShippingId}", shippingTransactionId);
            return new ShippingSyncResult { Success = false, Message = ex.Message, SyncedAt = DateTime.UtcNow };
        }

        if (statusResponse.Code != 200 || statusResponse.Data == null)
        {
            var msg = $"GHN API error {statusResponse.Code}: {statusResponse.Message}";
            await RecordPollingErrorAsync(shippingTransactionId, msg);
            return new ShippingSyncResult { Success = false, Message = msg, SyncedAt = DateTime.UtcNow };
        }

        // Delegate to core update
        return await ProcessStatusUpdateAsync(
            shipping.ProviderOrderCode,
            statusResponse.Data.Status,
            source: "Polling",
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<BatchSyncResult> SyncActiveShipmentsAsync(
        CancellationToken cancellationToken = default)
    {
        var batch = new BatchSyncResult { SyncedAt = DateTime.UtcNow };

        var activeShippings = await _context.ShippingProviderTransactions
            .Include(s => s.Order)
                .ThenInclude(o => o.Status)
            .Where(s =>
                s.Provider == "GHN" &&
                s.ProviderOrderCode != null &&
                s.Status != GHNStatusMapper.Delivered &&
                s.Status != GHNStatusMapper.Returned &&
                s.Status != GHNStatusMapper.Exception &&
                s.Status != GHNStatusMapper.Cancel &&
                s.RetryCount < MaxRetryCount &&
                (s.Order.Status!.StatusName == "Processing" || s.Order.Status.StatusName == "Shipped"))
            .OrderBy(s => s.CreatedAt)          // oldest-first priority
            .ToListAsync(cancellationToken);

        if (activeShippings.Count == 0)
        {
            _logger.LogInformation("GHN batch sync: no active shipments to poll");
            return batch;
        }

        _logger.LogInformation("GHN batch sync: polling {Count} active shipment(s)", activeShippings.Count);

        foreach (var shipping in activeShippings)
        {
            batch.TotalChecked++;
            try
            {
                var result = await SyncFromGHNApiAsync(shipping.ShippingTransactionId, cancellationToken);
                if (result.Success && result.StatusUpdated)
                    batch.Updated++;
                else if (!result.Success)
                {
                    batch.Errors++;
                    batch.ErrorMessages.Add($"ShippingTx {shipping.ShippingTransactionId}: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                batch.Errors++;
                batch.ErrorMessages.Add($"ShippingTx {shipping.ShippingTransactionId}: {ex.Message}");
                _logger.LogError(ex, "Error syncing ShippingTx {ShippingId}", shipping.ShippingTransactionId);
            }

            // Rate-limit guard: 500 ms between GHN API calls
            await Task.Delay(500, cancellationToken);
        }

        return batch;
    }

    // -------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------

    private async Task RecordPollingErrorAsync(long shippingTxId, string errorMessage)
    {
        try
        {
            await _context.ShippingProviderTransactions
                .Where(s => s.ShippingTransactionId == shippingTxId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RetryCount, x => x.RetryCount + 1)
                    .SetProperty(x => x.LastErrorMessage, errorMessage)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not record polling error for ShippingTx {Id}", shippingTxId);
        }
    }
}
