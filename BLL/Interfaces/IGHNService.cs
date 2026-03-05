using BLL.DTOs.Location;
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

    /// <summary>
    /// Tìm District ID từ tên tỉnh và tên huyện (dynamic lookup từ GHN API)
    /// </summary>
    Task<int?> GetDistrictIdByNameAsync(string provinceName, string districtName);

    /// <summary>
    /// Tìm Ward Code từ District ID và tên phường/xã (dynamic lookup từ GHN API)
    /// </summary>
    Task<string?> GetWardCodeByNameAsync(int districtId, string wardName);

    /// <summary>
    /// Lấy danh sách tất cả tỉnh/thành từ GHN master-data API
    /// </summary>
    Task<List<GHNProvinceDTO>> GetProvincesAsync();

    /// <summary>
    /// Lấy danh sách quận/huyện theo tỉnh từ GHN master-data API
    /// </summary>
    Task<List<GHNDistrictDTO>> GetDistrictsAsync(int provinceId);

    /// <summary>
    /// Lấy danh sách phường/xã theo quận từ GHN master-data API
    /// </summary>
    Task<List<GHNWardDTO>> GetWardsAsync(int districtId);

    /// <summary>
    /// Lấy chi tiết đơn hàng GHN bao gồm log trạng thái
    /// </summary>
    Task<GHNTrackingDetail?> GetOrderTrackingAsync(string orderCode);
}
