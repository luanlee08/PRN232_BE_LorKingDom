using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlog
{
    public class ReviewBlogAdminDto
    {
        public int ReviewBlogId { get; set; }
        public string BlogTitle { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
