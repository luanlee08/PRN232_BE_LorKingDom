using DAL.Models;

namespace DAL.Interface;

public interface IShippingStatusHistoryRepository
{
    Task AddAsync(ShippingStatusHistory history);
    Task<IEnumerable<ShippingStatusHistory>> GetByShippingTxIdAsync(long shippingTxId);
    Task<IEnumerable<ShippingStatusHistory>> GetByOrderIdAsync(int orderId);
}
