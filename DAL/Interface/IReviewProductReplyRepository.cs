using DAL.Models;

namespace DAL.Interface
{
    public interface IReviewProductReplyRepository
    {
        Task<ReviewProductReply> AddAsync(ReviewProductReply reply);
        Task<List<ReviewProductReply>> GetByReviewIdAsync(int reviewId);
        Task<ReviewProductReply?> GetByIdAsync(int replyId);
        Task UpdateAsync(ReviewProductReply reply);
        Task SoftDeleteAsync(int replyId);
    }
}
