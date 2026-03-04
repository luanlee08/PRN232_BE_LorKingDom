using BLL.DTOs;
using BLL.DTOs.ReviewBlogReaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IReviewBlogReactionService
    {
        Task<ApiResponse<bool>> ReactAsync(
            int accountId,
            CreateReviewBlogReactionRequest request);

        Task<ApiResponse<bool>> RemoveAsync(int accountId, int reviewBlogId);
        Task<ApiResponse<ReviewBlogReactionSummaryDto>>
    GetSummaryAsync(int? accountId, int reviewBlogId);
    }
}
