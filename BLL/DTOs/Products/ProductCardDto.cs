using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Products
{
    public class ProductCardDto
    {
        public int Id { get; set; }

        // Tên sản phẩm
        public string ProductName { get; set; } = string.Empty;

        // Ảnh chính
        public string? MainImageUrl { get; set; }

        // Giá bán
        public decimal Price { get; set; }

        // Số lượng tồn
        public int StockQuantity { get; set; }
    }
}
