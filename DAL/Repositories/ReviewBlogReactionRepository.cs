using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ReviewBlogReactionRepository
    : IReviewBlogReactionRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewBlogReactionRepository(
            AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<ReviewBlogReaction?> GetAsync(
            int reviewBlogId,
            int accountId)
        {
            return await _context.ReviewBlogReactions
                .FirstOrDefaultAsync(x =>
                    x.ReviewBlogId == reviewBlogId &&
                    x.AccountId == accountId);
        }

        public async Task AddAsync(ReviewBlogReaction reaction)
        {
            await _context.ReviewBlogReactions.AddAsync(reaction);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(ReviewBlogReaction reaction)
        {
            _context.ReviewBlogReactions.Remove(reaction);
            await Task.CompletedTask;
        }
        public async Task<List<ReviewBlogReaction>>
            GetByReviewBlogIdAsync(int reviewBlogId)
        {
            return await _context.ReviewBlogReactions
                .Where(x => x.ReviewBlogId == reviewBlogId)
                .ToListAsync();
        }
        public async Task<int> CountByTypeAsync(
    int reviewBlogId,
    string type)
        {
            return await _context.ReviewBlogReactions
                .CountAsync(x =>
                    x.ReviewBlogId == reviewBlogId &&
                    x.ReactionType == type);
        }

        public async Task<string?> GetUserReactionAsync(
            int reviewBlogId,
            int accountId)
        {
            return await _context.ReviewBlogReactions
                .Where(x =>
                    x.ReviewBlogId == reviewBlogId &&
                    x.AccountId == accountId)
                .Select(x => x.ReactionType)
                .FirstOrDefaultAsync();
        }
    }
}
