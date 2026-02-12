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
        Task<ApiResponse<ReviewSummaryResponse>> GetReviewSummaryAsync(int productId);
        Task<ApiResponse<List<ReviewResponse>>> GetMyReviewHistoryAsync(int productId, int accountId);

        // === ADMIN ===
        Task<ApiResponse<PagedResult<ReviewResponse>>> GetAdminReviewsAsync(AdminReviewListQuery query);
        Task<ApiResponse<ReviewResponse>> AdminUpdateReviewAsync(int reviewId, AdminUpdateReviewRequest request);
        Task<ApiResponse<object>> AdminSoftDeleteReviewAsync(int reviewId);
        Task<ApiResponse<ReplyResponse>> AddReplyAsync(AddReplyRequest request, int adminAccountId);
    }
}
