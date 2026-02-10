using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewProductRepository : IReviewProductRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewProductRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<ReviewProduct> AddAsync(ReviewProduct review)
        {
            await _context.ReviewProducts.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<ReviewProduct?> GetByIdAsync(int reviewId)
        {
            return await _context.ReviewProducts
                .Include(r => r.Account)
                .Include(r => r.Product)
                .Include(r => r.OrderDetail)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Include(r => r.ReviewProductReactions.Where(rr => !rr.IsDeleted))
                .Include(r => r.ReviewProductReplies.Where(rp => !rp.IsDeleted))
                .FirstOrDefaultAsync(r => r.ReviewProductId == reviewId && !r.IsDeleted);
        }

        public async Task<ReviewProduct?> GetByOrderDetailIdAsync(int orderDetailId)
        {
            return await _context.ReviewProducts
                .FirstOrDefaultAsync(r => r.OrderDetailId == orderDetailId && !r.IsDeleted);
        }

        public async Task<List<ReviewProduct>> GetByProductIdAsync(int productId, bool approvedOnly = true)
        {
            var query = _context.ReviewProducts
                .Include(r => r.Account)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Include(r => r.ReviewProductReactions.Where(rr => !rr.IsDeleted))
                .Where(r => r.ProductId == productId && !r.IsDeleted);

            if (approvedOnly)
                query = query.Where(r => r.Status == "Approved" && r.Visibility == "Public");

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ReviewProduct>> GetByAccountIdAsync(int accountId)
        {
            return await _context.ReviewProducts
                .Include(r => r.Product)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Where(r => r.AccountId == accountId && !r.IsDeleted && r.Visibility == "Public")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<ReviewProduct> Items, int Total)> GetPagedAsync(
            int productId,
            int page,
            int pageSize,
            string? status = "Approved")
        {
            var query = _context.ReviewProducts
                .Include(r => r.Account)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Include(r => r.ReviewProductReactions.Where(rr => !rr.IsDeleted))
                .Where(r => r.ProductId == productId && !r.IsDeleted && r.Visibility == "Public");

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task UpdateAsync(ReviewProduct review)
        {
            _context.ReviewProducts.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int reviewId, string status, string? moderationDetail = null)
        {
            var review = await _context.ReviewProducts.FindAsync(reviewId);
            if (review != null)
            {
                review.Status = status;
                review.ModerationDetail = moderationDetail;
                review.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementEditCountAsync(int reviewId)
        {
            var review = await _context.ReviewProducts.FindAsync(reviewId);
            if (review != null)
            {
                review.EditCount++;
                review.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasReviewedAsync(int accountId, int orderDetailId)
        {
            return await _context.ReviewProducts
                .AnyAsync(r =>
                    r.AccountId == accountId &&
                    r.OrderDetailId == orderDetailId &&
                    !r.IsDeleted);
        }

        public async Task<bool> CanEditAsync(int reviewId, int accountId)
        {
            var review = await _context.ReviewProducts
                .FirstOrDefaultAsync(r =>
                    r.ReviewProductId == reviewId &&
                    r.AccountId == accountId &&
                    !r.IsDeleted);

            if (review == null) return false;

            // Only one edit allowed
            if (review.EditCount >= 1) return false;

            // Edit in just 3 days
            var daysSinceCreated = (DateTime.UtcNow - review.CreatedAt).TotalDays;
            return daysSinceCreated <= 3;
        }
    }
}
