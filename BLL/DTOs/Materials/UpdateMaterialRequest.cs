using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Materials
{
    public class UpdateMaterialRequest
    {
        [Required(ErrorMessage = "Tên Material không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên Material tối đa 255 ký tự")]
        [RegularExpression(
           @"^[a-zA-ZÀ-ỹ\s]+$",
           ErrorMessage = "Tên Material chỉ được chứa chữ cái"
       )]
        public string MaterialName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}
