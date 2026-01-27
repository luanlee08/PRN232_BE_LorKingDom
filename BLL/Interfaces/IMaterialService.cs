using BLL.DTOs;
using BLL.DTOs.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMaterialService
    {
        Task<ApiResponse<PagedResult<MaterialResponse>>> GetAsync(MaterialQuery query);
        Task<ApiResponse<int>> CreateAsync(CreateMaterialRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateMaterialRequest request);
        Task<ApiResponse<List<MaterialResponse>>> GetActiveAsync();
    }
}
