using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.DTOs.Statistics;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderModel = DAL.Models.Order;

namespace BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AspLorKingDomContext _context;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(AspLorKingDomContext context, ILogger<StatisticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<RevenueStatisticsResponse>> GetRevenueStatisticsAsync(RevenueStatisticsQuery query)
        {
            try
            {
                var to = (query.To ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
                var from = (query.From ?? DateTime.UtcNow.AddMonths(-11)).Date;

                // Pull completed orders into memory for flexible grouping
                var completedOrders = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.Account)
                    .Where(o => !o.IsDeleted
                        && o.Status.StatusName == OrderStatusNames.Completed
                        && o.UpdatedAt != null
                        && o.UpdatedAt >= from && o.UpdatedAt <= to)
                    .AsNoTracking()
                    .ToListAsync();

                var grossRevenue = completedOrders.Sum(o => o.TotalAmount);
                var shippingRevenue = completedOrders.Sum(o => o.ShippingFee);

                var refundTotal = await _context.OrderRefunds
                    .Where(r => r.RefundStatus == RefundStatus.Completed
                        && r.ProcessedAt >= from && r.ProcessedAt <= to)
                    .SumAsync(r => (decimal?)r.RefundAmount) ?? 0;

                // Status counts — group by StatusName (EF can translate nav prop access in GroupBy key)
                var statusGroups = await _context.Orders
                    .Include(o => o.Status)
                    .Where(o => !o.IsDeleted && o.CreatedAt >= from && o.CreatedAt <= to)
                    .GroupBy(o => o.Status.StatusName)
                    .Select(g => new OrderStatusCount
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        TotalAmount = g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync();

                var revenueChart = BuildRevenueChart(completedOrders, query.Period, from, to);

                // Use Order.PaymentCompletedAt for date filtering — this correctly captures
                // when COD payments were collected (Delivered) and when online payments settled.
                var paymentMethods = await _context.PaymentHistories
                    .Where(p => p.PaymentStatus == PaymentStatus.Success
                        && p.Order.PaymentCompletedAt != null
                        && p.Order.PaymentCompletedAt >= from
                        && p.Order.PaymentCompletedAt <= to)
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new PaymentMethodBreakdown
                    {
                        Method = g.Key,
                        Count = g.Count(),
                        Amount = g.Sum(p => p.Amount)
                    })
                    .ToListAsync();

                // Top customers — in-memory from already-loaded completed orders
                var topCustomers = completedOrders
                    .GroupBy(o => new { o.AccountId, o.Account.AccountName, o.Account.Email })
                    .Select(g => new TopCustomerItem
                    {
                        AccountId = g.Key.AccountId,
                        Name = g.Key.AccountName,
                        Email = g.Key.Email,
                        OrderCount = g.Count(),
                        TotalSpent = g.Sum(o => o.TotalAmount)
                    })
                    .OrderByDescending(c => c.TotalSpent)
                    .Take(10)
                    .ToList();

                return new ApiResponse<RevenueStatisticsResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy thống kê doanh thu thành công",
                    Data = new RevenueStatisticsResponse
                    {
                        TotalCompletedOrders = completedOrders.Count,
                        GrossRevenue = grossRevenue,
                        ShippingRevenue = shippingRevenue,
                        RefundTotal = refundTotal,
                        OrdersByStatus = statusGroups,
                        RevenueChart = revenueChart,
                        PaymentMethods = paymentMethods,
                        TopCustomers = topCustomers
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue statistics");
                return new ApiResponse<RevenueStatisticsResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        private static List<RevenueChartPoint> BuildRevenueChart(
            List<OrderModel> completedOrders, string? period, DateTime from, DateTime to)
        {
            var result = new List<RevenueChartPoint>();

            if (period == "day")
            {
                for (var d = from.Date; d <= to.Date && d <= DateTime.UtcNow.Date; d = d.AddDays(1))
                {
                    var dayOrders = completedOrders
                        .Where(o => o.UpdatedAt.HasValue && o.UpdatedAt.Value.Date == d)
                        .ToList();
                    result.Add(new RevenueChartPoint
                    {
                        Label = d.ToString("yyyy-MM-dd"),
                        Revenue = dayOrders.Sum(o => o.TotalAmount),
                        Orders = dayOrders.Count
                    });
                }
            }
            else if (period == "year")
            {
                foreach (var g in completedOrders
                    .Where(o => o.UpdatedAt.HasValue)
                    .GroupBy(o => o.UpdatedAt!.Value.Year)
                    .OrderBy(g => g.Key))
                {
                    result.Add(new RevenueChartPoint
                    {
                        Label = g.Key.ToString(),
                        Revenue = g.Sum(o => o.TotalAmount),
                        Orders = g.Count()
                    });
                }
            }
            else // month (default)
            {
                var cur = new DateTime(from.Year, from.Month, 1);
                var limit = new DateTime(to.Year, to.Month, 1);
                while (cur <= limit)
                {
                    var mo = completedOrders
                        .Where(o => o.UpdatedAt.HasValue
                            && o.UpdatedAt.Value.Year == cur.Year
                            && o.UpdatedAt.Value.Month == cur.Month)
                        .ToList();
                    result.Add(new RevenueChartPoint
                    {
                        Label = cur.ToString("yyyy-MM"),
                        Revenue = mo.Sum(o => o.TotalAmount),
                        Orders = mo.Count
                    });
                    cur = cur.AddMonths(1);
                }
            }

            return result;
        }

        public async Task<ApiResponse<ProductStatisticsResponse>> GetProductStatisticsAsync(ProductStatisticsQuery query)
        {
            try
            {
                var threshold = Math.Max(1, query.LowStockThreshold);
                var topN = query.TopN > 0 ? query.TopN : 10;

                // All products for summary counts
                var allProducts = await _context.Products
                    .Where(p => !p.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                // Sold items for completed orders — scalar projections only (EF translatable)
                var soldItems = await _context.OrderDetails
                    .Where(od => !od.IsDeleted
                        && !od.Order.IsDeleted
                        && od.Order.Status.StatusName == OrderStatusNames.Completed)
                    .Select(od => new
                    {
                        od.ProductId,
                        od.Quantity,
                        Revenue = od.Total ?? (od.UnitPrice * od.Quantity)
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var soldByProduct = soldItems
                    .GroupBy(x => x.ProductId)
                    .ToDictionary(
                        g => g.Key,
                        g => (qty: g.Sum(x => x.Quantity), rev: g.Sum(x => x.Revenue)));

                var productIds = soldByProduct.Keys.ToList();

                // Product details for top-selling (with nav props)
                var productInfos = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Include(p => p.ProductImages.Where(img => img.IsMain))
                    .Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                // Ratings — simple group by scalar ProductId in DB
                var ratingData = await _context.ReviewProducts
                    .Where(r => productIds.Contains(r.ProductId)
                        && !r.IsDeleted
                        && r.Status == "Approved")
                    .GroupBy(r => r.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Avg = g.Average(r => (double)r.Rating),
                        Count = g.Count()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var ratingDict = ratingData.ToDictionary(r => r.ProductId);

                // Top selling — in-memory sort
                var topSelling = productInfos
                    .OrderByDescending(p => soldByProduct.TryGetValue(p.ProductId, out var s) ? s.qty : 0)
                    .Take(topN)
                    .Select(p =>
                    {
                        soldByProduct.TryGetValue(p.ProductId, out var sold);
                        ratingDict.TryGetValue(p.ProductId, out var rating);
                        return new TopSellingProduct
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Category = p.Category?.CategoryName,
                            Brand = p.Brand?.BrandName,
                            ImageUrl = p.ProductImages.FirstOrDefault()?.ImageUrl,
                            QuantitySold = sold.qty,
                            TotalRevenue = sold.rev,
                            AvgRating = rating != null ? Math.Round((decimal)rating.Avg, 1) : 0,
                            ReviewCount = rating?.Count ?? 0,
                            CurrentStock = p.Quantity
                        };
                    })
                    .ToList();

                // Low stock — direct DB query with scalar Select  
                var lowStockProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Where(p => !p.IsDeleted && p.Quantity <= threshold)
                    .OrderBy(p => p.Quantity)
                    .Take(50)
                    .AsNoTracking()
                    .Select(p => new LowStockProduct
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Category = p.Category != null ? p.Category.CategoryName : null,
                        Brand = p.Brand != null ? p.Brand.BrandName : null,
                        Quantity = p.Quantity,
                        ProductStatus = p.ProductStatus
                    })
                    .ToListAsync();

                // Category breakdown — in-memory from productInfos
                var categoryBreakdown = productInfos
                    .Where(p => p.Category != null)
                    .GroupBy(p => new { p.CategoryId, p.Category!.CategoryName })
                    .Select(g =>
                    {
                        var ids = g.Select(p => p.ProductId).ToList();
                        return new CategoryBreakdown
                        {
                            CategoryId = g.Key.CategoryId ?? 0,
                            CategoryName = g.Key.CategoryName,
                            ProductCount = g.Count(),
                            TotalSold = ids.Sum(id => soldByProduct.TryGetValue(id, out var s) ? s.qty : 0),
                            TotalRevenue = ids.Sum(id => soldByProduct.TryGetValue(id, out var s) ? s.rev : 0)
                        };
                    })
                    .OrderByDescending(c => c.TotalSold)
                    .ToList();

                // Brand breakdown — in-memory from productInfos
                var brandBreakdown = productInfos
                    .Where(p => p.Brand != null)
                    .GroupBy(p => new { p.BrandId, p.Brand!.BrandName })
                    .Select(g =>
                    {
                        var ids = g.Select(p => p.ProductId).ToList();
                        return new BrandBreakdown
                        {
                            BrandId = g.Key.BrandId ?? 0,
                            BrandName = g.Key.BrandName,
                            ProductCount = g.Count(),
                            TotalSold = ids.Sum(id => soldByProduct.TryGetValue(id, out var s) ? s.qty : 0),
                            TotalRevenue = ids.Sum(id => soldByProduct.TryGetValue(id, out var s) ? s.rev : 0)
                        };
                    })
                    .OrderByDescending(b => b.TotalSold)
                    .ToList();

                return new ApiResponse<ProductStatisticsResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy thống kê sản phẩm thành công",
                    Data = new ProductStatisticsResponse
                    {
                        TotalProducts = allProducts.Count,
                        ActiveProducts = allProducts.Count(p => p.ProductStatus == "Active"),
                        InactiveProducts = allProducts.Count(p => p.ProductStatus != "Active"),
                        OutOfStockCount = allProducts.Count(p => p.Quantity == 0),
                        LowStockCount = allProducts.Count(p => p.Quantity > 0 && p.Quantity <= threshold),
                        TotalStockQuantity = allProducts.Sum(p => p.Quantity),
                        TopSellingProducts = topSelling,
                        LowStockProducts = lowStockProducts,
                        CategoryBreakdown = categoryBreakdown,
                        BrandBreakdown = brandBreakdown
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return new ApiResponse<ProductStatisticsResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }
    }
}
