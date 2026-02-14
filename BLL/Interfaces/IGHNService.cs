using BLL.DTOs.Shipping;

namespace BLL.Interfaces;

public interface IGHNService
{
    /// <summary>
    /// Tạo đơn hàng vận chuyển GHN
    /// </summary>
    Task<GHNCreateOrderResponse> CreateOrderAsync(GHNCreateOrderRequest request);

    /// <summary>
    /// Tra cứu trạng thái đơn hàng
    /// </summary>
    Task<GHNStatusResponse> GetOrderStatusAsync(string orderCode);

    /// <summary>
    /// Hủy đơn hàng
    /// </summary>
    Task<bool> CancelOrderAsync(string orderCode);

    /// <summary>
    /// Lấy danh sách dịch vụ có sẵn
    /// </summary>
    Task<GHNServiceResponse> GetAvailableServicesAsync(int fromDistrict, int toDistrict);

    /// <summary>
    /// Tính phí vận chuyển
    /// </summary>
    Task<decimal> CalculateShippingFeeAsync(int serviceId, int fromDistrict, int toDistrict,
        string toWardCode, int weight, int codAmount);
}
