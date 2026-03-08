using BLL.DTOs;
using BLL.DTOs.Orders;

namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for Order query operations (read-only)
    /// </summary>
    public interface IOrderQueryService
    {
        /// <summary>
        /// Get order by ID
        /// </summary>
        Task<OrderDto> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Get order by ID for a specific account (validates ownership)
        /// </summary>
        Task<OrderDto> GetOrderByIdForAccountAsync(int orderId, int accountId);

        /// <summary>
        /// Get orders for a specific account (customer view)
        /// </summary>
        Task<PagedResult<OrderDto>> GetMyOrdersAsync(
            int accountId,
            int pageNumber = 1,
            int pageSize = 10,
            string? statusFilter = null);

        /// <summary>
        /// Get all orders (admin, simple filter)
        /// </summary>
        Task<PagedResult<OrderDto>> GetAllOrdersAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? statusFilter = null);

        /// <summary>
        /// Get admin orders with full filtering/sorting (returns OrderResponse)
        /// </summary>
        Task<PagedResult<OrderResponse>> GetAdminOrdersPagedAsync(OrderQuery query);

        /// <summary>
        /// Get admin order detail (returns OrderDetailResponse)
        /// </summary>
        Task<OrderDetailResponse> GetAdminOrderDetailAsync(int orderId);
    }
}
