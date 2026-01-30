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
    public class OriginRepository : IOriginRepository
    {
        private readonly AspLorKingDomContext _context;

        public OriginRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<List<Origin>> GetAllAsync()
        {
            return await _context.Origins
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
