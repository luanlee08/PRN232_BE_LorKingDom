using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AspLorKingDomContext _context;

        public OrderRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        #region Order CRUD

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.IsDeleted);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Account)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.Status)
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.ChangedByNavigation)
                .Include(o => o.PaymentHistories)
                .Include(o => o.ShippingProviderTransactions)
                .Where(o => o.OrderId == orderId && !o.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Order?> GetByIdForAccountAsync(int orderId, int accountId)
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.Status)
                .Include(o => o.PaymentHistories)
                .Include(o => o.ShippingProviderTransactions)
                .Where(o => o.OrderId == orderId && o.AccountId == accountId && !o.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId, int skip, int take, string? statusFilter = null)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductImages)
                .Where(o => o.AccountId == accountId && !o.IsDeleted);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status.StatusName == statusFilter);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int skip, int take, string? statusFilter = null)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Account)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductImages)
                .Where(o => !o.IsDeleted);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status.StatusName == statusFilter);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetOrdersCountByAccountIdAsync(int accountId, string? statusFilter = null)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Where(o => o.AccountId == accountId && !o.IsDeleted);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status.StatusName == statusFilter);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalOrdersCountAsync(string? statusFilter = null)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Where(o => !o.IsDeleted);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(o => o.Status.StatusName == statusFilter);
            }

            return await query.CountAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                order.IsDeleted = true;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region OrderDetail

        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                    .ThenInclude(p => p.ProductImages)
                .Where(od => od.OrderId == orderId && !od.IsDeleted)
                .ToListAsync();
        }

        #endregion

        #region OrderStatusHistory

        public async Task AddOrderStatusHistoryAsync(OrderStatusHistory statusHistory)
        {
            await _context.OrderStatusHistories.AddAsync(statusHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderStatusHistory>> GetStatusHistoryByOrderIdAsync(int orderId)
        {
            return await _context.OrderStatusHistories
                .Include(osh => osh.Status)
                .Include(osh => osh.ChangedByNavigation)
                .Where(osh => osh.OrderId == orderId)
                .OrderByDescending(osh => osh.ChangedAt)
                .ToListAsync();
        }

        #endregion

        #region StatusOrder

        public async Task<StatusOrder?> GetStatusByNameAsync(string statusName)
        {
            return await _context.StatusOrders
                .FirstOrDefaultAsync(s => s.StatusName == statusName);
        }

        public async Task<StatusOrder?> GetStatusByIdAsync(int statusId)
        {
            return await _context.StatusOrders.FindAsync(statusId);
        }

        #endregion

        #region Refund

        public async Task<OrderRefund?> GetRefundByIdAsync(long refundId)
        {
            return await _context.OrderRefunds
                .Include(r => r.Order)
                .Include(r => r.Account)
                .Include(r => r.ApprovedByNavigation)
                .FirstOrDefaultAsync(r => r.RefundId == refundId);
        }

        public async Task<IEnumerable<OrderRefund>> GetRefundRequestsAsync(int skip, int take, string? statusFilter = null)
        {
            var query = _context.OrderRefunds
                .Include(r => r.Order)
                .Include(r => r.Account)
                .Include(r => r.ApprovedByNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(r => r.RefundStatus == statusFilter);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetRefundsCountAsync(string? statusFilter = null)
        {
            var query = _context.OrderRefunds.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(r => r.RefundStatus == statusFilter);
            }

            return await query.CountAsync();
        }

        public async Task<OrderRefund> CreateRefundAsync(OrderRefund refund)
        {
            await _context.OrderRefunds.AddAsync(refund);
            await _context.SaveChangesAsync();
            return refund;
        }

        public async Task UpdateRefundAsync(OrderRefund refund)
        {
            _context.OrderRefunds.Update(refund);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Payment

        public async Task<PaymentHistory> AddPaymentHistoryAsync(PaymentHistory paymentHistory)
        {
            await _context.PaymentHistories.AddAsync(paymentHistory);
            await _context.SaveChangesAsync();
            return paymentHistory;
        }

        public async Task<PaymentGatewayTransaction> AddPaymentGatewayTransactionAsync(PaymentGatewayTransaction transaction)
        {
            await _context.PaymentGatewayTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<PaymentGatewayTransaction?> GetPaymentGatewayTransactionByOrderIdAsync(int orderId, string provider)
        {
            return await _context.PaymentGatewayTransactions
                .Include(g => g.PaymentHistory)
                    .ThenInclude(p => p.Order)
                .FirstOrDefaultAsync(g => g.PaymentHistory.OrderId == orderId && g.Provider == provider);
        }

        public async Task UpdatePaymentGatewayTransactionAsync(PaymentGatewayTransaction transaction)
        {
            _context.PaymentGatewayTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Webhook

        public async Task<WebhookEvent> AddWebhookEventAsync(WebhookEvent webhookEvent)
        {
            await _context.WebhookEvents.AddAsync(webhookEvent);
            await _context.SaveChangesAsync();
            return webhookEvent;
        }

        public async Task UpdateWebhookEventAsync(WebhookEvent webhookEvent)
        {
            _context.WebhookEvents.Update(webhookEvent);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
