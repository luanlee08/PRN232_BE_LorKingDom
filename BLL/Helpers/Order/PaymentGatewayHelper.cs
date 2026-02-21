using BLL.DTOs.Orders;
using BLL.DTOs.PaymentGateway;
using BLL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Helpers.Order
{
    /// <summary>
    /// Helper for payment gateway operations
    /// </summary>
    public class PaymentGatewayHelper
    {
        private readonly IVNPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly ISepayService _sepayService;
        private readonly ILogger<PaymentGatewayHelper> _logger;

        public PaymentGatewayHelper(
            IVNPayService vnPayService,
            IMoMoService moMoService,
            ISepayService sepayService,
            ILogger<PaymentGatewayHelper> logger)
        {
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _sepayService = sepayService;
            _logger = logger;
        }

        /// <summary>
        /// Generate payment URL based on provider
        /// </summary>
        public async Task<string> GeneratePaymentUrlAsync(
            string provider,
            int orderId,
            decimal amount,
            string baseUrl,
            string ipAddress)
        {
            try
            {
                switch (provider)
                {
                    case PaymentMethods.VNPay:
                        var vnpayRequest = new VNPayRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/vnpay-return",
                            IpAddress = ipAddress
                        };
                        var vnpayResponse = await _vnPayService.CreatePaymentUrlAsync(vnpayRequest);
                        return vnpayResponse.PaymentUrl;

                    case PaymentMethods.MoMo:
                        var momoRequest = new MoMoRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/momo-return",
                            NotifyUrl = $"{baseUrl}/api/order/webhook/payment/momo"
                        };
                        var momoResponse = await _moMoService.CreatePaymentAsync(momoRequest);
                        return momoResponse.PayUrl;

                    case PaymentMethods.Sepay:
                        var sepayRequest = new SepayRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/sepay-return",
                            CancelUrl = $"{baseUrl}/api/order/sepay-cancel",
                            NotifyUrl = $"{baseUrl}/api/order/webhook/payment/sepay"
                        };
                        var sepayResponse = await _sepayService.CreatePaymentAsync(sepayRequest);
                        return sepayResponse.PaymentUrl;

                    default:
                        throw new Exception($"Unsupported payment provider: {provider}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment URL for provider {Provider}", provider);
                throw;
            }
        }
    }
}
