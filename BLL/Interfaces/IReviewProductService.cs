using BLL.DTOs;
using BLL.DTOs.ReviewProduct;

namespace BLL.Interfaces
{
    public interface IReviewProductService
    {
        Task<ApiResponse<ReviewResponse>> AddReviewAsync(AddReviewRequest request, int accountId);
        Task<ApiResponse<ReviewResponse>> EditReviewAsync(int reviewId, EditReviewRequest request, int accountId);
        Task<ApiResponse<PagedResult<ReviewResponse>>> GetReviewsAsync(ReviewListQuery query, int? currentAccountId = null);
        Task<ApiResponse<ReviewResponse>> GetReviewByIdAsync(int reviewId, int? currentAccountId = null);
    }
}
