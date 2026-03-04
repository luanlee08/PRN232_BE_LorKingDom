using BLL.DTOs.ReviewBlogReply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlog
{
    public class ReviewBlogResponse
    {
        public int ReviewBlogId { get; set; }
        public int AccountId { get; set; }
        public string? CustomerName { get; set; }
        public int Rating { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string? Comment { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public required List<ReviewBlogReplyResponse> Replies { get; set; }
    }
}
