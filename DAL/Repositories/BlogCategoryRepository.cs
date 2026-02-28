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
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly AspLorKingDomContext _context;

        public BlogCategoryRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<List<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<BlogCategory>> GetByIdsAsync(List<int> ids)
        {
            return await _context.BlogCategories
                .Where(c => ids.Contains(c.BlogCategoryId) && !c.IsDeleted)
                .ToListAsync();
        }
    }
}
