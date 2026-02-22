using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Address
{
    public class AddressResponseDTO
    {
        public int AddressId { get; set; }

        public int AccountId { get; set; }
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string AddressLine { get; set; } = null!;

        // Text names (for display)
        public string City { get; set; } = null!;
        public string? District { get; set; }
        public string? Ward { get; set; }

        // GHN Master Data IDs (for shipping)
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public string? WardCode { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
