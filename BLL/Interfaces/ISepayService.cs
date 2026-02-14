using BLL.DTOs.PaymentGateway;

namespace BLL.Interfaces;

public interface ISepayService
{
    /// <summary>
    /// Tạo URL thanh toán Sepay
    /// </summary>
    Task<SepayResponse> CreatePaymentAsync(SepayRequest request);

    /// <summary>
    /// Xác thực callback từ Sepay
    /// </summary>
    bool ValidateCallback(SepayCallbackRequest callback);

    /// <summary>
    /// Query trạng thái giao dịch
    /// </summary>
    Task<SepayQueryResponse> QueryTransactionAsync(string orderId, string transactionId);

    /// <summary>
    /// Parse callback data từ query parameters hoặc body
    /// </summary>
    SepayCallbackRequest ParseCallback(Dictionary<string, string> data);
}
