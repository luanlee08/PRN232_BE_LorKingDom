using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ReviewBlogReply
{
    public class ReviewBlogReplyResponse
    {
        public int ReplyBlogId { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
