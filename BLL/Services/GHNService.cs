using BLL.DTOs.Location;
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
            // Sender/Pickup Information
            from_name = request.FromName,
            from_phone = request.FromPhone,
            from_address = request.FromAddress,
            from_ward_name = request.FromWardName,
            from_district_name = request.FromDistrictName,
            from_province_name = request.FromProvinceName,
            from_district_id = request.FromDistrictId, // REQUIRED for warehouse identification
            // Return Information
            return_phone = request.ReturnPhone,
            return_address = request.ReturnAddress,
            return_district_id = int.Parse(request.ReturnDistrictId),
            return_ward_code = request.ReturnWardCode,
            // Order Information
            client_order_code = request.ClientOrderCode,
            // Recipient Information
            to_name = request.ToName,
            to_phone = request.ToPhone,
            to_address = request.ToAddress,
            to_ward_name = request.ToWardName,       // REQUIRED by GHN docs
            to_district_name = request.ToDistrictName, // REQUIRED by GHN docs
            to_province_name = request.ToProvinceName, // REQUIRED by GHN docs
            to_ward_code = request.ToWardCode,       // Optional
            to_district_id = request.ToDistrictId,   // Optional
            // Package Details
            cod_amount = request.CodAmount,
            content = request.Content,
            weight = request.Weight,
            length = request.Length,
            width = request.Width,
            height = request.Height,
            service_id = request.ServiceId,
            service_type_id = request.ServiceTypeId,
            insurance_value = request.InsuranceValue,
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

    public async Task<int?> GetDistrictIdByNameAsync(string provinceName, string districtName)
    {
        try
        {
            // Step 1: Get all provinces from GHN
            var provRequest = new HttpRequestMessage(HttpMethod.Get, $"{_apiEndpoint}/master-data/province");
            provRequest.Headers.Add("Token", _apiToken);

            var provResponse = await _httpClient.SendAsync(provRequest);
            var provContent = await provResponse.Content.ReadAsStringAsync();

            var provResult = JsonSerializer.Deserialize<BLL.DTOs.Shipping.GHNProvinceListResponse>(provContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (provResult == null || provResult.Code != 200 || provResult.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get provinces: {provContent}");
                return null;
            }

            // Normalize for matching
            var normalizedProvince = NormalizeName(provinceName);
            var matchedProvince = provResult.Data.FirstOrDefault(p =>
                NormalizeName(p.ProvinceName).Contains(normalizedProvince) ||
                normalizedProvince.Contains(NormalizeName(p.ProvinceName)));

            if (matchedProvince == null)
            {
                Console.WriteLine($"[GHN] Cannot find province: '{provinceName}' (normalized: '{normalizedProvince}')");
                return null;
            }

            Console.WriteLine($"[GHN] Matched province '{provinceName}' → ID {matchedProvince.ProvinceId} ({matchedProvince.ProvinceName})");

            // Step 2: Get districts for this province
            var distRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{_apiEndpoint}/master-data/district?province_id={matchedProvince.ProvinceId}");
            distRequest.Headers.Add("Token", _apiToken);

            var distResponse = await _httpClient.SendAsync(distRequest);
            var distContent = await distResponse.Content.ReadAsStringAsync();

            var distResult = JsonSerializer.Deserialize<BLL.DTOs.Shipping.GHNDistrictListResponse>(distContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (distResult == null || distResult.Code != 200 || distResult.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get districts for province {matchedProvince.ProvinceId}: {distContent}");
                return null;
            }

            var normalizedDistrict = NormalizeName(districtName);
            var matchedDistrict = distResult.Data.FirstOrDefault(d =>
                NormalizeName(d.DistrictName).Contains(normalizedDistrict) ||
                normalizedDistrict.Contains(NormalizeName(d.DistrictName)));

            if (matchedDistrict == null)
            {
                Console.WriteLine($"[GHN] Cannot find district: '{districtName}' in province {matchedProvince.ProvinceName}");
                return null;
            }

            Console.WriteLine($"[GHN] Matched district '{districtName}' → ID {matchedDistrict.DistrictId} ({matchedDistrict.DistrictName})");
            return matchedDistrict.DistrictId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GHN] GetDistrictIdByNameAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> GetWardCodeByNameAsync(int districtId, string wardName)
    {
        try
        {
            // Call GHN API: /master-data/ward with district_id
            var requestData = new { district_id = districtId };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/master-data/ward")
            {
                Content = content
            };
            httpRequest.Headers.Add("Token", _apiToken);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var wardResult = JsonSerializer.Deserialize<BLL.DTOs.Shipping.GHNWardListResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (wardResult == null || wardResult.Code != 200 || wardResult.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get wards for district {districtId}: {responseContent}");
                return null;
            }

            var normalizedWard = NormalizeName(wardName);
            var matchedWard = wardResult.Data.FirstOrDefault(w =>
                NormalizeName(w.WardName).Contains(normalizedWard) ||
                normalizedWard.Contains(NormalizeName(w.WardName)));

            if (matchedWard == null)
            {
                Console.WriteLine($"[GHN] Cannot find ward: '{wardName}' in district {districtId}");
                return null;
            }

            Console.WriteLine($"[GHN] Matched ward '{wardName}' → WardCode {matchedWard.WardCode} ({matchedWard.WardName})");
            return matchedWard.WardCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GHN] GetWardCodeByNameAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<GHNProvinceDTO>> GetProvincesAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiEndpoint}/master-data/province");
            request.Headers.Add("Token", _apiToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BLL.DTOs.Location.GHNProvinceListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Code != 200 || result.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get provinces: {content}");
                return new List<GHNProvinceDTO>();
            }

            return result.Data.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GHN] GetProvincesAsync error: {ex.Message}");
            return new List<GHNProvinceDTO>();
        }
    }

    public async Task<List<GHNDistrictDTO>> GetDistrictsAsync(int provinceId)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_apiEndpoint}/master-data/district?province_id={provinceId}");
            request.Headers.Add("Token", _apiToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BLL.DTOs.Location.GHNDistrictListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || result.Code != 200 || result.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get districts for province {provinceId}: {content}");
                return new List<GHNDistrictDTO>();
            }

            return result.Data.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GHN] GetDistrictsAsync error: {ex.Message}");
            return new List<GHNDistrictDTO>();
        }
    }

    public async Task<List<GHNWardDTO>> GetWardsAsync(int districtId)
    {
        try
        {
            var requestData = new { district_id = districtId };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/master-data/ward")
            {
                Content = content
            };
            httpRequest.Headers.Add("Token", _apiToken);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var wardResult = JsonSerializer.Deserialize<BLL.DTOs.Location.GHNWardListResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (wardResult == null || wardResult.Code != 200 || wardResult.Data == null)
            {
                Console.WriteLine($"[GHN] Failed to get wards for district {districtId}: {responseContent}");
                return new List<GHNWardDTO>();
            }

            return wardResult.Data.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GHN] GetWardsAsync error: {ex.Message}");
            return new List<GHNWardDTO>();
        }
    }

    private string NormalizeName(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        return text.ToLowerInvariant()
            .Replace("đ", "d").Replace("Đ", "d")
            .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
            .Replace("ă", "a").Replace("â", "a")
            .Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
            .Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
            .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
            .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
            .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
            .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
            .Replace("ô", "o").Replace("ơ", "o")
            .Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
            .Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
            .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
            .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
            .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
            // Strip administrative prefixes so "Huyện Mỹ Tú" → "my tu" matches "Mỹ Tú"
            .Replace("thanh pho ", "").Replace("tinh ", "").Replace("quan ", "").Replace("huyen ", "")
            .Replace("phuong ", "").Replace("xa ", "").Replace("thi xa ", "").Replace("thi tran ", "")
            .Trim();
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
