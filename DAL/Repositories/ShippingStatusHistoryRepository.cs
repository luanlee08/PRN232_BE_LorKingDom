using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class ShippingStatusHistoryRepository : IShippingStatusHistoryRepository
{
    private readonly AspLorKingDomContext _context;

    public ShippingStatusHistoryRepository(AspLorKingDomContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ShippingStatusHistory history)
    {
        await _context.ShippingStatusHistories.AddAsync(history);
    }

    public async Task<IEnumerable<ShippingStatusHistory>> GetByShippingTxIdAsync(long shippingTxId)
    {
        return await _context.ShippingStatusHistories
            .Where(h => h.ShippingTxId == shippingTxId)
            .OrderByDescending(h => h.ProcessedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ShippingStatusHistory>> GetByOrderIdAsync(int orderId)
    {
        return await _context.ShippingStatusHistories
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.ProcessedAt)
            .ToListAsync();
    }
}
