using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlogReaction
{
    public class ReviewBlogReactionSummaryDto
    {
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string? UserReaction { get; set; }
    }
}
