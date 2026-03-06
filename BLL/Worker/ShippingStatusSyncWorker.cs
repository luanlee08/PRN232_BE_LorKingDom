using BLL.DTOs.Shipping;
using BLL.Interfaces;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// Hangfire recurring job that polls GHN API every 5 minutes for active shipments.
    /// All business logic is delegated to IGHNShippingStatusService — this class is a
    /// thin scheduler wrapper. SyncShippingByIdAsync / SyncShippingByOrderIdAsync are
    /// kept for the Admin manual-sync API endpoints.
    ///
    /// To switch from polling to webhook: set "Shipping:GHNPollingEnabled": false in
    /// appsettings.json. The job becomes a no-op without any code change.
    /// </summary>
    public class ShippingStatusSyncWorker
    {
        private readonly IGHNShippingStatusService _shippingStatusService;
        private readonly AspLorKingDomContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShippingStatusSyncWorker> _logger;

        public ShippingStatusSyncWorker(
            IGHNShippingStatusService shippingStatusService,
            AspLorKingDomContext context,
            IConfiguration configuration,
            ILogger<ShippingStatusSyncWorker> logger)
        {
            _shippingStatusService = shippingStatusService;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task SyncGHNShippingStatusJob()
        {
            // Feature-flag: disable polling when running with real webhooks
            if (!_configuration.GetValue<bool>("Shipping:GHNPollingEnabled", defaultValue: true))
            {
                _logger.LogInformation("GHN polling disabled via Shipping:GHNPollingEnabled — skipping");
                return;
            }

            _logger.LogInformation("🔄 Starting GHN shipping status sync...");

            try
            {
                var batch = await _shippingStatusService.SyncActiveShipmentsAsync();

                _logger.LogInformation(
                    "✅ GHN sync completed: {Checked} checked, {Updated} updated, {Errors} errors",
                    batch.TotalChecked, batch.Updated, batch.Errors);

                if (batch.ErrorMessages.Any())
                    _logger.LogWarning("GHN sync errors: {Errors}", string.Join("; ", batch.ErrorMessages));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GHN shipping status sync failed");
                throw; // Let Hangfire handle retry
            }
        }

        public async Task<ShippingSyncResult> SyncShippingByIdAsync(long shippingId)
        {
            var shipping = await _context.ShippingProviderTransactions
                .Include(s => s.Order)
                    .ThenInclude(o => o.Status)
                .FirstOrDefaultAsync(s => s.ShippingTransactionId == shippingId);

            if (shipping == null)
            {
                return new ShippingSyncResult
                {
                    Success = false,
                    Message = "Shipping not found",
                    SyncedAt = DateTime.UtcNow
                };
            }

            if (shipping.Provider != "GHN")
            {
                return new ShippingSyncResult
                {
                    Success = false,
                    Message = "Only GHN shipments are supported",
                    SyncedAt = DateTime.UtcNow
                };
            }

            if (string.IsNullOrEmpty(shipping.ProviderOrderCode))
            {
                return new ShippingSyncResult
                {
                    Success = false,
                    Message = "Shipping has no GHN order code",
                    SyncedAt = DateTime.UtcNow
                };
            }

            // Delegate to service — contains all business logic
            return await _shippingStatusService.SyncFromGHNApiAsync(shippingId);
        }

        public async Task<ShippingSyncResult> SyncShippingByOrderIdAsync(int orderId)
        {
            var shipping = await _context.ShippingProviderTransactions
                .FirstOrDefaultAsync(s => s.OrderId == orderId && s.Provider == "GHN");

            if (shipping == null)
            {
                return new ShippingSyncResult
                {
                    Success = false,
                    Message = "No GHN shipping found for this order",
                    SyncedAt = DateTime.UtcNow
                };
            }

            return await _shippingStatusService.SyncFromGHNApiAsync(shipping.ShippingTransactionId);
        }
    }
}
