using BLL.DTOs;
using BLL.DTOs.ReviewProduct;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class ReviewProductReactionService : IReviewReactionService
    {
        private readonly IReviewProductRepository _reviewRepo;
        private readonly IReviewProductReactionRepository _reactionRepo;

        public ReviewProductReactionService(
            IReviewProductRepository reviewRepo,
            IReviewProductReactionRepository reactionRepo)
        {
            _reviewRepo = reviewRepo;
            _reactionRepo = reactionRepo;
        }

        public async Task<ApiResponse<object>> ToggleReactionAsync(
            ReactionRequest request, int accountId)
        {
            try
            {
                // === STEP 1: Validate Review ===
                var review = await _reviewRepo.GetByIdAsync(request.ReviewProductId);

                if (review == null || review.IsDeleted)
                {
                    return new ApiResponse<object>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy review"
                    };
                }

                // Chỉ cho reaction review đã Approved
                if (review.Status != "Approved")
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Chỉ có thể reaction review đã được duyệt"
                    };
                }

                // Không cho reaction review của chính mình
                if (review.AccountId == accountId)
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Bạn không thể reaction review của chính mình"
                    };
                }

                // === STEP 2: Check existing reaction ===
                var existingReaction = await _reactionRepo.GetReactionAsync(
                    request.ReviewProductId, accountId);

                if (existingReaction != null)
                {
                    if (existingReaction.ReactionType == request.ReactionType)
                    {
                        // Remove reaction
                        existingReaction.IsDeleted = true;
                        await _reactionRepo.UpdateAsync(existingReaction);

                        return new ApiResponse<object>
                        {
                            Status = 200,
                            StatusMessage = "SUCCESS",
                            Message = "Đã hủy reaction",
                            Data = new { Action = "Removed" }
                        };
                    }
                    else
                    {
                        // Change reaction
                        existingReaction.ReactionType = request.ReactionType;
                        existingReaction.IsDeleted = false;
                        await _reactionRepo.UpdateAsync(existingReaction);

                        return new ApiResponse<object>
                        {
                            Status = 200,
                            StatusMessage = "SUCCESS",
                            Message = $"Đã đổi sang {request.ReactionType}",
                            Data = new { Action = "Changed", NewType = request.ReactionType }
                        };
                    }
                }

                // === STEP 3: Add new reaction ===
                var newReaction = new ReviewProductReaction
                {
                    ReviewProductId = request.ReviewProductId,
                    AccountId = accountId,
                    ReactionType = request.ReactionType,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _reactionRepo.AddAsync(newReaction);

                return new ApiResponse<object>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = $"Đã {request.ReactionType} review",
                    Data = new { Action = "Added", Type = request.ReactionType }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = $"Có lỗi xảy ra: {ex.Message}"
                };
            }
        }
    }
}
