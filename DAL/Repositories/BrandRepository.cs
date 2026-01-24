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
    public class BrandRepository : IBrandRepository
    {
        private readonly AspLorKingDomContext _context;

        public BrandRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Brand>, int)> GetAsync(
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Brands
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.BrandName.Contains(keyword));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Brand?> GetByIdAsync(int id)
            => await _context.Brands
                .FirstOrDefaultAsync(x => x.BrandId == id);

        public async Task AddAsync(Brand entity)
            => await _context.Brands.AddAsync(entity);

        public async Task<bool> IsNameExistAsync(string name, int? excludeId = null)
        {
            var q = _context.Brands
                .Where(x => x.BrandName == name);

            if (excludeId.HasValue)
                q = q.Where(x => x.BrandId != excludeId.Value);

            return await q.AnyAsync();
        }

        public async Task<List<Brand>> GetActiveAsync()
        {
            return await _context.Brands
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
