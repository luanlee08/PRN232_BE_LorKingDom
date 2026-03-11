using BLL.DTOs.Notifications;
using BLL.Helpers.Notification;
using BLL.Interfaces.Notification;
using DAL.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Worker
{
    /// <summary>
    /// Hangfire recurring job that detects products whose stock quantity has dropped
    /// at or below the configured threshold and sends a real-time notification to Admin.
    ///
    /// De-duplication: a product will not be re-notified within <c>LowStock:CooldownHours</c>
    /// (default 24h) to avoid spamming the admin inbox when stock stays low.
    ///
    /// Config keys (appsettings.json):
    ///   "LowStock:Threshold"      — stock level that triggers the alert (default: 10)
    ///   "LowStock:CooldownHours"  — hours before the same product triggers another alert (default: 24)
    /// </summary>
    public class LowStockNotificationWorker
    {
        private readonly AspLorKingDomContext _context;
        private readonly INotificationCommandService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LowStockNotificationWorker> _logger;

        public LowStockNotificationWorker(
            AspLorKingDomContext context,
            INotificationCommandService notificationService,
            IConfiguration configuration,
            ILogger<LowStockNotificationWorker> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _configuration = configuration;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 2)]
        public async Task CheckLowStockJob()
        {
            var threshold = _configuration.GetValue<int>("LowStock:Threshold", 10);
            var cooldownHours = _configuration.GetValue<int>("LowStock:CooldownHours", 24);
            var cooldownCutoff = DateTime.UtcNow.AddHours(-cooldownHours);

            // 1. Find products with low (but non-zero) stock
            var lowStockProducts = await _context.Products
                .Where(p => !p.IsDeleted && p.Quantity > 0 && p.Quantity <= threshold)
                .AsNoTracking()
                .Select(p => new { p.ProductId, p.ProductName, p.Quantity })
                .ToListAsync();

            if (!lowStockProducts.Any())
            {
                _logger.LogDebug("LowStockNotificationWorker: No low-stock products found (threshold={Threshold}).", threshold);
                return;
            }

            _logger.LogInformation(
                "LowStockNotificationWorker: {Count} low-stock products found (threshold={Threshold}).",
                lowStockProducts.Count, threshold);

            // 2. Build set of product IDs already notified within cooldown window
            //    We identify low-stock deliveries by TemplateCode = "LOW_STOCK".
            var lowStockProductIds = lowStockProducts.Select(p => p.ProductId).ToList();

            var recentlyNotifiedProductIds = await _context.Deliveries
                .Where(d =>
                    d.TemplateCode == NotificationConstants.SystemOnlyTemplateCodes.LowStock &&
                    d.CreatedAt >= cooldownCutoff &&
                    lowStockProductIds.Any(id => d.ActionTarget == $"/admin/products/{id}"))
                .Select(d => d.ActionTarget)
                .Distinct()
                .ToListAsync();

            var alreadyNotified = recentlyNotifiedProductIds
                .Where(t => t != null)
                .Select(t => t!)
                .ToHashSet();

            // 3. Send notifications for products NOT in the cooldown set
            var toNotify = lowStockProducts
                .Where(p => !alreadyNotified.Contains($"/admin/products/{p.ProductId}"))
                .ToList();

            if (!toNotify.Any())
            {
                _logger.LogDebug("LowStockNotificationWorker: All low-stock products were notified recently — skipping.");
                return;
            }

            _logger.LogInformation(
                "LowStockNotificationWorker: Sending low-stock alerts for {Count} product(s).",
                toNotify.Count);

            foreach (var product in toNotify)
            {
                try
                {
                    await _notificationService.SendNotificationAsync(
                        new SendNotificationRequest
                        {
                            Title = $"Sản phẩm sắp hết hàng: {product.ProductName}",
                            Message = $"Tồn kho của \"{product.ProductName}\" chỉ còn {product.Quantity} sản phẩm (ngưỡng cảnh báo: {threshold}). Vui lòng nhập thêm hàng.",
                            TemplateCode = NotificationConstants.SystemOnlyTemplateCodes.LowStock,
                            TargetType = NotificationConstants.TargetTypes.Role,
                            TargetRoleId = 1, // Admin role
                            ActionType = "url",
                            ActionTarget = $"/admin/products/{product.ProductId}",
                            Payload = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                type = "low_stock",
                                productId = product.ProductId,
                                productName = product.ProductName,
                                quantity = product.Quantity,
                                threshold
                            })
                        },
                        createdByAccountId: 0,
                        isSystemGenerated: true
                    );

                    _logger.LogInformation(
                        "LowStockNotificationWorker: Alert sent for product {ProductId} ({ProductName}, qty={Qty}).",
                        product.ProductId, product.ProductName, product.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "LowStockNotificationWorker: Failed to send alert for product {ProductId}.",
                        product.ProductId);
                }
            }
        }
    }
}
