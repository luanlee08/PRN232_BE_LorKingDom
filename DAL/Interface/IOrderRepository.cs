using DAL.Models;

namespace DAL.Interface
{
    public interface IOrderRepository
    {
        // Order CRUD
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order?> GetByIdWithDetailsAsync(int orderId);
        Task<Order?> GetByIdForAccountAsync(int orderId, int accountId);
        Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId, int skip, int take, string? statusFilter = null);
        Task<IEnumerable<Order>> GetAllOrdersAsync(int skip, int take, string? statusFilter = null);
        Task<int> GetOrdersCountByAccountIdAsync(int accountId, string? statusFilter = null);
        Task<int> GetTotalOrdersCountAsync(string? statusFilter = null);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);

        // OrderDetail
        Task AddOrderDetailAsync(OrderDetail orderDetail);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId);

        // OrderStatusHistory
        Task AddOrderStatusHistoryAsync(OrderStatusHistory statusHistory);
        Task<IEnumerable<OrderStatusHistory>> GetStatusHistoryByOrderIdAsync(int orderId);

        // StatusOrder
        Task<StatusOrder?> GetStatusByNameAsync(string statusName);
        Task<StatusOrder?> GetStatusByIdAsync(int statusId);

        // Refund
        Task<OrderRefund?> GetRefundByIdAsync(long refundId);
        Task<IEnumerable<OrderRefund>> GetRefundRequestsAsync(int skip, int take, string? statusFilter = null);
        Task<int> GetRefundsCountAsync(string? statusFilter = null);
        Task<OrderRefund> CreateRefundAsync(OrderRefund refund);
        Task UpdateRefundAsync(OrderRefund refund);

        // Payment
        Task<PaymentHistory> AddPaymentHistoryAsync(PaymentHistory paymentHistory);
        Task<PaymentGatewayTransaction> AddPaymentGatewayTransactionAsync(PaymentGatewayTransaction transaction);
        Task<PaymentGatewayTransaction?> GetPaymentGatewayTransactionByOrderIdAsync(int orderId, string provider);
        Task UpdatePaymentGatewayTransactionAsync(PaymentGatewayTransaction transaction);

        // Webhook
        Task<WebhookEvent> AddWebhookEventAsync(WebhookEvent webhookEvent);
        Task UpdateWebhookEventAsync(WebhookEvent webhookEvent);

        // Admin Order Management (from remote)
        Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize,
            string sortBy,
            bool sortDesc);

        Task UpdateStatusAsync(int orderId, int statusId);

        /// <summary>Get the first payment history record for an order+method combination.</summary>
        Task<PaymentHistory?> GetPaymentHistoryByOrderIdAndMethodAsync(int orderId, string paymentMethod);

        Task<List<Order>> GetOrdersForExportAsync(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            string sortBy,
            bool sortDesc,
            int maxRecords = 5000);
    }
}
