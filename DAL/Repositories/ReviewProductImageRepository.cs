using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewProductImageRepository : IReviewProductImageRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewProductImageRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReviewProductImage image)
        {
            await _context.ReviewProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ReviewProductImage> images)
        {
            await _context.ReviewProductImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReviewProductImage>> GetByReviewIdAsync(int reviewId)
        {
            return await _context.ReviewProductImages
                .Where(i => i.ReviewProductId == reviewId && !i.IsDeleted)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteRange(IEnumerable<ReviewProductImage> images)
        {
            foreach (var img in images)
            {
                img.IsDeleted = true;
                img.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
