namespace BLL.DTOs.Statistics
{
    // ─── QUERY ───────────────────────────────────────────────────────
    public class RevenueStatisticsQuery
    {
        /// <summary>Granularity: "day" | "month" | "year"</summary>
        public string Period { get; set; } = "month";

        /// <summary>Start date (inclusive). Defaults to 12 months ago.</summary>
        public DateTime? From { get; set; }

        /// <summary>End date (inclusive). Defaults to today.</summary>
        public DateTime? To { get; set; }
    }

    public class ProductStatisticsQuery
    {
        public int TopN { get; set; } = 10;
        public int LowStockThreshold { get; set; } = 10;
    }

    // ─── REVENUE RESPONSE ────────────────────────────────────────────
    public class RevenueStatisticsResponse
    {
        // Tổng quan
        public int TotalCompletedOrders { get; set; }
        public decimal GrossRevenue { get; set; }          // Tổng TotalAmount của đơn Completed
        public decimal ShippingRevenue { get; set; }       // Tổng ShippingFee của đơn Completed
        public decimal RefundTotal { get; set; }           // Tổng tiền đã hoàn (OrderRefunds Completed)
        public decimal NetRevenue => GrossRevenue - RefundTotal;
        public decimal AvgOrderValue => TotalCompletedOrders > 0
            ? Math.Round(GrossRevenue / TotalCompletedOrders, 0) : 0;

        // Phân bổ theo trạng thái (tất cả đơn trong kỳ)
        public List<OrderStatusCount> OrdersByStatus { get; set; } = [];

        // Biểu đồ doanh thu theo kỳ
        public List<RevenueChartPoint> RevenueChart { get; set; } = [];

        // Phân bổ phương thức thanh toán
        public List<PaymentMethodBreakdown> PaymentMethods { get; set; } = [];

        // Top khách hàng
        public List<TopCustomerItem> TopCustomers { get; set; } = [];
    }

    public class OrderStatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class RevenueChartPoint
    {
        public string Label { get; set; } = string.Empty;  // "2026-02", "2026-02-28", "2026"
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class PaymentMethodBreakdown
    {
        public string Method { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }

    public class TopCustomerItem
    {
        public int AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    // ─── PRODUCT RESPONSE ────────────────────────────────────────────
    public class ProductStatisticsResponse
    {
        // Tổng quan kho
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int TotalStockQuantity { get; set; }

        // Sản phẩm bán chạy (từ đơn Completed)
        public List<TopSellingProduct> TopSellingProducts { get; set; } = [];

        // Sản phẩm tồn kho thấp
        public List<LowStockProduct> LowStockProducts { get; set; } = [];

        // Phân bổ theo danh mục
        public List<CategoryBreakdown> CategoryBreakdown { get; set; } = [];

        // Phân bổ theo thương hiệu
        public List<BrandBreakdown> BrandBreakdown { get; set; } = [];
    }

    public class TopSellingProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public int CurrentStock { get; set; }
    }

    public class LowStockProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public int Quantity { get; set; }
        public string ProductStatus { get; set; } = string.Empty;
    }

    public class CategoryBreakdown
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? SuperCategory { get; set; }
        public int ProductCount { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class BrandBreakdown
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
