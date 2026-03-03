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
    public class ReviewBlogRepository : IReviewBlogRepository
    {
        private readonly AspLorKingDomContext _context;

        public ReviewBlogRepository(AspLorKingDomContext context)
        {
            _context = context;
        }
        public IQueryable<ReviewBlog> GetQueryable()
        {
            return _context.ReviewBlogs
                .Include(x => x.Account)
                .Include(x => x.BlogPost);
        }
        public async Task AddAsync(ReviewBlog review)
        {
            await _context.ReviewBlogs.AddAsync(review);
        }

        public async Task<ReviewBlog?> GetByIdAsync(int id)
        {
            return await _context.ReviewBlogs
                .Include(x => x.Account)
                .Include(x => x.BlogPost)
                .FirstOrDefaultAsync(r => r.ReviewBlogId == id);
        }

        public async Task<List<ReviewBlog>>
    GetByBlogIdAsync(int blogPostId)
        {
            return await _context.ReviewBlogs
                .Include(r => r.Account)
                .Include(r => r.ReviewBlogReactions)
                .Include(r => r.ReviewBlogReplies)
                    .ThenInclude(reply => reply.Account)  
                .Where(r =>
                    r.BlogPostId == blogPostId &&
                    !r.IsBlocked)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
