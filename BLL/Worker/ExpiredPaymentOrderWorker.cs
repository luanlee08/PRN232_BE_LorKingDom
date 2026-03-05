using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// Hangfire recurring job that auto-cancels Pending orders whose external payment
    /// (VNPay, MoMo, Sepay) was never confirmed within the expiry window.
    ///
    /// This handles the case where a user creates an order, receives a payment URL,
    /// then closes the tab or the payment gateway times out without sending a webhook.
    ///
    /// COD and Wallet orders are never touched — only orders where PaymentMethod is
    /// an external gateway AND PaymentStatus is still Pending after the timeout.
    ///
    /// Config: "Payment:ExpiryMinutes" (default: 15)
    /// </summary>
    public class ExpiredPaymentOrderWorker
    {
        private readonly AspLorKingDomContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExpiredPaymentOrderWorker> _logger;

        private static readonly HashSet<string> ExternalProviders =
            new(StringComparer.OrdinalIgnoreCase) { "VNPay", "MoMo", "Sepay" };

        public ExpiredPaymentOrderWorker(
            AspLorKingDomContext context,
            IConfiguration configuration,
            ILogger<ExpiredPaymentOrderWorker> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task CancelExpiredPaymentOrdersJob()
        {
            var expiryMinutes = _configuration.GetValue<int>("Payment:ExpiryMinutes", 15);
            var cutoff = DateTime.UtcNow.AddMinutes(-expiryMinutes);

            // Find orders that are:
            // - Still Pending status
            // - Have an external payment history that is still Pending
            // - Created before the cutoff time
            var pendingStatus = await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == "Pending");
            var cancelledStatus = await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == "Cancelled");

            if (pendingStatus == null || cancelledStatus == null)
            {
                _logger.LogWarning("ExpiredPaymentOrderWorker: Pending or Cancelled status not found in DB.");
                return;
            }

            var expiredOrders = await _context.Orders
                .Include(o => o.PaymentHistories)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o =>
                    !o.IsDeleted &&
                    o.StatusId == pendingStatus.StatusId &&
                    o.CreatedAt < cutoff &&
                    o.PaymentCompletedAt == null &&
                    o.PaymentHistories.Any(ph =>
                        ExternalProviders.Contains(ph.PaymentMethod) &&
                        ph.PaymentStatus == "Pending"))
                .ToListAsync();

            if (!expiredOrders.Any())
                return;

            _logger.LogInformation("ExpiredPaymentOrderWorker: Found {Count} expired unpaid orders to cancel.", expiredOrders.Count);

            foreach (var order in expiredOrders)
            {
                await using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Mark payment histories as expired
                    foreach (var ph in order.PaymentHistories
                        .Where(ph => ExternalProviders.Contains(ph.PaymentMethod) && ph.PaymentStatus == "Pending"))
                    {
                        ph.PaymentStatus = "Expired";
                    }

                    // Restore product stock
                    foreach (var detail in order.OrderDetails)
                    {
                        if (detail.Product != null)
                            detail.Product.Quantity += detail.Quantity;
                    }

                    // Cancel order
                    order.StatusId = cancelledStatus.StatusId;
                    order.UpdatedAt = DateTime.UtcNow;

                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        StatusId = cancelledStatus.StatusId,
                        ChangedAt = DateTime.UtcNow,
                        Note = $"Tự động hủy do quá thời gian thanh toán ({expiryMinutes} phút)",
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    _logger.LogInformation("ExpiredPaymentOrderWorker: Auto-cancelled order {OrderId} (created {CreatedAt:u}).",
                        order.OrderId, order.CreatedAt);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "ExpiredPaymentOrderWorker: Failed to cancel order {OrderId}.", order.OrderId);
                }
            }
        }
    }
}
