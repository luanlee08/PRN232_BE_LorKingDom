using BLL.DTOs;
using BLL.DTOs.Brands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBrandService
    {
        Task<ApiResponse<PagedResult<BrandResponse>>> GetAsync(BrandQuery query);
        Task<ApiResponse<int>> CreateAsync(CreateBrandRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateBrandRequest request);
        Task<ApiResponse<List<BrandResponse>>> GetActiveAsync();
    }
}
