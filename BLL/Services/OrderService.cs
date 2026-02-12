using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Interfaces;
using DAL.Infrastructure;
using DAL.Interface;
using DAL.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Status transition rules (only management statuses 1-6, excluding Refunded)
    private readonly Dictionary<int, List<int>> _allowedTransitions = new()
    {
        { 1, new List<int> { 2, 6 } },  // Pending -> Confirmed or Cancelled
        { 2, new List<int> { 3, 6 } },  // Confirmed -> Shipped or Cancelled
        { 3, new List<int> { 4 } },     // Shipped -> Delivered
        { 4, new List<int> { 5 } },     // Delivered -> Completed
        { 5, new List<int>() },         // Completed -> (no transitions)
        { 6, new List<int>() }          // Cancelled -> (no transitions)
    };

    public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersAsync(OrderQuery query)
    {
        try
        {
            var (items, totalCount) = await _orderRepository.GetPagedAsync(
                query.Keyword,
                query.StatusId,
                query.FromDate,
                query.ToDate,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDesc);

            var orderResponses = items.Select(MapToOrderResponse).ToList();

            return new ApiResponse<PagedResult<OrderResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách đơn hàng thành công",
                Data = new PagedResult<OrderResponse>
                {
                    Items = orderResponses,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedResult<OrderResponse>>
            {
                Status = 500,
                StatusMessage = "ERROR",
                Message = $"Đã xảy ra lỗi: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<OrderDetailResponse>> GetOrderDetailAsync(int orderId)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
            {
                return new ApiResponse<OrderDetailResponse>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            var response = MapToOrderDetailResponse(order);

            return new ApiResponse<OrderDetailResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy chi tiết đơn hàng thành công",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<OrderDetailResponse>
            {
                Status = 500,
                StatusMessage = "ERROR",
                Message = $"Đã xảy ra lỗi: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<object>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, int adminId)
    {
        try
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
            {
                return new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy đơn hàng"
                };
            }

            // Block updates on Refunded orders
            if (order.StatusId == 7)
            {
                return new ApiResponse<object>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Đơn hàng đang trong quá trình hoàn tiền, không thể thay đổi trạng thái"
                };
            }

            // Validate status transition
            if (!IsValidTransition(order.StatusId, request.StatusId))
            {
                return new ApiResponse<object>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Chuyển trạng thái không hợp lệ"
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Update order status
                await _orderRepository.UpdateStatusAsync(orderId, request.StatusId);

                // Create status history
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = request.StatusId,
                    ChangedBy = adminId,
                    ChangedAt = DateTime.UtcNow,
                    Note = request.Note
                };

                await _orderRepository.AddOrderStatusHistoryAsync(statusHistory);

                await _unitOfWork.CommitTransactionAsync();

                return new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Cập nhật trạng thái đơn hàng thành công"
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Status = 500,
                StatusMessage = "ERROR",
                Message = $"Đã xảy ra lỗi: {ex.Message}"
            };
        }
    }

    public async Task<byte[]> ExportOrdersToExcelAsync(OrderQuery query)
    {
        // Get orders for export (max 5000)
        var orders = await _orderRepository.GetOrdersForExportAsync(
            query.Keyword,
            query.StatusId,
            query.FromDate,
            query.ToDate,
            query.SortBy,
            query.SortDesc,
            5000);

        // Check if result exceeds limit
        if (orders.Count >= 5000)
        {
            throw new InvalidOperationException("Chỉ cho phép export tối đa 5,000 đơn hàng. Vui lòng thu hẹp bộ lọc.");
        }

        ExcelPackage.License.SetNonCommercialPersonal("Lorkingdom");
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Orders");

        // Headers
        worksheet.Cells[1, 1].Value = "STT";
        worksheet.Cells[1, 2].Value = "Mã đơn hàng";
        worksheet.Cells[1, 3].Value = "Tên khách hàng";
        worksheet.Cells[1, 4].Value = "Số điện thoại";
        worksheet.Cells[1, 5].Value = "Tổng tiền";
        worksheet.Cells[1, 6].Value = "Trạng thái";
        worksheet.Cells[1, 7].Value = "Trạng thái hoàn tiền";
        worksheet.Cells[1, 8].Value = "Ngày đặt hàng";
        worksheet.Cells[1, 9].Value = "Sản phẩm";
        worksheet.Cells[1, 10].Value = "Địa chỉ giao hàng";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 10])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Data rows
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = i + 1;
            worksheet.Cells[row, 2].Value = $"ORD{order.OrderId:D8}";
            worksheet.Cells[row, 3].Value = order.ShippingName ?? order.Account?.Email ?? "N/A";
            worksheet.Cells[row, 4].Value = order.ShippingPhone ?? "N/A";
            worksheet.Cells[row, 5].Value = order.TotalAmount;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[row, 6].Value = order.Status?.StatusName ?? "N/A";
            worksheet.Cells[row, 7].Value = order.RefundStatus;
            worksheet.Cells[row, 8].Value = order.OrderDate;
            worksheet.Cells[row, 8].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";

            // Products (concatenated)
            var products = string.Join(", ",
                order.OrderDetails.Select(od => $"{od.Product?.ProductName} (x{od.Quantity})"));
            worksheet.Cells[row, 9].Value = products;

            // Shipping address
            var address = BuildShippingAddress(order);
            worksheet.Cells[row, 10].Value = address;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

    private bool IsValidTransition(int currentStatusId, int newStatusId)
    {
        if (!_allowedTransitions.ContainsKey(currentStatusId))
            return false;

        return _allowedTransitions[currentStatusId].Contains(newStatusId);
    }

    private OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            OrderCode = $"ORD{order.OrderId:D8}",
            CustomerName = order.ShippingName ?? order.Account?.Email ?? "N/A",
            CustomerPhone = order.ShippingPhone ?? "N/A",
            StatusId = order.StatusId,
            StatusName = order.Status?.StatusName ?? "Unknown",
            TotalAmount = order.TotalAmount,
            ShippingAddress = BuildShippingAddress(order),
            OrderDate = order.OrderDate,
            PaymentCompletedAt = order.PaymentCompletedAt,
            RefundStatus = order.RefundStatus,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemResponse
            {
                ProductId = od.ProductId,
                ProductName = od.Product?.ProductName ?? "N/A",
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Total = od.Total ?? 0,
                ImageUrl = od.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
            }).ToList()
        };
    }

    private OrderDetailResponse MapToOrderDetailResponse(Order order)
    {
        var response = new OrderDetailResponse
        {
            OrderId = order.OrderId,
            OrderCode = $"ORD{order.OrderId:D8}",
            CustomerName = order.ShippingName ?? order.Account?.Email ?? "N/A",
            CustomerPhone = order.ShippingPhone ?? "N/A",
            StatusId = order.StatusId,
            StatusName = order.Status?.StatusName ?? "Unknown",
            TotalAmount = order.TotalAmount,
            ShippingAddress = BuildShippingAddress(order),
            OrderDate = order.OrderDate,
            PaymentCompletedAt = order.PaymentCompletedAt,
            RefundStatus = order.RefundStatus,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemResponse
            {
                ProductId = od.ProductId,
                ProductName = od.Product?.ProductName ?? "N/A",
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Total = od.Total ?? 0,
                ImageUrl = od.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
            }).ToList(),
            AccountId = order.AccountId,
            AccountEmail = order.Account?.Email ?? "N/A",
            VoucherId = order.VoucherId,
            VoucherCode = order.Voucher?.VoucherCode,
            VoucherDiscount = order.Voucher?.DiscountValue,
            ShippingMethod = order.ShippingMethod,
            ShippingFee = order.ShippingFee,
            PaidByWalletAmount = order.PaidByWalletAmount,
            PaidByExternalAmount = order.PaidByExternalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            StatusHistories = order.OrderStatusHistories
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new OrderStatusHistoryResponse
                {
                    OrderStatusHistoryId = h.OrderStatusHistoryId,
                    StatusId = h.StatusId ?? 0,
                    StatusName = h.Status?.StatusName ?? "Unknown",
                    ChangedAt = h.ChangedAt,
                    ChangedBy = h.ChangedBy,
                    ChangedByName = h.ChangedByNavigation?.Email,
                    Note = h.Note
                }).ToList()
        };

        return response;
    }

    private string BuildShippingAddress(Order order)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(order.ShippingAddressLine))
            parts.Add(order.ShippingAddressLine);

        if (!string.IsNullOrWhiteSpace(order.ShippingWard))
            parts.Add(order.ShippingWard);

        if (!string.IsNullOrWhiteSpace(order.ShippingCity))
            parts.Add(order.ShippingCity);

        return parts.Count > 0 ? string.Join(", ", parts) : "N/A";
    }
}
