using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Categories
{
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage = "SuperCategoryId không được để trống")]
        public int SuperCategoryId { get; set; }
        [Required(ErrorMessage = "Tên Category không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên Category tối đa 255 ký tự")]
        [RegularExpression(
            @"^[a-zA-ZÀ-ỹ\s]+$",
            ErrorMessage = "Tên Category chỉ được chứa chữ cái"
        )]
        public string CategoryName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
