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
    public class SexRepository : ISexRepository
    {
        private readonly AspLorKingDomContext _context;

        public SexRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<List<Sex>> GetAllAsync()
        {
            return await _context.Sexes
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
