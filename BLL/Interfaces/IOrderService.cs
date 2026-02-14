using BLL.DTOs;
using BLL.DTOs.Orders;

namespace BLL.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersAsync(OrderQuery query);
    Task<ApiResponse<OrderDetailResponse>> GetOrderDetailAsync(int orderId);
    Task<ApiResponse<object>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, int adminId);
    Task<byte[]> ExportOrdersToExcelAsync(OrderQuery query);
}
