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
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.Status)
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.ChangedByNavigation)
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
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.Status)
                .Include(o => o.OrderStatusHistories)
                    .ThenInclude(osh => osh.ChangedByNavigation)
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

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Status)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId && !od.IsDeleted);
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

        #region Admin Order Management

        public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize,
            string sortBy,
            bool sortDesc)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Account)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductImages)
                .Where(o => !o.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(keyword) ||
                    (o.ShippingName != null && o.ShippingName.Contains(keyword)) ||
                    (o.Account.Email != null && o.Account.Email.Contains(keyword)) ||
                    (o.ShippingPhone != null && o.ShippingPhone.Contains(keyword)));
            }

            if (statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= toDate.Value);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "orderid" => sortDesc ? query.OrderByDescending(o => o.OrderId) : query.OrderBy(o => o.OrderId),
                "shippingname" => sortDesc ? query.OrderByDescending(o => o.ShippingName) : query.OrderBy(o => o.ShippingName),
                "totalamount" => sortDesc ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                "createdat" or "orderdate" => sortDesc ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
                _ => sortDesc ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task UpdateStatusAsync(int orderId, int statusId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null && !order.IsDeleted)
            {
                order.StatusId = statusId;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PaymentHistory?> GetPaymentHistoryByOrderIdAndMethodAsync(int orderId, string paymentMethod)
        {
            return await _context.PaymentHistories
                .FirstOrDefaultAsync(ph => ph.OrderId == orderId && ph.PaymentMethod == paymentMethod);
        }

        public async Task<List<Order>> GetOrdersForExportAsync(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            string sortBy,
            bool sortDesc,
            int maxRecords = 5000)
        {
            var query = _context.Orders
                .Include(o => o.Status)
                .Include(o => o.Account)
                .Include(o => o.OrderDetails.Where(od => !od.IsDeleted))
                    .ThenInclude(od => od.Product)
                .Where(o => !o.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(o =>
                    o.OrderId.ToString().Contains(keyword) ||
                    (o.ShippingName != null && o.ShippingName.Contains(keyword)) ||
                    (o.Account.Email != null && o.Account.Email.Contains(keyword)) ||
                    (o.ShippingPhone != null && o.ShippingPhone.Contains(keyword)));
            }

            if (statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= toDate.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "orderid" => sortDesc ? query.OrderByDescending(o => o.OrderId) : query.OrderBy(o => o.OrderId),
                "shippingname" => sortDesc ? query.OrderByDescending(o => o.ShippingName) : query.OrderBy(o => o.ShippingName),
                "totalamount" => sortDesc ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                "createdat" or "orderdate" => sortDesc ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
                _ => sortDesc ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt)
            };

            // Apply max records limit
            return await query.Take(maxRecords).ToListAsync();
        }

        #endregion
    }
}
