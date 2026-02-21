using BLL.DTOs.Orders;

namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for Order command operations (write)
    /// </summary>
    public interface IOrderCommandService
    {
        /// <summary>
        /// Create new order
        /// </summary>
        Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, string ipAddress);

        /// <summary>
        /// Cancel order
        /// </summary>
        Task CancelOrderAsync(int orderId, CancelOrderRequest request);

        /// <summary>
        /// Update order status (admin)
        /// </summary>
        Task UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);

        /// <summary>
        /// Confirm COD payment received
        /// </summary>
        Task ConfirmCODPaymentAsync(int orderId);
    }
}
