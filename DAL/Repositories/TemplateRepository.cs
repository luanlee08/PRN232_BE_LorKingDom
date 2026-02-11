using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly AspLorKingDomContext _context;

        public TemplateRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Template>, int)> GetAsync(
            string? keyword,
            bool? isActive,
            int page,
            int pageSize)
        {
            var query = _context.Templates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(t =>
                    t.TemplateCode.Contains(keyword) ||
                    t.TitleTemplate.Contains(keyword) ||
                    t.MessageTemplate.Contains(keyword));
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Template?> GetByIdAsync(short id)
        {
            return await _context.Templates
                .FirstOrDefaultAsync(t => t.TemplateId == id);
        }

        public async Task<Template?> GetByCodeAsync(string templateCode)
        {
            return await _context.Templates
                .FirstOrDefaultAsync(t => t.TemplateCode == templateCode && t.IsActive);
        }

        public async Task<List<Template>> GetActiveTemplatesAsync()
        {
            return await _context.Templates
                .Where(t => t.IsActive)
                .OrderBy(t => t.TemplateCode)
                .ToListAsync();
        }

        public async Task AddAsync(Template entity)
        {
            await _context.Templates.AddAsync(entity);
        }

        public async Task UpdateAsync(Template entity)
        {
            _context.Templates.Update(entity);
        }

        public async Task<bool> IsCodeExistAsync(string code, short? excludeId = null)
        {
            var query = _context.Templates
                .Where(t => t.TemplateCode == code);

            if (excludeId.HasValue)
                query = query.Where(t => t.TemplateId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
