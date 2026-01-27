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
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AspLorKingDomContext _context;

        public MaterialRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Material>, int)> GetAsync(
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Materials
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.MaterialName.Contains(keyword));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Material?> GetByIdAsync(int id)
            => await _context.Materials
                .FirstOrDefaultAsync(x => x.MaterialId == id);

        public async Task AddAsync(Material entity)
            => await _context.Materials.AddAsync(entity);

        public async Task<bool> IsNameExistAsync(string name, int? excludeId = null)
        {
            var q = _context.Materials
                .Where(x => x.MaterialName == name);

            if (excludeId.HasValue)
                q = q.Where(x => x.MaterialId != excludeId.Value);

            return await q.AnyAsync();
        }

        public async Task<List<Material>> GetActiveAsync()
        {
            return await _context.Materials
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
