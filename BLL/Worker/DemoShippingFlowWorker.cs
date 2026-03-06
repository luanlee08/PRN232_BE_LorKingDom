using BLL.Domain;
using BLL.Interfaces;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker;

/// <summary>
/// Demo-only Hangfire worker that auto-advances GHN shipping status through
/// the happy-path sequence: ready_to_pick → picking → transporting → delivering → delivered.
///
/// Enabled only when appsettings.json has:
///   "DemoMode": { "AutoFlowEnabled": true }
///
/// Design principles:
///   - Does NOT call the real GHN API — feeds statuses directly into
///     IGHNShippingStatusService as Source = "Demo".
///   - Domain/BLL know nothing about "demo" beyond the Source string.
///   - Safe to disable at any time without data loss or business logic changes.
///   - One step per 2-minute invocation per order (configurable via DemoStepIntervalMinutes).
/// </summary>
public class DemoShippingFlowWorker
{
    private readonly AspLorKingDomContext _context;
    private readonly IGHNShippingStatusService _shippingStatusService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DemoShippingFlowWorker> _logger;

    public DemoShippingFlowWorker(
        AspLorKingDomContext context,
        IGHNShippingStatusService shippingStatusService,
        IConfiguration configuration,
        ILogger<DemoShippingFlowWorker> logger)
    {
        _context = context;
        _shippingStatusService = shippingStatusService;
        _configuration = configuration;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 0)] // Demo failures should not spam the queue
    public async Task AdvanceDemoShippingFlowJob()
    {
        if (!IsDemoModeEnabled())
        {
            _logger.LogDebug("DemoShippingFlowWorker: DemoMode.AutoFlowEnabled is false — skipping");
            return;
        }

        _logger.LogInformation("🎬 Demo: advancing GHN shipping flow...");

        // Find all non-terminal GHN shipments that belong to active orders
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
                (s.Order.Status!.StatusName == "Processing" || s.Order.Status.StatusName == "Shipped"))
            .ToListAsync();

        if (activeShippings.Count == 0)
        {
            _logger.LogInformation("Demo: no active shipments to advance");
            return;
        }

        _logger.LogInformation("Demo: advancing {Count} shipment(s)", activeShippings.Count);

        foreach (var shipping in activeShippings)
        {
            var nextStatus = GHNStatusMapper.GetNextDemoStatus(shipping.Status);
            if (nextStatus == null)
            {
                _logger.LogDebug(
                    "Demo: ShippingTx {Id} is at final demo step ({Status}) — skipping",
                    shipping.ShippingTransactionId, shipping.Status);
                continue;
            }

            _logger.LogInformation(
                "Demo: Order#{OrderId} (ShippingTx#{ShippingId}) {Old} → {New}",
                shipping.OrderId, shipping.ShippingTransactionId, shipping.Status, nextStatus);

            await _shippingStatusService.ProcessStatusUpdateAsync(
                providerOrderCode: shipping.ProviderOrderCode!,
                newGHNStatus: nextStatus,
                source: "Demo");

            // Small delay between orders to space out SignalR pushes
            await Task.Delay(200);
        }
    }

    private bool IsDemoModeEnabled()
        => _configuration.GetValue<bool>("DemoMode:AutoFlowEnabled");
}
