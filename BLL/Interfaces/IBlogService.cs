using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOs;
using BLL.DTOs.Blog;

namespace BLL.Interfaces
{
    public interface IBlogService
    {
        Task<ApiResponse<int>> CreateAsync(CreateBlogRequest request, int accountId);

        Task<ApiResponse<PagedResult<BlogAdminResponse>>>
            SearchForAdminAsync(SearchBlogAdminRequest request);

        Task<ApiResponse<bool>> UpdateAsync(int blogId, UpdateBlogRequest request);

        Task<ApiResponse<PagedResult<BlogPublicResponse>>> GetPublicAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize
        );

        Task<ApiResponse<BlogDetailResponse>> GetPublicDetailAsync(int blogId);

        Task<ApiResponse<List<BlogPublicResponse>>> GetRecentAsync(int limit);
    }
}
