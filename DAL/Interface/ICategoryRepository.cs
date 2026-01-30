using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface ICategoryRepository
    {
        Task<(List<Category> Items, int TotalCount)> GetAsync(
            string? keyword,
            int? superCategoryId,
            int page,
            int pageSize);

        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category entity);
        Task<bool> IsNameExistAsync(
            string name,
            int superCategoryId,
            int? excludeId = null);
        Task DisableBySuperCategoryAsync(int superCategoryId);

        Task<List<Category>> GetActiveAsync(int? superCategoryId);
        Task SaveChangesAsync();
    }
}
