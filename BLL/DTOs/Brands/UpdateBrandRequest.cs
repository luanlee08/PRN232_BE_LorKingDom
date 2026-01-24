using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Brands
{
    public class UpdateBrandRequest
    {
        [Required(ErrorMessage = "Tên Brand không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên Brand tối đa 255 ký tự")]
        [RegularExpression(
            @"^[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên Brand chỉ được chứa chữ cái"
        )]
        public string BrandName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
