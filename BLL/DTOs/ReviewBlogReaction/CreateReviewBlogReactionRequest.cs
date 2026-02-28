using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlogReaction
{
    public class CreateReviewBlogReactionRequest
    {
        public int ReviewBlogId { get; set; }
        public string ReactionType { get; set; } = null!;
    }
}
