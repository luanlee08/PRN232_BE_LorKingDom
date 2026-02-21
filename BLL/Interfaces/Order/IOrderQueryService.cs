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
        /// Get current user's orders
        /// </summary>
        Task<PagedResult<OrderDto>> GetMyOrdersAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Get all orders (admin)
        /// </summary>
        Task<PagedResult<OrderDto>> GetAllOrdersAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Get orders with filters
        /// </summary>
        Task<List<OrderDto>> GetOrdersAsync(
            int? userId = null,
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null);

        /// <summary>
        /// Get order detail by ID
        /// </summary>
        Task<OrderDto> GetOrderDetailAsync(int orderId);
    }
}
