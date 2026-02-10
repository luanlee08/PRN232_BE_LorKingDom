using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewProductReactionRepository : IReviewProductReactionRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewProductReactionRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<ReviewProductReaction?> GetReactionAsync(int reviewId, int accountId)
        {
            return await _context.ReviewProductReactions
                .FirstOrDefaultAsync(r =>
                    r.ReviewProductId == reviewId &&
                    r.AccountId == accountId &&
                    !r.IsDeleted);
        }

        public async Task AddAsync(ReviewProductReaction reaction)
        {
            await _context.ReviewProductReactions.AddAsync(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReviewProductReaction reaction)
        {
            reaction.CreatedAt = DateTime.UtcNow;
            _context.ReviewProductReactions.Update(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetLikeCountAsync(int reviewId)
        {
            return await _context.ReviewProductReactions
                .Where(r =>
                    r.ReviewProductId == reviewId &&
                    r.ReactionType == "Like" &&
                    !r.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetDislikeCountAsync(int reviewId)
        {
            return await _context.ReviewProductReactions
                .Where(r =>
                    r.ReviewProductId == reviewId &&
                    r.ReactionType == "Dislike" &&
                    !r.IsDeleted)
                .CountAsync();
        }
    }
}
