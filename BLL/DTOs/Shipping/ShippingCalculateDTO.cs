namespace BLL.DTOs.Shipping
{
    /// <summary>
    /// Request to calculate shipping fee
    /// </summary>
    public class CalculateShippingFeeRequest
    {
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public int Weight { get; set; } = 1000; // Default 1kg in grams
        public decimal OrderValue { get; set; }
        public string? Carrier { get; set; } // "Express", "Standard", "Economy"
    }

    /// <summary>
    /// Shipping method information for customer
    /// </summary>
    public class ShippingMethodInfo
    {
        public string Code { get; set; } = null!; // Unique identifier (e.g. "GHN-53322")
        public string Type { get; set; } = null!; // "Express", "Standard", "Economy" - for backend processing
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Fee { get; set; }
        public string EstimatedDays { get; set; } = null!;
        public string Carrier { get; set; } = "Internal"; // "Internal", "GHN", "GoShip"
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// Response with available shipping methods
    /// </summary>
    public class GetShippingMethodsResponse
    {
        public List<ShippingMethodInfo> ShippingMethods { get; set; } = new();
    }
}
