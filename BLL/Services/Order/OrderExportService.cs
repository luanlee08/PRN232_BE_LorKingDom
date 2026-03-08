using BLL.DTOs.Orders;
using BLL.Interfaces.Order;
using DAL.Interface;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BLL.Services.Order
{

    public class OrderExportService : IOrderExportService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ILogger<OrderExportService> _logger;

        public OrderExportService(
            IOrderRepository orderRepo,
            ILogger<OrderExportService> logger)
        {
            _orderRepo = orderRepo;
            _logger = logger;
        }

        public async Task<byte[]> ExportOrdersToExcelAsync(OrderQuery query)
        {
            var orders = await _orderRepo.GetOrdersForExportAsync(
                query.Keyword,
                query.StatusId,
                query.FromDate,
                query.ToDate,
                query.SortBy,
                query.SortDesc,
                5000);

            ExcelPackage.License.SetNonCommercialPersonal("LorKingdom");
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            // Headers
            worksheet.Cells[1, 1].Value = "Order ID";
            worksheet.Cells[1, 2].Value = "Order Code";
            worksheet.Cells[1, 3].Value = "Customer Name";
            worksheet.Cells[1, 4].Value = "Customer Phone";
            worksheet.Cells[1, 5].Value = "Email";
            worksheet.Cells[1, 6].Value = "Status";
            worksheet.Cells[1, 7].Value = "Total Amount";
            worksheet.Cells[1, 8].Value = "Shipping Fee";
            worksheet.Cells[1, 9].Value = "Shipping Address";
            worksheet.Cells[1, 10].Value = "Order Date";
            worksheet.Cells[1, 11].Value = "Payment Completed";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(255, 211, 211, 211); // ARGB LightGray — avoids System.Drawing.Common dependency
            }

            // Data
            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.OrderId;
                worksheet.Cells[row, 2].Value = $"ORD{order.OrderId:D6}";
                worksheet.Cells[row, 3].Value = order.ShippingName ?? order.Account?.AccountName ?? "Unknown";
                worksheet.Cells[row, 4].Value = order.ShippingPhone ?? "";
                worksheet.Cells[row, 5].Value = order.Account?.Email ?? "";
                worksheet.Cells[row, 6].Value = order.Status?.StatusName ?? "";
                worksheet.Cells[row, 7].Value = order.TotalAmount;
                worksheet.Cells[row, 8].Value = order.ShippingFee;
                worksheet.Cells[row, 9].Value = $"{order.ShippingAddressLine}, {order.ShippingWard}, {order.ShippingCity}";
                worksheet.Cells[row, 10].Value = order.OrderDate.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cells[row, 11].Value = order.PaymentCompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "";
                row++;
            }

            // Auto-fit columns
            if (worksheet.Dimension != null)
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }
    }
}
