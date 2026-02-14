using BLL.DTOs.PaymentGateway;

namespace BLL.Interfaces;

public interface IVNPayService
{
    /// <summary>
    /// Tạo URL thanh toán VNPay
    /// </summary>
    Task<VNPayResponse> CreatePaymentUrlAsync(VNPayRequest request);

    /// <summary>
    /// Xác thực callback từ VNPay
    /// </summary>
    bool ValidateCallback(VNPayCallbackRequest callback);

    /// <summary>
    /// Parse query string callback thành object
    /// </summary>
    VNPayCallbackRequest ParseCallback(Dictionary<string, string> queryParams);
}
