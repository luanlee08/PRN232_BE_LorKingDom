using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface ISuperCategoryRepository
    {
        Task<(List<SuperCategory> Items, int TotalCount)> GetAsync(
            string? keyword,
            int page,
            int pageSize
        );

        Task<SuperCategory?> GetByIdAsync(int id);

        Task AddAsync(SuperCategory entity);

        Task<List<SuperCategory>> GetActiveAsync();

        Task<bool> IsNameExistAsync(string name, int? excludeId = null);

        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int id);

    }
}
