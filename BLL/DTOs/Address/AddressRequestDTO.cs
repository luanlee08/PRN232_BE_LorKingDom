namespace BLL.DTOs.Address
{
    public class AddressRequestDTO
    {
        public string RecipientName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AddressLine { get; set; } = null!;

        // Text names (for display and legacy compatibility)
        public string City { get; set; } = null!;
        public string? District { get; set; }
        public string? Ward { get; set; }

        // GHN Master Data IDs (for reliable shipping integration)
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string? WardCode { get; set; }

        public bool IsDefault { get; set; }
    }
}
