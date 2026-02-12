using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewProductReplyRepository : IReviewProductReplyRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewProductReplyRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<ReviewProductReply> AddAsync(ReviewProductReply reply)
        {
            await _context.ReviewProductReplies.AddAsync(reply);
            await _context.SaveChangesAsync();
            return reply;
        }

        public async Task<List<ReviewProductReply>> GetByReviewIdAsync(int reviewId)
        {
            return await _context.ReviewProductReplies
                .Include(r => r.Account)
                .Where(r => r.ReviewProductId == reviewId && !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ReviewProductReply?> GetByIdAsync(int replyId)
        {
            return await _context.ReviewProductReplies
                .Include(r => r.Account)
                .FirstOrDefaultAsync(r => r.ReplyProductId == replyId && !r.IsDeleted);
        }

        public async Task UpdateAsync(ReviewProductReply reply)
        {
            _context.ReviewProductReplies.Update(reply);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int replyId)
        {
            var reply = await _context.ReviewProductReplies.FindAsync(replyId);
            if (reply != null)
            {
                reply.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
