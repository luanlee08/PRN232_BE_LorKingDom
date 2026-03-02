using BLL.DTOs;
using BLL.DTOs.Statistics;

namespace BLL.Interfaces
{
    public interface IStatisticsService
    {
        /// <summary>
        /// Thống kê doanh thu – chỉ dành cho Admin.
        /// Chỉ tính đơn hàng ở trạng thái Completed.
        /// Doanh thu ròng = Gross – Refund đã hoàn.
        /// </summary>
        Task<ApiResponse<RevenueStatisticsResponse>> GetRevenueStatisticsAsync(RevenueStatisticsQuery query);

        /// <summary>
        /// Thống kê sản phẩm – Admin và Warehouse.
        /// Bán chạy dựa trên OrderDetails của đơn Completed.
        /// </summary>
        Task<ApiResponse<ProductStatisticsResponse>> GetProductStatisticsAsync(ProductStatisticsQuery query);
    }
}
