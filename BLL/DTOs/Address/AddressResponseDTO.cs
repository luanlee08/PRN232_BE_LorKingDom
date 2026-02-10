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

        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }
        public string City { get; set; } = null!;

        public bool IsDefault { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
