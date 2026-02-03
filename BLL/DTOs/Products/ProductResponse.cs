using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Products
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ProductStatus { get; set; } = null!;
        public bool IsDeleted { get; set; }

        // Lookup name
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public string? MaterialName { get; set; }
        public string? AgeRange { get; set; }
        public string? SexName { get; set; }
        public string? OriginName { get; set; }

        // Images
        public string? MainImageUrl { get; set; }
        public List<string> SecondaryImageUrls { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}
