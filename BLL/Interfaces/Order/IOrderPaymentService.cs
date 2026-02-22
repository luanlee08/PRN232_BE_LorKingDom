using BLL.DTOs.Orders;

namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for payment-related operations
    /// </summary>
    public interface IOrderPaymentService
    {
        /// <summary>
        /// Get available payment methods based on order total
        /// </summary>
        Task<List<PaymentMethodInfo>> GetPaymentMethodsAsync(decimal orderTotal);

        /// <summary>
        /// Process wallet payment
        /// </summary>
        Task<PaymentResult> ProcessWalletPaymentAsync(int orderId, int userId);

        /// <summary>
        /// Generate payment URL for gateway providers
        /// </summary>
        Task<string> GeneratePaymentUrlAsync(
            int orderId,
            string paymentMethod,
            string baseUrl,
            string ipAddress);

        /// <summary>
        /// Handle payment callback/return
        /// </summary>
        Task<PaymentCallbackResult> HandlePaymentCallbackAsync(
            string provider,
            Dictionary<string, string> queryParams);
    }
}
