using BLL.DTOs;
using BLL.DTOs.Orders;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        // Payment Methods
        Task<ApiResponse<GetPaymentMethodsResponse>> GetAvailablePaymentMethodsAsync();

        // Order Management
        Task<ApiResponse<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request, int accountId);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId, int accountId);
        Task<ApiResponse<PagedResult<OrderDto>>> GetMyOrdersAsync(int accountId, int pageNumber = 1, int pageSize = 10, string? statusFilter = null);
        Task<ApiResponse<PagedResult<OrderDto>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10, string? statusFilter = null);
        Task<ApiResponse<object>> CancelOrderAsync(int orderId, int accountId, string? reason = null);

        // Order Status Management (Admin)
        Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, int adminId);

        // Payment Processing
        Task<ApiResponse<object>> HandlePaymentWebhookAsync(string provider, string payload, string signature);
        Task<ApiResponse<object>> ConfirmCODPaymentAsync(int orderId, int shipperId);

        // Refund Management
        Task<ApiResponse<RefundDto>> CreateRefundRequestAsync(CreateRefundRequest request, int accountId);
        Task<ApiResponse<RefundDto>> ApproveRefundAsync(ApproveRefundRequest request, int adminId);
        Task<ApiResponse<PagedResult<RefundDto>>> GetRefundRequestsAsync(int pageNumber = 1, int pageSize = 10, string? statusFilter = null);
        Task<ApiResponse<RefundDto>> GetRefundByIdAsync(long refundId);
    }
}
