using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IBrandRepository
    {
        Task<(List<Brand> Items, int TotalCount)> GetAsync(
            string? keyword,
            int page,
            int pageSize);

        Task<Brand?> GetByIdAsync(int id);
        Task AddAsync(Brand entity);
        Task<bool> IsNameExistAsync(string name, int? excludeId = null);
        Task<List<Brand>> GetActiveAsync();
        Task SaveChangesAsync();
    }
}
