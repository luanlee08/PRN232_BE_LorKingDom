using BLL.DTOs.PaymentGateway;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services;

public class VNPayService : IVNPayService
{
    private readonly IConfiguration _configuration;
    private readonly string _vnpUrl;
    private readonly string _vnpTmnCode;
    private readonly string _vnpHashSecret;
    private readonly string _vnpVersion = "2.1.0";
    private readonly string _vnpCommand = "pay";

    public VNPayService(IConfiguration configuration)
    {
        _configuration = configuration;
        _vnpUrl = configuration["VNPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        _vnpTmnCode = configuration["VNPay:TmnCode"] ?? "";
        _vnpHashSecret = configuration["VNPay:HashSecret"] ?? "";
    }

    public async Task<VNPayResponse> CreatePaymentUrlAsync(VNPayRequest request)
    {
        // VNPay yêu cầu amount * 100 và là số nguyên
        var amount = ((long)(request.Amount * 100)).ToString();

        var vnpay = new SortedDictionary<string, string>
        {
            { "vnp_Version", _vnpVersion },
            { "vnp_Command", _vnpCommand },
            { "vnp_TmnCode", _vnpTmnCode },
            { "vnp_Amount", amount },
            { "vnp_CreateDate", request.CreateDate.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", request.IpAddress },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", request.OrderInfo },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", request.ReturnUrl },
            { "vnp_TxnRef", request.OrderId }
        };

        // Tạo query string
        var queryString = string.Join("&", vnpay.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

        // Tạo secure hash
        var signData = queryString;
        var secureHash = HmacSHA512(_vnpHashSecret, signData);

        // URL cuối cùng
        var paymentUrl = $"{_vnpUrl}?{queryString}&vnp_SecureHash={secureHash}";

        return await Task.FromResult(new VNPayResponse
        {
            PaymentUrl = paymentUrl
        });
    }

    public bool ValidateCallback(VNPayCallbackRequest callback)
    {
        try
        {
            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_TmnCode", callback.vnp_TmnCode },
                { "vnp_Amount", callback.vnp_Amount },
                { "vnp_BankCode", callback.vnp_BankCode },
                { "vnp_BankTranNo", callback.vnp_BankTranNo },
                { "vnp_CardType", callback.vnp_CardType },
                { "vnp_PayDate", callback.vnp_PayDate },
                { "vnp_OrderInfo", callback.vnp_OrderInfo },
                { "vnp_TransactionNo", callback.vnp_TransactionNo },
                { "vnp_ResponseCode", callback.vnp_ResponseCode },
                { "vnp_TransactionStatus", callback.vnp_TransactionStatus },
                { "vnp_TxnRef", callback.vnp_TxnRef },
                { "vnp_SecureHashType", callback.vnp_SecureHashType }
            };

            var queryString = string.Join("&", vnpayData.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            var checkSum = HmacSHA512(_vnpHashSecret, queryString);

            return checkSum.Equals(callback.vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public VNPayCallbackRequest ParseCallback(Dictionary<string, string> queryParams)
    {
        return new VNPayCallbackRequest
        {
            vnp_TmnCode = queryParams.GetValueOrDefault("vnp_TmnCode", ""),
            vnp_Amount = queryParams.GetValueOrDefault("vnp_Amount", ""),
            vnp_BankCode = queryParams.GetValueOrDefault("vnp_BankCode", ""),
            vnp_BankTranNo = queryParams.GetValueOrDefault("vnp_BankTranNo", ""),
            vnp_CardType = queryParams.GetValueOrDefault("vnp_CardType", ""),
            vnp_PayDate = queryParams.GetValueOrDefault("vnp_PayDate", ""),
            vnp_OrderInfo = queryParams.GetValueOrDefault("vnp_OrderInfo", ""),
            vnp_TransactionNo = queryParams.GetValueOrDefault("vnp_TransactionNo", ""),
            vnp_ResponseCode = queryParams.GetValueOrDefault("vnp_ResponseCode", ""),
            vnp_TransactionStatus = queryParams.GetValueOrDefault("vnp_TransactionStatus", ""),
            vnp_TxnRef = queryParams.GetValueOrDefault("vnp_TxnRef", ""),
            vnp_SecureHashType = queryParams.GetValueOrDefault("vnp_SecureHashType", ""),
            vnp_SecureHash = queryParams.GetValueOrDefault("vnp_SecureHash", "")
        };
    }

    private string HmacSHA512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
