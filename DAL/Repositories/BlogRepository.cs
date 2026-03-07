using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AspLorKingDomContext _context;

        public BlogRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlogPost blog)
        {
            await _context.BlogPosts.AddAsync(blog);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(List<BlogPost> Data, int Total)>
SearchForAdminAsync(string? keyword, int page, int pageSize)
        {
            IQueryable<BlogPost> query = _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b =>
                    b.BlogTitle.Contains(keyword) ||
                    b.BlogContent.Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }


        public async Task<BlogPost?> GetByIdAsync(int blogId)
        {
            return await _context.BlogPosts
                .Include(b => b.BlogCategories)
                .FirstOrDefaultAsync(b =>
                    b.BlogPostId == blogId);
        }

        public async Task<(List<BlogPost>, int)> GetPublicAsync(
    string? keyword,
    int? categoryId,
    int page,
    int pageSize)
        {
            var query = _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories)
                .Where(b =>
                    b.IsPublished &&
                    !b.IsDeleted
                );

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(b =>
                    b.BlogTitle.Contains(keyword) ||
                    b.BlogContent.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.BlogCategories.Any(c => c.BlogCategoryId == categoryId));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }
        public async Task<BlogPost?> GetPublicByIdAsync(int blogId)
        {
            return await _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories)
                .FirstOrDefaultAsync(b =>
                    b.BlogPostId == blogId &&
                    b.IsPublished &&
                    !b.IsDeleted
                );
        }
        public async Task<List<BlogPost>> GetRecentAsync(int limit)
        {
            return await _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories)
                .Where(b => b.IsPublished && !b.IsDeleted)
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
        public async Task<BlogPost?> GetPublicDetailAsync(int blogId)
        {
            return await _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories)
                .Where(b =>
                    b.BlogPostId == blogId &&
                    b.IsPublished &&
                    !b.IsDeleted)
                .FirstOrDefaultAsync();
        }
        public async Task<List<BlogPost>> GetFeaturedAsync(int limit)
        {
            return await _context.BlogPosts
                .Include(b => b.Account)
                .Include(b => b.BlogCategories)
                .Where(b =>
                    b.IsPublished &&
                    !b.IsDeleted &&
                    b.IsFeatured
                )
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetFeaturedForRotationAsync()
        {
            return await _context.BlogPosts
                .Where(b => b.IsFeatured && !b.IsDeleted)
                .OrderBy(b => b.CreatedAt)
                .ThenBy(b => b.BlogPostId)
                .ToListAsync();
        }
    }
}
