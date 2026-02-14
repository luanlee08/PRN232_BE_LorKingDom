using BLL.DTOs.PaymentGateway;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BLL.Services;

public class MoMoService : IMoMoService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _partnerCode;
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _apiEndpoint;

    public MoMoService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _partnerCode = configuration["MoMo:PartnerCode"] ?? "";
        _accessKey = configuration["MoMo:AccessKey"] ?? "";
        _secretKey = configuration["MoMo:SecretKey"] ?? "";
        _apiEndpoint = configuration["MoMo:ApiEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";
    }

    public async Task<MoMoResponse> CreatePaymentAsync(MoMoRequest request)
    {
        var requestId = request.RequestId;
        var orderId = request.OrderId;
        var amount = ((long)request.Amount).ToString();
        var orderInfo = request.OrderInfo;
        var returnUrl = request.ReturnUrl;
        var notifyUrl = request.NotifyUrl;
        var extraData = ""; // Extra data if needed

        // Tạo raw signature
        var rawSignature = $"accessKey={_accessKey}&amount={amount}&extraData={extraData}&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={_partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";

        var signature = HmacSHA256(_secretKey, rawSignature);

        var requestData = new
        {
            partnerCode = _partnerCode,
            accessKey = _accessKey,
            requestId = requestId,
            amount = amount,
            orderId = orderId,
            orderInfo = orderInfo,
            redirectUrl = returnUrl,
            ipnUrl = notifyUrl,
            extraData = extraData,
            requestType = "captureWallet",
            signature = signature,
            lang = "vi"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(_apiEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<MoMoApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null || result.ResultCode != 0)
        {
            throw new Exception($"MoMo API error: {result?.Message ?? "Unknown error"}");
        }

        return new MoMoResponse
        {
            PayUrl = result.PayUrl ?? "",
            DeepLink = result.DeepLink ?? "",
            QRCodeUrl = result.QrCodeUrl ?? ""
        };
    }

    public bool ValidateCallback(MoMoCallbackRequest callback)
    {
        try
        {
            var rawSignature = $"accessKey={_accessKey}&amount={callback.amount}&extraData={callback.extraData}&message={callback.message}&orderId={callback.orderId}&orderInfo={callback.orderInfo}&orderType={callback.orderType}&partnerCode={callback.partnerCode}&payType={callback.payType}&requestId={callback.requestId}&responseTime={callback.responseTime}&resultCode={callback.resultCode}&transId={callback.transId}";

            var signature = HmacSHA256(_secretKey, rawSignature);

            return signature.Equals(callback.signature, StringComparison.InvariantCultureIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public async Task<MoMoCallbackRequest> QueryTransactionAsync(string orderId, string requestId)
    {
        var queryEndpoint = _configuration["MoMo:QueryEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/query";

        var rawSignature = $"accessKey={_accessKey}&orderId={orderId}&partnerCode={_partnerCode}&requestId={requestId}";
        var signature = HmacSHA256(_secretKey, rawSignature);

        var requestData = new
        {
            partnerCode = _partnerCode,
            accessKey = _accessKey,
            requestId = requestId,
            orderId = orderId,
            signature = signature,
            lang = "vi"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(queryEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<MoMoCallbackRequest>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? throw new Exception("Failed to query MoMo transaction");
    }

    private string HmacSHA256(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    // Helper class for API response
    private class MoMoApiResponse
    {
        public string? PartnerCode { get; set; }
        public string? OrderId { get; set; }
        public string? RequestId { get; set; }
        public long Amount { get; set; }
        public int ResultCode { get; set; }
        public string? Message { get; set; }
        public string? PayUrl { get; set; }
        public string? DeepLink { get; set; }
        public string? QrCodeUrl { get; set; }
    }
}
