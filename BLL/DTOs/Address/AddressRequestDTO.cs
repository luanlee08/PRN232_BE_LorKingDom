namespace BLL.DTOs.Address
{
    public class AddressRequestDTO
    {
        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }
        public string City { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}
