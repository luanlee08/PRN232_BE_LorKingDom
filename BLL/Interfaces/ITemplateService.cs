using BLL.DTOs;
using BLL.DTOs.Templates;

namespace BLL.Interfaces
{
    public interface ITemplateService
    {
        Task<ApiResponse<PagedResult<TemplateResponse>>> GetAsync(TemplateQuery query);
        Task<ApiResponse<TemplateResponse>> GetByIdAsync(short id);
        Task<ApiResponse<TemplateResponse>> GetByCodeAsync(string templateCode);
        Task<ApiResponse<List<TemplateResponse>>> GetActiveAsync();
        Task<ApiResponse<short>> CreateAsync(CreateTemplateRequest request);
        Task<ApiResponse<bool>> UpdateAsync(short id, UpdateTemplateRequest request);
        Task<ApiResponse<bool>> ToggleStatusAsync(short id);
    }
}
