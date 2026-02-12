using DAL.Models;

namespace DAL.Interface;

public interface IOrderRepository
{
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
        string? keyword,
        int? statusId,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        string sortBy,
        bool sortDesc);

    Task<Order?> GetByIdWithDetailsAsync(int orderId);

    Task UpdateStatusAsync(int orderId, int statusId);

    Task AddOrderStatusHistoryAsync(OrderStatusHistory history);

    Task<List<Order>> GetOrdersForExportAsync(
        string? keyword,
        int? statusId,
        DateTime? fromDate,
        DateTime? toDate,
        string sortBy,
        bool sortDesc,
        int maxRecords = 5000);
}
