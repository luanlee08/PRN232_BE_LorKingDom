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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AspLorKingDomContext _context;

        public CategoryRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Category>, int)> GetAsync(
            string? keyword,
            int? superCategoryId,
            int page,
            int pageSize)
        {
            var query = _context.Categories
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x =>
                    x.CategoryName.Contains(keyword));

            if (superCategoryId.HasValue)
                query = query.Where(x =>
                    x.SuperCategoryId == superCategoryId);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
                .FirstOrDefaultAsync(x => x.CategoryId == id);

        public async Task AddAsync(Category entity)
            => await _context.Categories.AddAsync(entity);

        public async Task<bool> IsNameExistAsync(
            string name,
            int superCategoryId,
            int? excludeId = null)
        {
            var q = _context.Categories.Where(x =>
                x.CategoryName == name &&
                x.SuperCategoryId == superCategoryId);

            if (excludeId.HasValue)
                q = q.Where(x => x.CategoryId != excludeId);

            return await q.AnyAsync();
        }

        public async Task<List<Category>> GetActiveAsync(int? superCategoryId)
        {
            var q = _context.Categories
                .Where(x => !x.IsDeleted);

            if (superCategoryId.HasValue)
                q = q.Where(x =>
                    x.SuperCategoryId == superCategoryId);

            return await q
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
        public async Task DisableBySuperCategoryAsync(int superCategoryId)
        {
            var categories = await _context.Categories
                .Where(x =>
                    x.SuperCategoryId == superCategoryId &&
                    !x.IsDeleted)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.IsDeleted = true;
            }
        }

    }
}
