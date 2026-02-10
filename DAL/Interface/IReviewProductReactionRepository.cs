using DAL.Models;

namespace DAL.Interface
{
    public interface IReviewProductReactionRepository
    {
        Task<ReviewProductReaction?> GetReactionAsync(int reviewId, int accountId);
        Task AddAsync(ReviewProductReaction reaction);
        Task UpdateAsync(ReviewProductReaction reaction);
        Task<int> GetLikeCountAsync(int reviewId);
        Task<int> GetDislikeCountAsync(int reviewId);
    }
}
