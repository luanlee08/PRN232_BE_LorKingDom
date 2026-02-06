using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Products
{
    public class ProductQuery
    {
        public string? Keyword { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? PriceRangeId { get; set; }
    }
}
