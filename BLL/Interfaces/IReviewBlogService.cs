using BLL.DTOs;
using BLL.DTOs.ReviewBlog;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IReviewBlogService
    {
        Task<ApiResponse<bool>> CreateAsync(int accountId, CreateReviewBlogRequest request);

        Task<ApiResponse<List<ReviewBlogResponse>>> GetByBlogIdAsync(int blogPostId);

        Task<ApiResponse<PagedResult<ReviewBlogAdminDto>>>
            GetAllAsync(int page, int pageSize);

        Task<ApiResponse<bool>> BlockAsync(int reviewId, bool isBlocked);
    }
}
