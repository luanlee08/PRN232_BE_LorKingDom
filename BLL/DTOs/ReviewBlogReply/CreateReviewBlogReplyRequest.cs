using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlogReply
{
    public class CreateReviewBlogReplyRequest
    {
        public int ReviewBlogId { get; set; }

        [Required]
        public string Content { get; set; } = null!;
    }
}
