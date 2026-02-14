using BLL.DTOs.Shipping;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLL.Services;

public class GHNService : IGHNService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;
    private readonly int _shopId;
    private readonly string _apiEndpoint;

    public GHNService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _apiToken = configuration["GHN:ApiToken"] ?? "";
        _shopId = int.Parse(configuration["GHN:ShopId"] ?? "0");
        _apiEndpoint = configuration["GHN:ApiEndpoint"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api";
    }

    public async Task<GHNCreateOrderResponse> CreateOrderAsync(GHNCreateOrderRequest request)
    {
        var requestData = new
        {
            payment_type_id = int.Parse(request.PaymentTypeId),
            note = request.Note,
            required_note = request.RequiredNote,
            return_phone = request.ReturnPhone,
            return_address = request.ReturnAddress,
            return_district_id = int.Parse(request.ReturnDistrictId),
            return_ward_code = request.ReturnWardCode,
            client_order_code = request.ClientOrderCode,
            to_name = request.ToName,
            to_phone = request.ToPhone,
            to_address = request.ToAddress,
            to_ward_code = request.ToWardCode,
            to_district_id = request.ToDistrictId,
            cod_amount = request.CodAmount,
            content = request.Content,
            weight = request.Weight,
            length = request.Length,
            width = request.Width,
            height = request.Height,
            service_id = request.ServiceId,
            service_type_id = request.ServiceTypeId,
            items = request.Items.Select(i => new
            {
                name = i.Name,
                code = i.Code,
                quantity = i.Quantity,
                price = i.Price,
                weight = i.Weight
            }).ToArray()
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/v2/shipping-order/create")
        {
            Content = content
        };
        httpRequest.Headers.Add("Token", _apiToken);
        httpRequest.Headers.Add("ShopId", _shopId.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GHNApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null || result.Code != 200)
        {
            return new GHNCreateOrderResponse
            {
                Code = result?.Code ?? 500,
                Message = result?.Message ?? "Unknown error"
            };
        }

        return new GHNCreateOrderResponse
        {
            Code = result.Code,
            Message = result.Message ?? "Success",
            Data = result.Data == null ? null : new GHNOrderData
            {
                OrderCode = result.Data.OrderCode ?? "",
                SortCode = result.Data.SortCode ?? "",
                TransType = result.Data.TransType ?? "",
                WardEncode = result.Data.WardEncode ?? "",
                DistrictEncode = result.Data.DistrictEncode ?? "",
                Fee = result.Data.TotalFee,
                TotalFee = result.Data.TotalFee,
                ExpectedDeliveryTime = result.Data.ExpectedDeliveryTime ?? ""
            }
        };
    }

    public async Task<GHNStatusResponse> GetOrderStatusAsync(string orderCode)
    {
        var requestData = new { order_code = orderCode };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/v2/shipping-order/detail")
        {
            Content = content
        };
        httpRequest.Headers.Add("Token", _apiToken);
        httpRequest.Headers.Add("ShopId", _shopId.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GHNStatusApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null || result.Code != 200)
        {
            return new GHNStatusResponse
            {
                Code = result?.Code ?? 500,
                Message = result?.Message ?? "Unknown error"
            };
        }

        return new GHNStatusResponse
        {
            Code = result.Code,
            Message = "Success",
            Data = result.Data == null ? null : new GHNOrderStatus
            {
                OrderCode = result.Data.OrderCode ?? "",
                Status = result.Data.Status ?? "",
                StatusText = GetStatusText(result.Data.Status ?? ""),
                Fee = result.Data.TotalFee,
                ExpectedDeliveryTime = result.Data.ExpectedDeliveryTime ?? ""
            }
        };
    }

    public async Task<bool> CancelOrderAsync(string orderCode)
    {
        var requestData = new { order_codes = new[] { orderCode } };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/v2/switch-status/cancel")
        {
            Content = content
        };
        httpRequest.Headers.Add("Token", _apiToken);
        httpRequest.Headers.Add("ShopId", _shopId.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GHNApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Code == 200;
    }

    public async Task<GHNServiceResponse> GetAvailableServicesAsync(int fromDistrict, int toDistrict)
    {
        var requestData = new
        {
            shop_id = _shopId,
            from_district = fromDistrict,
            to_district = toDistrict
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/v2/shipping-order/available-services")
        {
            Content = content
        };
        httpRequest.Headers.Add("Token", _apiToken);

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GHNServiceApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null || result.Code != 200)
        {
            return new GHNServiceResponse
            {
                Code = result?.Code ?? 500,
                Data = Array.Empty<DTOs.Shipping.GHNService>()
            };
        }

        return new GHNServiceResponse
        {
            Code = result.Code,
            Data = result.Data?.Select(s => new DTOs.Shipping.GHNService
            {
                ServiceId = s.ServiceId,
                ShortName = s.ShortName ?? "",
                ServiceTypeId = s.ServiceTypeId
            }).ToArray() ?? Array.Empty<DTOs.Shipping.GHNService>()
        };
    }

    public async Task<decimal> CalculateShippingFeeAsync(int serviceId, int fromDistrict, int toDistrict,
        string toWardCode, int weight, int codAmount)
    {
        var requestData = new
        {
            service_id = serviceId,
            from_district_id = fromDistrict,
            to_district_id = toDistrict,
            to_ward_code = toWardCode,
            weight = weight,
            insurance_value = 0,
            cod_value = codAmount
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/v2/shipping-order/fee")
        {
            Content = content
        };
        httpRequest.Headers.Add("Token", _apiToken);
        httpRequest.Headers.Add("ShopId", _shopId.ToString());

        var response = await _httpClient.SendAsync(httpRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GHNFeeResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Data?.Total ?? 0;
    }

    private string GetStatusText(string status)
    {
        return status switch
        {
            "ready_to_pick" => "Chờ lấy hàng",
            "picking" => "Đang lấy hàng",
            "picked" => "Đã lấy hàng",
            "storing" => "Đang lưu kho",
            "transporting" => "Đang vận chuyển",
            "delivering" => "Đang giao hàng",
            "delivered" => "Đã giao hàng",
            "return" => "Chuyển hoàn",
            "returned" => "Đã chuyển hoàn",
            "cancel" => "Đơn hủy",
            _ => status
        };
    }

    // Helper classes for API responses
    private class GHNApiResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public GHNOrderApiData? Data { get; set; }
    }

    private class GHNOrderApiData
    {
        [JsonPropertyName("order_code")]
        public string? OrderCode { get; set; }

        [JsonPropertyName("sort_code")]
        public string? SortCode { get; set; }

        [JsonPropertyName("trans_type")]
        public string? TransType { get; set; }

        [JsonPropertyName("ward_encode")]
        public string? WardEncode { get; set; }

        [JsonPropertyName("district_encode")]
        public string? DistrictEncode { get; set; }

        [JsonPropertyName("total_fee")]
        public decimal TotalFee { get; set; }

        [JsonPropertyName("expected_delivery_time")]
        public string? ExpectedDeliveryTime { get; set; }

        public string? Status { get; set; }
    }

    private class GHNStatusApiResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public GHNOrderApiData? Data { get; set; }
    }

    private class GHNServiceApiResponse
    {
        public int Code { get; set; }
        public GHNServiceApiData[]? Data { get; set; }
    }

    private class GHNServiceApiData
    {
        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        [JsonPropertyName("service_type_id")]
        public int ServiceTypeId { get; set; }
    }

    private class GHNFeeResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public GHNFeeData? Data { get; set; }
    }

    private class GHNFeeData
    {
        public decimal Total { get; set; }

        [JsonPropertyName("service_fee")]
        public decimal ServiceFee { get; set; }

        [JsonPropertyName("insurance_fee")]
        public decimal InsuranceFee { get; set; }
    }
}
