using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// Hangfire recurring job that auto-expires Pending wallet top-up transactions
    /// where the user never completed payment (e.g. pressed Back in VNPay without
    /// cancelling, closed the browser tab, or the gateway timed out silently).
    ///
    /// Config: "Payment:TopUpExpiryMinutes" (default: 30)
    /// </summary>
    public class ExpiredPendingTopUpWorker
    {
        private readonly AspLorKingDomContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExpiredPendingTopUpWorker> _logger;

        public ExpiredPendingTopUpWorker(
            AspLorKingDomContext context,
            IConfiguration configuration,
            ILogger<ExpiredPendingTopUpWorker> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task ExpirePendingTopUpsJob()
        {
            var expiryMinutes = _configuration.GetValue<int>("Payment:TopUpExpiryMinutes", 30);
            var cutoff = DateTime.UtcNow.AddMinutes(-expiryMinutes);

            var pendingTopUps = await _context.WalletTransactions
                .Where(t =>
                    t.TxnType == "TopUp" &&
                    t.Status == "Pending" &&
                    t.CreatedAt < cutoff)
                .ToListAsync();

            if (!pendingTopUps.Any())
                return;

            _logger.LogInformation(
                "ExpiredPendingTopUpWorker: Found {Count} pending top-up transactions to expire.",
                pendingTopUps.Count);

            foreach (var txn in pendingTopUps)
            {
                txn.Status = "Failed";
                txn.Reason = "Hết thời gian chờ thanh toán";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "ExpiredPendingTopUpWorker: Expired {Count} pending top-up transactions.",
                pendingTopUps.Count);
        }
    }
}
