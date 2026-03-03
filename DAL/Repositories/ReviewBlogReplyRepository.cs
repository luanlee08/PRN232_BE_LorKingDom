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
    public class ReviewBlogReplyRepository
    : IReviewBlogReplyRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewBlogReplyRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ReviewBlogReply reply)
        {
            await _context.ReviewBlogReplies.AddAsync(reply);
        }

        public async Task<List<ReviewBlogReply>>
            GetByReviewIdAsync(int reviewBlogId)
        {
            return await _context.ReviewBlogReplies
                .Include(x => x.Account)
                .Where(x => x.ReviewBlogId == reviewBlogId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
