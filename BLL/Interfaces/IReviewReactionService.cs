using BLL.DTOs;
using BLL.DTOs.ReviewProduct;

namespace BLL.Interfaces
{
    public interface IReviewReactionService
    {
        Task<ApiResponse<object>> ToggleReactionAsync(ReactionRequest request, int accountId);
    }
}
