using BLL.DTOs;
using BLL.DTOs.Shipping;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/[controller]")]
    public class CShippingController : ControllerBase
    {
        private readonly IGHNService _ghnService;
        private readonly IConfiguration _configuration;

        public CShippingController(IGHNService ghnService, IConfiguration configuration)
        {
            _ghnService = ghnService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get available shipping methods with fees from multiple carriers
        /// Calculates real-time fees from GHN
        /// Users can compare and choose the best option
        /// </summary>
        [HttpGet("methods")]
        public async Task<IActionResult> GetShippingMethods(
            [FromQuery] string? city = null,
            [FromQuery] string? district = null,
            [FromQuery] string? ward = null,
            [FromQuery] int weight = 1000,
            [FromQuery] decimal orderValue = 0,
            [FromQuery] string? carrier = null,
            [FromQuery] int? districtId = null,
            [FromQuery] string? wardCode = null)
        {
            // ⚠️ Require address for dynamic pricing
            if (string.IsNullOrEmpty(city) && !districtId.HasValue)
            {
                return BadRequest(new ApiResponse<GetShippingMethodsResponse>
                {
                    Status = 400,
                    StatusMessage = "Bad Request",
                    Message = "Vui lòng chọn địa chỉ giao hàng để tính phí vận chuyển",
                    Data = null
                });
            }

            var methods = new List<ShippingMethodInfo>();
            var errors = new List<string>();

            // Read shop address from configuration
            var senderCity = _configuration["ShopAddress:City"] ?? "Cần Thơ";
            var senderDistrict = _configuration["ShopAddress:District"] ?? "Bình Thủy";
            var shopDistrictId = int.Parse(_configuration["ShopAddress:DistrictId"] ?? "3695");

            Console.WriteLine($"[Shipping] Calculating fees from {senderCity} ({senderDistrict}) to {city}, {district}");
            Console.WriteLine($"[Shipping] Weight: {weight}g, Value: {orderValue:N0}đ");


            // ========== TRY GHN ==========
            try
            {
                int? ghnDistrictId;
                string? ghnWardCode;

                if (districtId.HasValue)
                {
                    // PRIORITY 1: use stored GHN IDs — no API lookup needed
                    ghnDistrictId = districtId.Value;
                    ghnWardCode = wardCode;
                    Console.WriteLine($"[Shipping] ✅ Using stored GHN IDs: DistrictId={ghnDistrictId}, WardCode={ghnWardCode}");
                }
                else
                {
                    // FALLBACK: text-based lookup
                    Console.WriteLine($"[Shipping] ⚠️ No GHN IDs stored, falling back to text lookup for '{district}, {city}'");
                    ghnDistrictId = await _ghnService.GetDistrictIdByNameAsync(city ?? "", district ?? "");
                    ghnWardCode = null;
                    if (ghnDistrictId.HasValue && !string.IsNullOrEmpty(ward))
                    {
                        ghnWardCode = await _ghnService.GetWardCodeByNameAsync(ghnDistrictId.Value, ward);
                        if (!string.IsNullOrEmpty(ghnWardCode))
                            Console.WriteLine($"[Shipping] 🗺️ Mapped '{ward}' to GHN Ward Code: {ghnWardCode}");
                    }
                }

                if (ghnDistrictId.HasValue)
                {
                    Console.WriteLine($"[Shipping] 🗺️ GHN District ID: {ghnDistrictId.Value}");

                    // Get available services from shop to customer
                    var servicesRes = await _ghnService.GetAvailableServicesAsync(shopDistrictId, ghnDistrictId.Value);

                    if (servicesRes.Code == 200 && servicesRes.Data.Length > 0)
                    {
                        Console.WriteLine($"[Shipping] Found {servicesRes.Data.Length} GHN services");

                        foreach (var service in servicesRes.Data)
                        {
                            try
                            {
                                var fee = await _ghnService.CalculateShippingFeeAsync(
                                    service.ServiceId,
                                    shopDistrictId,
                                    ghnDistrictId.Value,
                                    ghnWardCode ?? "", // Ward code from lookup
                                    weight,
                                    (int)orderValue
                                );

                                if (fee > 0)
                                {
                                    Console.WriteLine($"[Shipping] ✅ GHN {service.ShortName}: {fee:N0}đ (Service ID: {service.ServiceId})");

                                    var (type, name, estimatedDays) = MapGHNServiceInfo(service.ShortName);
                                    methods.Add(new ShippingMethodInfo
                                    {
                                        Code = $"GHN-{service.ServiceId}",
                                        Type = type, // "Express", "Standard", "Economy"
                                        Name = $"{name} (GHN)",
                                        Description = $"Dịch vụ {service.ShortName}",
                                        Fee = fee,
                                        EstimatedDays = estimatedDays,
                                        Carrier = "GHN",
                                        IsAvailable = true
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Shipping] ⚠️ GHN {service.ShortName} failed: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[Shipping] ⚠️ No GHN services available for this route (Code: {servicesRes.Code}), using fallback");
                        errors.Add("GHN: Không có dịch vụ khả dụng cho tuyến đường này");
                    }
                }
                else
                {
                    Console.WriteLine($"[Shipping] ⚠️ Cannot map '{city}/{district}' to GHN District ID");
                    errors.Add($"GHN: Chưa hỗ trợ khu vực {district}, {city}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Shipping] ❌ GHN Error: {ex.Message}");
                errors.Add($"GHN: {ex.Message}");
            }

            // ========== RETURN RESULTS ==========
            if (methods.Count == 0)
            {
                // GHN unavailable or no route — return estimated fallback fees so checkout can proceed
                Console.WriteLine($"[Shipping] ⚠️ No real fees obtained, returning estimated fallback. Errors: {string.Join("; ", errors)}");
                methods = new List<ShippingMethodInfo>
                {
                    new ShippingMethodInfo
                    {
                        Code = "FALLBACK-Standard",
                        Type = "Standard",
                        Name = "Giao tiêu chuẩn (ước tính)",
                        Description = "Phí ước tính — không thể kết nối GHN",
                        Fee = 30000,
                        EstimatedDays = "3-5 ngày",
                        Carrier = "GHN",
                        IsAvailable = true
                    },
                    new ShippingMethodInfo
                    {
                        Code = "FALLBACK-Express",
                        Type = "Express",
                        Name = "Giao nhanh (ước tính)",
                        Description = "Phí ước tính — không thể kết nối GHN",
                        Fee = 50000,
                        EstimatedDays = "1-2 ngày",
                        Carrier = "GHN",
                        IsAvailable = true
                    }
                };
            }

            // Sort by fee (cheapest first)
            methods = methods.OrderBy(m => m.Fee).ToList();

            Console.WriteLine($"[Shipping] ✅ Total {methods.Count} shipping methods available");

            var isFallback = methods.All(m => m.Code.StartsWith("FALLBACK-"));
            var response = new ApiResponse<GetShippingMethodsResponse>
            {
                Status = 200,
                StatusMessage = "OK",
                Message = isFallback
                    ? "Hiển thị phí ước tính (không thể kết nối GHN)"
                    : $"Tìm thấy {methods.Count} phương thức vận chuyển từ {senderCity} đến {city}",
                Data = new GetShippingMethodsResponse
                {
                    ShippingMethods = methods
                }
            };

            return Ok(response);
        }

        private (string code, string name, string estimatedDays) MapGHNServiceInfo(string serviceName)
        {
            var normalized = serviceName?.ToLower() ?? "";
            if (normalized.Contains("hỏa") || normalized.Contains("express"))
                return ("Express", "Giao hỏa tốc", "1-2 ngày");
            if (normalized.Contains("nhanh"))
                return ("Standard", "Giao nhanh", "2-3 ngày");
            if (normalized.Contains("tiêu chuẩn") || normalized.Contains("standard"))
                return ("Standard", "Giao tiêu chuẩn", "3-5 ngày");
            return ("Standard", serviceName ?? "Giao hàng", "3-5 ngày");
        }


        /// <summary>
        /// Calculate shipping fee for specific method
        /// </summary>
        [HttpPost("calculate-fee")]
        public async Task<IActionResult> CalculateShippingFee([FromBody] CalculateShippingFeeRequest request)
        {
            // Calculate fee based on shipping method
            var fee = request.Carrier?.ToLower() switch
            {
                "express" => 50000m,
                "standard" => 30000m,
                "economy" => 20000m,
                _ => 30000m // Default to standard
            };

            // TODO: Can be extended to use GHN real-time calculation
            // if (request.City != null && request.District != null) {
            //     try {
            //         var ghnFee = await _ghnService.CalculateShippingFeeAsync(...);
            //         fee = ghnFee;
            //     } catch { /* fallback to fixed fee */ }
            // }

            var response = new ApiResponse<decimal>
            {
                Status = 200,
                StatusMessage = "OK",
                Message = "Tính phí vận chuyển thành công",
                Data = fee
            };

            return Ok(response);
        }
    }
}
