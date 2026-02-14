using BLL.DTOs.Shipping;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BLL.Services;

public class GoShipService : IGoShipService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;
    private readonly string _apiEndpoint;
    private readonly string _clientId;

    public GoShipService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _apiToken = configuration["GoShip:ApiToken"] ?? "";
        _clientId = configuration["GoShip:ClientId"] ?? "";
        _apiEndpoint = configuration["GoShip:ApiEndpoint"] ?? "https://api.goship.io/api/v1";
    }

    public async Task<GoShipCreateOrderResponse> CreateOrderAsync(GoShipCreateOrderRequest request)
    {
        var requestData = new
        {
            order_code = request.Order.Id,
            client_id = _clientId,
            sender = new
            {
                name = request.Order.SenderName,
                phone = request.Order.SenderPhone,
                address = request.Order.SenderAddress,
                city = request.Order.SenderCity,
                district = request.Order.SenderDistrict,
                ward = request.Order.SenderWard
            },
            receiver = new
            {
                name = request.Order.ReceiverName,
                phone = request.Order.ReceiverPhone,
                address = request.Order.ReceiverAddress,
                city = request.Order.ReceiverCity,
                district = request.Order.ReceiverDistrict,
                ward = request.Order.ReceiverWard
            },
            shipment = new
            {
                weight = request.Products.Sum(p => p.Weight * p.Quantity),
                length = 20, // Default dimensions
                width = 20,
                height = 10,
                value = request.Products.Sum(p => p.Price * p.Quantity),
                cod_amount = request.Order.CodAmount,
                note = request.Order.Note,
                service_type = request.Order.ServiceType
            },
            items = request.Products.Select(p => new
            {
                name = p.Name,
                quantity = p.Quantity,
                weight = p.Weight,
                price = p.Price
            }).ToArray()
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/orders")
        {
            Content = content
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiToken}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GoShipApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || !result.Success)
            {
                return new GoShipCreateOrderResponse
                {
                    Success = false,
                    Message = result?.Message ?? "Unknown error"
                };
            }

            return new GoShipCreateOrderResponse
            {
                Success = true,
                Message = result.Message ?? "Order created successfully",
                Data = result.Data == null ? null : new GoShipOrderData
                {
                    OrderCode = result.Data.OrderCode ?? "",
                    TrackingNumber = result.Data.TrackingNumber ?? "",
                    ShippingFee = result.Data.ShippingFee,
                    InsuranceFee = result.Data.InsuranceFee,
                    EstimatedPickupTime = result.Data.EstimatedPickupTime ?? "",
                    EstimatedDeliveryTime = result.Data.EstimatedDeliveryTime ?? "",
                    QrCode = result.Data.QrCode ?? ""
                }
            };
        }
        catch (Exception ex)
        {
            return new GoShipCreateOrderResponse
            {
                Success = false,
                Message = $"Error creating order: {ex.Message}"
            };
        }
    }

    public async Task<GoShipStatusResponse> GetOrderStatusAsync(string trackingNumber)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_apiEndpoint}/orders/{trackingNumber}");
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiToken}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GoShipStatusApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || !result.Success)
            {
                return new GoShipStatusResponse
                {
                    Success = false,
                    Message = result?.Message ?? "Unknown error"
                };
            }

            return new GoShipStatusResponse
            {
                Success = true,
                Message = "Success",
                Data = result.Data == null ? null : new GoShipOrderStatus
                {
                    OrderCode = result.Data.OrderCode ?? "",
                    TrackingNumber = result.Data.TrackingNumber ?? "",
                    Status = result.Data.Status ?? "",
                    StatusDescription = result.Data.StatusDescription ?? "",
                    ShippingFee = result.Data.ShippingFee,
                    PickedUpAt = result.Data.PickedUpAt,
                    DeliveredAt = result.Data.DeliveredAt
                }
            };
        }
        catch (Exception ex)
        {
            return new GoShipStatusResponse
            {
                Success = false,
                Message = $"Error getting order status: {ex.Message}"
            };
        }
    }

    public async Task<bool> CancelOrderAsync(string trackingNumber)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{_apiEndpoint}/orders/{trackingNumber}");
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiToken}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GoShipApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Success ?? false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<GoShipFeeResponse> CalculateShippingFeeAsync(GoShipFeeRequest request)
    {
        var requestData = new
        {
            sender_city = request.SenderCity,
            sender_district = request.SenderDistrict,
            receiver_city = request.ReceiverCity,
            receiver_district = request.ReceiverDistrict,
            receiver_ward = request.ReceiverWard,
            weight = request.Weight,
            value = request.Value,
            service_type = request.ServiceType
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json"
        );

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_apiEndpoint}/shipping-fee")
        {
            Content = content
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiToken}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GoShipFeeApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || !result.Success)
            {
                return new GoShipFeeResponse
                {
                    Success = false,
                    Message = result?.Message ?? "Unknown error"
                };
            }

            return new GoShipFeeResponse
            {
                Success = true,
                Message = "Success",
                Data = result.Data == null ? null : new GoShipFeeData
                {
                    ShippingFee = result.Data.ShippingFee,
                    InsuranceFee = result.Data.InsuranceFee,
                    TotalFee = result.Data.TotalFee
                }
            };
        }
        catch (Exception ex)
        {
            return new GoShipFeeResponse
            {
                Success = false,
                Message = $"Error calculating shipping fee: {ex.Message}"
            };
        }
    }

    // Helper classes for API responses
    private class GoShipApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public GoShipOrderApiData? Data { get; set; }
    }

    private class GoShipOrderApiData
    {
        [JsonPropertyName("order_code")]
        public string? OrderCode { get; set; }

        [JsonPropertyName("tracking_number")]
        public string? TrackingNumber { get; set; }

        [JsonPropertyName("shipping_fee")]
        public decimal ShippingFee { get; set; }

        [JsonPropertyName("insurance_fee")]
        public decimal InsuranceFee { get; set; }

        [JsonPropertyName("estimated_pickup_time")]
        public string? EstimatedPickupTime { get; set; }

        [JsonPropertyName("estimated_delivery_time")]
        public string? EstimatedDeliveryTime { get; set; }

        [JsonPropertyName("qr_code")]
        public string? QrCode { get; set; }
    }

    private class GoShipStatusApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public GoShipOrderStatusApiData? Data { get; set; }
    }

    private class GoShipOrderStatusApiData
    {
        [JsonPropertyName("order_code")]
        public string? OrderCode { get; set; }

        [JsonPropertyName("tracking_number")]
        public string? TrackingNumber { get; set; }

        public string? Status { get; set; }

        [JsonPropertyName("status_description")]
        public string? StatusDescription { get; set; }

        [JsonPropertyName("shipping_fee")]
        public decimal ShippingFee { get; set; }

        [JsonPropertyName("picked_up_at")]
        public DateTime? PickedUpAt { get; set; }

        [JsonPropertyName("delivered_at")]
        public DateTime? DeliveredAt { get; set; }
    }

    private class GoShipFeeApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public GoShipFeeApiData? Data { get; set; }
    }

    private class GoShipFeeApiData
    {
        [JsonPropertyName("shipping_fee")]
        public decimal ShippingFee { get; set; }

        [JsonPropertyName("insurance_fee")]
        public decimal InsuranceFee { get; set; }

        [JsonPropertyName("total_fee")]
        public decimal TotalFee { get; set; }
    }
}
