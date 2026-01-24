using BLL.DTOs;
using BLL.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<PagedResult<CategoryResponse>>> GetAsync(CategoryQuery query);
        Task<ApiResponse<int>> CreateAsync(CreateCategoryRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateCategoryRequest request);
        Task<ApiResponse<List<CategoryResponse>>> GetActiveAsync(int? superCategoryId);
    }
}
