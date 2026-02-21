using BLL.DTOs;
using BLL.DTOs.Orders;

namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for order refund operations
    /// </summary>
    public interface IOrderRefundService
    {
        /// <summary>
        /// Create refund request
        /// </summary>
        Task<OrderRefundDto> CreateRefundAsync(int orderId, CreateRefundRequest request);

        /// <summary>
        /// Approve or reject refund (admin)
        /// </summary>
        Task<OrderRefundDto> ProcessRefundAsync(int refundId, ProcessRefundRequest request);

        /// <summary>
        /// Get all refunds with filters (admin)
        /// </summary>
        Task<PagedResult<OrderRefundDto>> GetRefundsAsync(
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Get refund by ID
        /// </summary>
        Task<OrderRefundDto> GetRefundByIdAsync(int refundId);
    }
}
