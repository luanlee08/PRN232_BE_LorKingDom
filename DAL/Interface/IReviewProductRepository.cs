using DAL.Models;

namespace DAL.Interface
{
    public interface IReviewProductRepository
    {
        // === CREATE ===
        Task<ReviewProduct> AddAsync(ReviewProduct review);

        // === READ ===
        Task<ReviewProduct?> GetByIdAsync(int reviewId);
        Task<ReviewProduct?> GetByOrderDetailIdAsync(int orderDetailId);
        Task<List<ReviewProduct>> GetByProductIdAsync(int productId, bool approvedOnly = true);
        Task<List<ReviewProduct>> GetByAccountIdAsync(int accountId);
        Task<(List<ReviewProduct> Items, int Total)> GetPagedAsync(
            int productId,
            int page,
            int pageSize,
            string? status = "Approved");

        // === UPDATE ===
        Task UpdateAsync(ReviewProduct review);
        Task UpdateStatusAsync(int reviewId, string status, string? moderationDetail = null);
        Task IncrementEditCountAsync(int reviewId);

        // === CHECK ===
        Task<bool> HasReviewedAsync(int accountId, int orderDetailId);
        Task<bool> CanEditAsync(int reviewId, int accountId);
    }
}
