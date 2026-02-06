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
    public class PriceRangeRepository : IPriceRangeRepository
    {
        private readonly AspLorKingDomContext _context;

        public PriceRangeRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<List<PriceRange>> GetAllAsync()
        {
            return await _context.PriceRanges
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
