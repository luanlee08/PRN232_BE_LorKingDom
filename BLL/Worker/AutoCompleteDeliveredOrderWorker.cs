using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// Hangfire recurring job that auto-completes Delivered orders when the customer
    /// has not manually confirmed receipt within the configured window (default: 3 days).
    ///
    /// The transition timestamp is taken from OrderStatusHistory — specifically the most
    /// recent history entry where StatusId == Delivered.  No new DB column is required.
    ///
    /// Config: "Order:AutoCompleteDeliveredDays" (default: 3)
    /// </summary>
    public class AutoCompleteDeliveredOrderWorker
    {
        private readonly AspLorKingDomContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutoCompleteDeliveredOrderWorker> _logger;

        public AutoCompleteDeliveredOrderWorker(
            AspLorKingDomContext context,
            IConfiguration configuration,
            ILogger<AutoCompleteDeliveredOrderWorker> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task AutoCompleteDeliveredOrdersJob()
        {
            var days = _configuration.GetValue<int>("Order:AutoCompleteDeliveredDays", 3);
            var cutoff = DateTime.UtcNow.AddDays(-days);

            var deliveredStatus = await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == "Delivered");
            var completedStatus = await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == "Completed");

            if (deliveredStatus == null || completedStatus == null)
            {
                _logger.LogWarning("AutoCompleteDeliveredOrderWorker: Delivered or Completed status not found in DB.");
                return;
            }

            // Find orders that are:
            // - Currently in Delivered status
            // - Not deleted
            // - Whose most recent Delivered history entry was created before the cutoff
            var ordersToComplete = await _context.Orders
                .Where(o =>
                    !o.IsDeleted &&
                    o.StatusId == deliveredStatus.StatusId &&
                    _context.OrderStatusHistories
                        .Where(h => h.OrderId == o.OrderId && h.StatusId == deliveredStatus.StatusId)
                        .OrderByDescending(h => h.ChangedAt)
                        .Select(h => h.ChangedAt)
                        .FirstOrDefault() < cutoff)
                .ToListAsync();

            if (!ordersToComplete.Any())
                return;

            _logger.LogInformation(
                "AutoCompleteDeliveredOrderWorker: Found {Count} delivered orders to auto-complete.",
                ordersToComplete.Count);

            foreach (var order in ordersToComplete)
            {
                await using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    order.StatusId = completedStatus.StatusId;
                    order.UpdatedAt = DateTime.UtcNow;

                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        StatusId = completedStatus.StatusId,
                        ChangedAt = DateTime.UtcNow,
                        Note = $"Tự động hoàn thành sau {days} ngày giao hàng không xác nhận",
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    _logger.LogInformation(
                        "AutoCompleteDeliveredOrderWorker: Order {OrderId} auto-completed.", order.OrderId);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "AutoCompleteDeliveredOrderWorker: Failed to auto-complete order {OrderId}.", order.OrderId);
                }
            }
        }
    }
}
