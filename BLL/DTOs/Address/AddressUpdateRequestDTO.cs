namespace BLL.DTOs.Address
{
    public class AddressUpdateRequestDTO
    {
        public int AddressId { get; set; }


        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }
        public string City { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}
