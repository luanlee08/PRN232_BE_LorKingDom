using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IMaterialRepository
    {
        Task<(List<Material> Items, int TotalCount)> GetAsync(
            string? keyword,
            int page,
            int pageSize);

        Task<Material?> GetByIdAsync(int id);
        Task AddAsync(Material entity);
        Task<bool> IsNameExistAsync(string name, int? excludeId = null);
        Task<List<Material>> GetActiveAsync();
        Task SaveChangesAsync();
    }
}
