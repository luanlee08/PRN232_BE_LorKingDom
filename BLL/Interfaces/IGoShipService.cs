using BLL.DTOs.Shipping;

namespace BLL.Interfaces;

public interface IGoShipService
{
    /// <summary>
    /// Tạo đơn hàng vận chuyển GoShip
    /// </summary>
    Task<GoShipCreateOrderResponse> CreateOrderAsync(GoShipCreateOrderRequest request);

    /// <summary>
    /// Lấy trạng thái đơn hàng
    /// </summary>
    Task<GoShipStatusResponse> GetOrderStatusAsync(string trackingNumber);

    /// <summary>
    /// Hủy đơn hàng
    /// </summary>
    Task<bool> CancelOrderAsync(string trackingNumber);

    /// <summary>
    /// Tính phí vận chuyển
    /// </summary>
    Task<GoShipFeeResponse> CalculateShippingFeeAsync(GoShipFeeRequest request);
}
