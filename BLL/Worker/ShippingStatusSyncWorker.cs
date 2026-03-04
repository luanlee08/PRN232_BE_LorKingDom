using BLL.DTOs.Shipping;
using BLL.Interfaces;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{

    public class ShippingStatusSyncWorker
    {
        private readonly AspLorKingDomContext _context;
        private readonly IGHNService _ghnService;
        private readonly IOrderService _orderService;
        private readonly ILogger<ShippingStatusSyncWorker> _logger;

        public ShippingStatusSyncWorker(
            AspLorKingDomContext context,
            IGHNService ghnService,
            IOrderService orderService,
            ILogger<ShippingStatusSyncWorker> logger)
        {
            _context = context;
            _ghnService = ghnService;
            _orderService = orderService;
            _logger = logger;
        }


        [AutomaticRetry(Attempts = 3)]
        public async Task SyncGHNShippingStatusJob()
        {
            _logger.LogInformation("🔄 Starting GHN shipping status sync...");

            try
            {
                // 1. Get active shipping orders (not delivered, cancelled, or returned)
                var activeShippings = await GetActiveShippingsAsync();

                if (activeShippings.Count == 0)
                {
                    _logger.LogInformation("No active GHN shipments to sync");
                    return;
                }

                _logger.LogInformation($"Found {activeShippings.Count} active GHN shipments to sync");

                int totalChecked = 0;
                int updated = 0;
                int errors = 0;
                var errorMessages = new List<string>();

                // 2. Process each shipping
                foreach (var shipping in activeShippings)
                {
                    totalChecked++;

                    try
                    {
                        var result = await SyncSingleShippingAsync(shipping);

                        if (result.Success && result.StatusUpdated)
                        {
                            updated++;
                            _logger.LogInformation(
                                $"📦 Order {shipping.Order.OrderId} (Shipping {shipping.ShippingTransactionId}): {result.OldStatus} → {result.NewStatus}");
                        }
                        else if (!result.Success)
                        {
                            errors++;
                            errorMessages.Add($"Shipping {shipping.ShippingTransactionId}: {result.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        errorMessages.Add($"Shipping {shipping.ShippingTransactionId}: {ex.Message}");
                        _logger.LogError(ex,
                            $"Error syncing shipping {shipping.ShippingTransactionId} (Order {shipping.Order.OrderId})");
                    }

                    // Small delay to avoid rate limiting
                    await Task.Delay(500);
                }

                // 3. Log summary
                _logger.LogInformation(
                    $"✅ GHN sync completed: {totalChecked} checked, {updated} updated, {errors} errors");

                if (errorMessages.Any())
                {
                    _logger.LogWarning($"Errors: {string.Join("; ", errorMessages)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GHN shipping status sync failed");
                throw; // Let Hangfire handle retry
            }
        }


        private async Task<List<ShippingProviderTransaction>> GetActiveShippingsAsync()
        {
            return await _context.ShippingProviderTransactions
                .Include(s => s.Order)
                    .ThenInclude(o => o.Status)
                .Where(s => s.Provider == "GHN" &&
                           s.ProviderOrderCode != null &&
                           s.Status != "delivered" &&
                           s.Status != "returned" &&
                           s.Status != "cancelled" &&
                           s.Status != "exception" &&
                           (s.Order.Status.StatusName == "Processing" ||
                            s.Order.Status.StatusName == "Shipped"))
                .OrderBy(s => s.CreatedAt) // Oldest first
                .ToListAsync();
        }


        private async Task<ShippingSyncResult> SyncSingleShippingAsync(ShippingProviderTransaction shipping)
        {
            var result = new ShippingSyncResult
            {
                Success = false,
                OldStatus = shipping.Status,
                SyncedAt = DateTime.UtcNow
            };

            try
            {
                // 1. Call GHN API to get current status
                var statusResponse = await _ghnService.GetOrderStatusAsync(shipping.ProviderOrderCode!);

                if (statusResponse.Code != 200 || statusResponse.Data == null)
                {
                    result.Message = $"GHN API error: {statusResponse.Message}";
                    return result;
                }

                var ghnStatus = statusResponse.Data.Status;
                result.NewStatus = ghnStatus;
                result.StatusText = statusResponse.Data.StatusText;

                // 2. Check if status changed
                if (shipping.Status == ghnStatus)
                {
                    result.Success = true;
                    result.StatusUpdated = false;
                    result.Message = "Status unchanged";
                    return result;
                }

                // 3. Status changed - simulate webhook to update order
                var webhookData = new GHNWebhookRequest
                {
                    Type = "Update Order Status",
                    Data = new GHNWebhookPayload
                    {
                        OrderCode = shipping.ProviderOrderCode!,
                        Status = ghnStatus,
                        StatusText = statusResponse.Data.StatusText,
                        ClientOrderCode = $"ORD{shipping.Order.OrderId:D6}",
                        Fee = statusResponse.Data.Fee,
                        Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                };

                // 4. Call webhook handler to update status and send notifications
                var updateResult = await _orderService.HandleShippingWebhookAsync("GHN", webhookData);

                if (updateResult.Status == 200)
                {
                    result.Success = true;
                    result.StatusUpdated = true;
                    result.Message = "Status updated successfully";
                }
                else
                {
                    result.Success = false;
                    result.Message = updateResult.Message;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Message = $"Exception: {ex.Message}";
                return result;
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

            return await SyncSingleShippingAsync(shipping);
        }

        public async Task<ShippingSyncResult> SyncShippingByOrderIdAsync(int orderId)
        {
            var shipping = await _context.ShippingProviderTransactions
                .Include(s => s.Order)
                    .ThenInclude(o => o.Status)
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

            return await SyncShippingByIdAsync(shipping.ShippingTransactionId);
        }
    }
}
