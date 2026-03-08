using BLL.DTOs.Orders;

namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for order export operations
    /// </summary>
    public interface IOrderExportService
    {
        /// <summary>
        /// Export orders to Excel using admin query parameters
        /// </summary>
        Task<byte[]> ExportOrdersToExcelAsync(OrderQuery query);
    }
}
