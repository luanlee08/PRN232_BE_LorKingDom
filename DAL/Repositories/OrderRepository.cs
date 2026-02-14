using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AspLorKingDomContext _context;

    public OrderRepository(AspLorKingDomContext context)
    {
        _context = context;
    }

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
            .Include(o => o.Account)
            .Include(o => o.Status)
            .Include(o => o.Voucher)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.ProductImages)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim().ToLower();
            query = query.Where(o =>
                (o.ShippingName != null && o.ShippingName.ToLower().Contains(keyword)) ||
                (o.ShippingPhone != null && o.ShippingPhone.Contains(keyword)) ||
                o.Account.Email.ToLower().Contains(keyword));
        }

        if (statusId.HasValue)
        {
            query = query.Where(o => o.StatusId == statusId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var toDateEnd = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(o => o.OrderDate <= toDateEnd);
        }

        // Count total before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = sortBy switch
        {
            "Status" => sortDesc
                ? query.OrderByDescending(o => o.StatusId)
                : query.OrderBy(o => o.StatusId),
            "TotalAmount" => sortDesc
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),
            _ => sortDesc
                ? query.OrderByDescending(o => o.OrderDate)
                : query.OrderBy(o => o.OrderDate)
        };

        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Status)
            .Include(o => o.Voucher)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.ProductImages)
            .Include(o => o.OrderStatusHistories)
                .ThenInclude(osh => osh.Status)
            .Include(o => o.OrderStatusHistories)
                .ThenInclude(osh => osh.ChangedByNavigation)
            .Where(o => !o.IsDeleted)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task UpdateStatusAsync(int orderId, int statusId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.StatusId = statusId;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddOrderStatusHistoryAsync(OrderStatusHistory history)
    {
        _context.OrderStatusHistories.Add(history);
        await _context.SaveChangesAsync();
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
            .Include(o => o.Account)
            .Include(o => o.Status)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        // Apply same filters as GetPagedAsync
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim().ToLower();
            query = query.Where(o =>
                (o.ShippingName != null && o.ShippingName.ToLower().Contains(keyword)) ||
                (o.ShippingPhone != null && o.ShippingPhone.Contains(keyword)) ||
                o.Account.Email.ToLower().Contains(keyword));
        }

        if (statusId.HasValue)
        {
            query = query.Where(o => o.StatusId == statusId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var toDateEnd = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(o => o.OrderDate <= toDateEnd);
        }

        // Apply sorting
        query = sortBy switch
        {
            "Status" => sortDesc
                ? query.OrderByDescending(o => o.StatusId)
                : query.OrderBy(o => o.StatusId),
            "TotalAmount" => sortDesc
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),
            _ => sortDesc
                ? query.OrderByDescending(o => o.OrderDate)
                : query.OrderBy(o => o.OrderDate)
        };

        // Limit to max records
        return await query.Take(maxRecords).ToListAsync();
    }
}
