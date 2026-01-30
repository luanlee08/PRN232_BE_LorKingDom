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
    public class AgeRepository : IAgeRepository
    {
        private readonly AspLorKingDomContext _context;

        public AgeRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<List<Age>> GetAllAsync()
        {
            return await _context.Ages
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
