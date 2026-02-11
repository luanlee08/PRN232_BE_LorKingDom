using DAL.Models;

namespace DAL.Interface
{
    public interface IReviewProductImageRepository
    {
        Task AddAsync(ReviewProductImage image);
        Task AddRangeAsync(IEnumerable<ReviewProductImage> images);
        Task<List<ReviewProductImage>> GetByReviewIdAsync(int reviewId);
        Task DeleteRange(IEnumerable<ReviewProductImage> images);
    }
}
