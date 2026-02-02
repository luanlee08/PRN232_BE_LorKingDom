using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BLL.DTOs.Products
{
    public class CreateProductRequest
    {
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public int MaterialId { get; set; }
        public int AgeId { get; set; }
        public int SexId { get; set; }
        public int PriceRangeId { get; set; }
        public int BrandId { get; set; }
        public int OriginId { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ProductStatus { get; set; }

        public string? DescriptionHtml { get; set; }

        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? SecondaryImages { get; set; }
    }
}
