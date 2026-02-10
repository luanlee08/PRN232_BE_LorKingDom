using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewModerationLogRepository : IReviewModerationLogRepository
    {
        private readonly AspLorKingDomContext _context;
        public ReviewModerationLogRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReviewModerationLog log)
        {
            await _context.ReviewModerationLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReviewModerationLog>> GetByReviewIdAsync(int reviewId)
        {
            return await _context.ReviewModerationLogs
                .Where(l => l.ReviewProductId == reviewId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
