namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for order export operations
    /// </summary>
    public interface IOrderExportService
    {
        /// <summary>
        /// Export orders to Excel
        /// </summary>
        Task<byte[]> ExportOrdersToExcelAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }
}
