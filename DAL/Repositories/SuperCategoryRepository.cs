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
    public class SuperCategoryRepository : ISuperCategoryRepository
    {
        private readonly AspLorKingDomContext _context;

        public SuperCategoryRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        // LIST + SEARCH + PAGING
        public async Task<(List<SuperCategory> Items, int TotalCount)> GetAsync(
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.SuperCategories
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.SuperCategoryName.Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<SuperCategory?> GetByIdAsync(int id)
            => await _context.SuperCategories
                .FirstOrDefaultAsync(x => x.SuperCategoryId == id);

        public async Task AddAsync(SuperCategory entity)
        {
            await _context.SuperCategories.AddAsync(entity);
        }

        public async Task<List<SuperCategory>> GetActiveAsync()
        {
            return await _context.SuperCategories
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsNameExistAsync(string name, int? excludeId = null)
        {
            var q = _context.SuperCategories
                .Where(x => x.SuperCategoryName == name);

            if (excludeId.HasValue)
                q = q.Where(x => x.SuperCategoryId != excludeId.Value);

            return await q.AnyAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.SuperCategories
                .AnyAsync(x => x.SuperCategoryId == id && !x.IsDeleted);
        }

    }
}
