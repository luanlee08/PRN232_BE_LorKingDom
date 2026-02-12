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
                .Where(r => r.AccountId == accountId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<ReviewProduct> Items, int Total)> GetPagedAsync(
            int productId,
            int page,
            int pageSize,
            string? status = "Approved",
            int? rating = null,
            int? requestingAccountId = null)
        {
            var query = _context.ReviewProducts
                .Include(r => r.Account)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Include(r => r.ReviewProductReactions.Where(rr => !rr.IsDeleted))
                .Where(r => r.ProductId == productId && !r.IsDeleted);

            // === VISIBILITY FILTER ===
            if (requestingAccountId.HasValue)
            {
                // Show: Public reviews OR own reviews (any status)
                query = query.Where(r =>
                    (r.Status == "Approved" && r.Visibility == "Public") ||
                    r.AccountId == requestingAccountId.Value);
            }
            else
            {
                // Public only - no user context
                query = query.Where(r => r.Status == "Approved" && r.Visibility == "Public");
            }

            // === STATUS FILTER (for public reviews only) ===
            if (!string.IsNullOrEmpty(status) && !requestingAccountId.HasValue)
            {
                query = query.Where(r => r.Status == status);
            }

            // === RATING FILTER ===
            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            var total = await query.CountAsync();

            // === PRIORITY SORTING ===
            // 1. Own reviews first
            // 2. Then by CreatedAt DESC
            var items = await query
                .OrderByDescending(r => requestingAccountId.HasValue && r.AccountId == requestingAccountId.Value)
                .ThenByDescending(r => r.CreatedAt)
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

        public async Task<List<ReviewProduct>> GetMyReviewHistoryAsync(int productId, int accountId)
        {
            return await _context.ReviewProducts
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Where(r => r.ProductId == productId && r.AccountId == accountId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(decimal AverageRating, int TotalReviews, Dictionary<int, int> RatingDistribution)> GetReviewSummaryAsync(int productId)
        {
            var approvedReviews = await _context.ReviewProducts
                .Where(r => r.ProductId == productId &&
                            r.Status == "Approved" &&
                            r.Visibility == "Public" &&
                            !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync();

            if (!approvedReviews.Any())
            {
                return (0m, 0, new Dictionary<int, int>
                {
                    { 5, 0 }, { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 }
                });
            }

            var averageRating = (decimal)approvedReviews.Average();
            var totalReviews = approvedReviews.Count;
            var ratingDistribution = new Dictionary<int, int>
            {
                { 5, approvedReviews.Count(r => r == 5) },
                { 4, approvedReviews.Count(r => r == 4) },
                { 3, approvedReviews.Count(r => r == 3) },
                { 2, approvedReviews.Count(r => r == 2) },
                { 1, approvedReviews.Count(r => r == 1) }
            };

            return (averageRating, totalReviews, ratingDistribution);
        }

        // === ADMIN METHODS ===
        public async Task<(List<ReviewProduct> Items, int Total)> GetAdminPagedAsync(
            int? productId,
            string? status,
            int? rating,
            string? searchKeyword,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            var query = _context.ReviewProducts
                .Include(r => r.Account)
                .Include(r => r.Product)
                .Include(r => r.ReviewProductImages.Where(i => !i.IsDeleted))
                .Include(r => r.ReviewProductReactions.Where(rr => !rr.IsDeleted))
                .Include(r => r.ReviewProductReplies.Where(rp => !rp.IsDeleted))
                .Where(r => !r.IsDeleted);

            // Filter by ProductId
            if (productId.HasValue)
            {
                query = query.Where(r => r.ProductId == productId.Value);
            }

            // Filter by Status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(r => r.Status == status);
            }

            // Filter by Rating
            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            // Search by keyword (in comment, account name, product name)
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                var keyword = searchKeyword.ToLower();
                query = query.Where(r =>
                    r.Comment!.ToLower().Contains(keyword) ||
                    r.Account.AccountName.ToLower().Contains(keyword) ||
                    r.Product.ProductName.ToLower().Contains(keyword));
            }

            // Filter by date range
            if (fromDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= toDate.Value.AddDays(1));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task SoftDeleteAsync(int reviewId)
        {
            var review = await _context.ReviewProducts.FindAsync(reviewId);
            if (review != null)
            {
                review.IsDeleted = true;
                review.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
