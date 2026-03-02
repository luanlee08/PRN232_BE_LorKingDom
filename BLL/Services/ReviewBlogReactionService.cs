using BLL.DTOs;
using BLL.DTOs.ReviewBlogReaction;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReviewBlogReactionService
    : IReviewBlogReactionService
    {
        private readonly IReviewBlogReactionRepository _repo;
        private readonly IReviewBlogRepository _reviewRepo;

        public ReviewBlogReactionService(
            IReviewBlogReactionRepository repo,
            IReviewBlogRepository reviewRepo)
        {
            _repo = repo;
            _reviewRepo = reviewRepo;
        }

        public async Task<ApiResponse<bool>> ReactAsync(
    int accountId,
    CreateReviewBlogReactionRequest request)
        {
            var existing = await _repo
    .GetAsync(request.ReviewBlogId, accountId);

            // Nếu đã react trước đó
            if (existing != null)
            {
                // Nếu cùng loại reaction → bỏ reaction (toggle)
                if (existing.ReactionType == request.ReactionType)
                {
                    await _repo.RemoveAsync(existing);
                    await _repo.SaveChangesAsync();

                    return new ApiResponse<bool>
                    {
                        Status = 200,
                        StatusMessage = "SUCCESS",
                        Message = "Đã bỏ reaction",
                        Data = true
                    };
                }

                // Nếu khác loại → update
                existing.ReactionType = request.ReactionType;
                await _repo.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Đã cập nhật reaction",
                    Data = true
                };
            }

            // Nếu chưa từng react → tạo mới
            var reaction = new ReviewBlogReaction
            {
                AccountId = accountId,
                ReviewBlogId = request.ReviewBlogId,
                ReactionType = request.ReactionType,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(reaction);
            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Đã react thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<bool>> RemoveAsync(
    int accountId,
    int reviewBlogId)
        {
            var reaction = await _repo
                .GetAsync(reviewBlogId, accountId);

            if (reaction == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Reaction không tồn tại",
                    Data = false
                };
            }

            await _repo.RemoveAsync(reaction);
            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đã xóa reaction",
                Data = true
            };
        }

        public async Task<ApiResponse<ReviewBlogReactionSummaryDto>>
    GetSummaryAsync(int? accountId, int reviewBlogId)
        {
            var likeCount = await _repo
                .CountByTypeAsync(reviewBlogId, "Like");

            var dislikeCount = await _repo
                .CountByTypeAsync(reviewBlogId, "Dislike");

            string? userReaction = null;

            if (accountId.HasValue)
            {
                userReaction = await _repo
                    .GetUserReactionAsync(reviewBlogId, accountId.Value);
            }

            return new ApiResponse<ReviewBlogReactionSummaryDto>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Data = new ReviewBlogReactionSummaryDto
                {
                    LikeCount = likeCount,
                    DislikeCount = dislikeCount,
                    UserReaction = userReaction
                }
            };
        }
    }
}
