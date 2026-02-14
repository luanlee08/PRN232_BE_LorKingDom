using BLL.DTOs.PaymentGateway;

namespace BLL.Interfaces;

public interface IMoMoService
{
    /// <summary>
    /// Tạo URL thanh toán MoMo
    /// </summary>
    Task<MoMoResponse> CreatePaymentAsync(MoMoRequest request);

    /// <summary>
    /// Xác thực callback từ MoMo
    /// </summary>
    bool ValidateCallback(MoMoCallbackRequest callback);

    /// <summary>
    /// Query trạng thái giao dịch
    /// </summary>
    Task<MoMoCallbackRequest> QueryTransactionAsync(string orderId, string requestId);
}
