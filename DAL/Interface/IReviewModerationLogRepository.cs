using DAL.Models;

namespace DAL.Interface
{
    public interface IReviewModerationLogRepository
    {
        Task AddAsync(ReviewModerationLog log);
        Task<List<ReviewModerationLog>> GetByReviewIdAsync(int reviewId);
    }
}
