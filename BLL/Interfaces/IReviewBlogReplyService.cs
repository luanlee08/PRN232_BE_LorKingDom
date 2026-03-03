using BLL.DTOs;
using BLL.DTOs.ReviewBlogReply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IReviewBlogReplyService
    {
        Task<ApiResponse<bool>> CreateAsync(
            int accountId,
            CreateReviewBlogReplyRequest request);

        Task<ApiResponse<List<ReviewBlogReplyResponse>>>
            GetByReviewIdAsync(int reviewBlogId);
    }
}
