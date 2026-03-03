using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(string? keyword);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task<bool> ExistsBySkuAsync(string sku);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<(List<Product> Items, int Total)> QueryStorefrontPagedAsync(
     string? keyword, int page, int pageSize, int? priceRangeId = null);
        Task<(List<Product> Items, int Total)> QueryAdminPagedAsync(string? keyword, int page, int pageSize);
            
        Task<List<Product>> GetAvailableProductsAsync(string? keyword);


    }
}
