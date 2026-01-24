using BLL.DTOs;
using BLL.DTOs.SuperCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ISuperCategoryService
    {
        Task<ApiResponse<PagedResult<SuperCategoryResponse>>> GetAsync(SuperCategoryQuery query);
        Task<ApiResponse<int>> CreateAsync(CreateSuperCategoryRequest request);

        Task<ApiResponse<List<SuperCategoryResponse>>> GetActiveAsync();
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateSuperCategoryRequest request);

    }
}
