using BLL.Helpers;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Blog
{
    public class UpdateBlogRequest
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string BlogTitle { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string BlogContent { get; set; } = null!;

        [Required(ErrorMessage = "Thể loại không hợp lệ")]
        public int BlogCategoryId { get; set; }

        public IFormFile? BlogThumbnail { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsDeleted { get; set; }
    }
}
