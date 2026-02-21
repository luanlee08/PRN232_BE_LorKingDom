using BLL.DTOs.Orders;
using BLL.Interfaces.Order;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace BLL.Services.Order
{
    /// <summary>
    /// Service for exporting order data to Excel
    /// </summary>
    public class OrderExportService : IOrderExportService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly AspLorKingDomContext _context;
        private readonly ILogger<OrderExportService> _logger;

        public OrderExportService(
            IOrderRepository orderRepo,
            AspLorKingDomContext context,
            ILogger<OrderExportService> logger)
        {
            _orderRepo = orderRepo;
            _context = context;
            _logger = logger;
        }

        [Obsolete]
        public async Task<byte[]> ExportOrdersToExcelAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var query = _context.Orders
                    .Include(o => o.Account)
                    .Include(o => o.Status)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(o => o.StatusId == status.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= toDate.Value);
                }

                var orders = await query.ToListAsync();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Orders");

                // Headers
                worksheet.Cells[1, 1].Value = "Order ID";
                worksheet.Cells[1, 2].Value = "Order Code";
                worksheet.Cells[1, 3].Value = "Customer Name";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Order Date";
                worksheet.Cells[1, 6].Value = "Status";
                worksheet.Cells[1, 7].Value = "Total Amount";
                worksheet.Cells[1, 8].Value = "Shipping Address";
                worksheet.Cells[1, 9].Value = "Payment Method";
                worksheet.Cells[1, 10].Value = "Payment Status";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.OrderId;
                    worksheet.Cells[row, 2].Value = $"ORD{order.OrderId:D6}";
                    worksheet.Cells[row, 3].Value = order.ShippingName ?? order.Account?.AccountName;
                    worksheet.Cells[row, 4].Value = order.ShippingPhone;
                    worksheet.Cells[row, 5].Value = order.OrderDate;
                    worksheet.Cells[row, 6].Value = order.Status?.StatusName;
                    worksheet.Cells[row, 7].Value = order.TotalAmount;
                    worksheet.Cells[row, 8].Value = $"{order.ShippingAddressLine}, {order.ShippingWard}, {order.ShippingCity}";
                    worksheet.Cells[row, 9].Value = ""; // Get from payment history
                    worksheet.Cells[row, 10].Value = ""; // Get from payment history

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting orders to Excel");
                throw;
            }
        }
    }
}
