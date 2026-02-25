
using System.ComponentModel.DataAnnotations;
using BLL.Helpers;
using Microsoft.AspNetCore.Http;
namespace BLL.DTOs.Blog
{
    public class CreateBlogRequest
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string BlogTitle { get; set; } = null!;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string BlogContent { get; set; } = null!;

        [RequiredFile(ErrorMessage = "Ảnh đại diện là bắt buộc")]
        public IFormFile BlogThumbnail { get; set; } = null!;

        [Required(ErrorMessage = "Thể loại không được để trống")]
        public int BlogCategoryId { get; set; }

        public bool IsPublished { get; set; } = true;

        public bool IsFeatured { get; set; } = false;
    }

}
