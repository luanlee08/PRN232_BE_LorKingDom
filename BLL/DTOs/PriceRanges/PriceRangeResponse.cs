using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.PriceRanges
{
    public class PriceRangeResponse
    {
        public int PriceRangeId { get; set; }
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
