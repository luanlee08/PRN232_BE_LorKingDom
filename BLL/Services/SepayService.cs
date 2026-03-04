using BLL.DTOs.PaymentGateway;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BLL.Services;

public class SepayService : ISepayService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _merchantId;
    private readonly string _secretKey;
    private readonly string _apiEndpoint;

    public SepayService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _merchantId = configuration["Sepay:MerchantId"] ?? "";
        _secretKey = configuration["Sepay:SecretKey"] ?? "";
        _apiEndpoint = configuration["Sepay:ApiEndpoint"] ?? "https://my.sepay.vn/userapi/transactions/create";
    }

    public async Task<SepayResponse> CreatePaymentAsync(SepayRequest request)
    {
        // 🔧 TEMPORARY: Mock response for testing since test credentials may not work
        // TODO: Replace with real Sepay credentials when available
        Console.WriteLine($"[Sepay] MerchantId: {_merchantId}");
        Console.WriteLine($"[Sepay] Checking if starts with 'SP-TEST': {_merchantId.StartsWith("SP-TEST")}");

        if (_merchantId.StartsWith("SP-TEST"))
        {
            var mockUrl = $"http://localhost:3000/payment/sepay-test?mock=true&order_id={request.OrderId}&amount={request.Amount:F0}";
            Console.WriteLine($"[Sepay] ✅ Using MOCK response - Redirect to: {mockUrl}");
            return new SepayResponse
            {
                PaymentUrl = mockUrl,
                QRCodeUrl = "https://via.placeholder.com/300x300?text=QR+Code",
                TransactionId = $"MOCK-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}"
            };
        }

        Console.WriteLine($"[Sepay] ⚠️ Using REAL API (production credentials detected)");

        // Tạo raw data cho signature
        var rawData = $"{_merchantId}|{request.OrderId}|{request.Amount:F0}|{request.OrderInfo}|{request.ReturnUrl}";
        var signature = HmacSHA256(_secretKey, rawData);

        var requestData = new
        {
            merchant_id = _merchantId,
            order_id = request.OrderId,
            amount = (long)request.Amount,
            content = request.OrderInfo,
            return_url = request.ReturnUrl,
            cancel_url = request.CancelUrl,
            notify_url = request.NotifyUrl,
            bank_code = request.BankCode,
            signature = signature
        };

        // Log request for debugging
        Console.WriteLine($"[Sepay] API Endpoint: {_apiEndpoint}");
        Console.WriteLine($"[Sepay] Merchant ID: {_merchantId}");
        Console.WriteLine($"[Sepay] Request: {JsonSerializer.Serialize(requestData)}");

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");

        var response = await _httpClient.PostAsync(_apiEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log response for debugging
        Console.WriteLine($"[Sepay] Status Code: {response.StatusCode}");
        Console.WriteLine($"[Sepay] Response: {responseContent}");

        // Check if response is HTML (error page)
        if (responseContent.TrimStart().StartsWith("<"))
        {
            throw new Exception($"Sepay API returned HTML instead of JSON. Status: {response.StatusCode}. This usually means authentication failed or endpoint is incorrect. Response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
        }

        // Check HTTP status code
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Sepay API HTTP error: {response.StatusCode}. Response: {responseContent}");
        }

        SepayApiResponse? result;
        try
        {
            result = JsonSerializer.Deserialize<SepayApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse Sepay response as JSON. Response: {responseContent}", ex);
        }

        if (result == null || !result.Success)
        {
            throw new Exception($"Sepay API error: {result?.Message ?? "Unknown error"}. Response: {responseContent}");
        }

        return new SepayResponse
        {
            PaymentUrl = result.Data?.PaymentUrl ?? "",
            QRCodeUrl = result.Data?.QrCodeUrl ?? "",
            TransactionId = result.Data?.TransactionId ?? ""
        };
    }

    public bool ValidateCallback(SepayCallbackRequest callback)
    {
        try
        {
            // 🔧 Bypass signature validation in test/mock mode (same as CreatePaymentAsync)
            if (_merchantId.StartsWith("SP-TEST"))
            {
                Console.WriteLine($"[Sepay] ✅ Mock mode — skipping signature validation for order {callback.order_id}");
                return true;
            }

            // Tạo raw signature từ callback data
            var rawData = $"{callback.order_id}|{callback.transaction_id}|{callback.amount:F0}|{callback.status}|{callback.timestamp}";
            var expectedSignature = HmacSHA256(_secretKey, rawData);

            return expectedSignature.Equals(callback.signature, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public async Task<SepayQueryResponse> QueryTransactionAsync(string orderId, string transactionId)
    {
        var queryEndpoint = _configuration["Sepay:QueryEndpoint"] ?? "https://my.sepay.vn/userapi/transactions/check";

        var rawData = $"{_merchantId}|{orderId}|{transactionId}";
        var signature = HmacSHA256(_secretKey, rawData);

        var requestData = new
        {
            merchant_id = _merchantId,
            order_id = orderId,
            transaction_id = transactionId,
            signature = signature
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_secretKey}");

        var response = await _httpClient.PostAsync(queryEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<SepayQueryApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
        {
            throw new Exception("Failed to query Sepay transaction");
        }

        return new SepayQueryResponse
        {
            Success = result.Success,
            Status = result.Data?.Status ?? "unknown",
            Amount = result.Data?.Amount ?? 0,
            Message = result.Message ?? "",
            Data = result.Data == null ? null : new SepayTransactionData
            {
                TransactionId = result.Data.TransactionId ?? "",
                OrderId = result.Data.OrderId ?? "",
                Amount = result.Data.Amount,
                Status = result.Data.Status ?? "",
                CreatedAt = result.Data.CreatedAt,
                CompletedAt = result.Data.CompletedAt
            }
        };
    }

    public SepayCallbackRequest ParseCallback(Dictionary<string, string> data)
    {
        return new SepayCallbackRequest
        {
            order_id = data.GetValueOrDefault("order_id", ""),
            transaction_id = data.GetValueOrDefault("transaction_id", ""),
            reference_number = data.GetValueOrDefault("reference_number", ""),
            amount = decimal.TryParse(data.GetValueOrDefault("amount", "0"), out var amt) ? amt : 0,
            content = data.GetValueOrDefault("content", ""),
            status = data.GetValueOrDefault("status", ""),
            bank_code = data.GetValueOrDefault("bank_code", ""),
            account_number = data.GetValueOrDefault("account_number", ""),
            timestamp = long.TryParse(data.GetValueOrDefault("timestamp", "0"), out var ts) ? ts : 0,
            signature = data.GetValueOrDefault("signature", "")
        };
    }

    private string HmacSHA256(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    // Helper classes for API responses
    private class SepayApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public SepayApiData? Data { get; set; }
    }

    private class SepayApiData
    {
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public string? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    private class SepayQueryApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public SepayApiData? Data { get; set; }
    }
}
