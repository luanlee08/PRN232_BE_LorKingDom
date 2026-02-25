using BLL.DTOs;
using BLL.DTOs.Blog;
using BLL.Interfaces;
using DAL.Interface;

namespace BLL.Services
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly IBlogCategoryRepository _repo;

        public BlogCategoryService(IBlogCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<BlogCategoryResponse>>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();

            return new ApiResponse<List<BlogCategoryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách BlogCategory",
                Data = categories.Select(c => new BlogCategoryResponse
                {
                    BlogCategoryId = c.BlogCategoryId,
                    BlogCategoryName = c.BlogCategoryName
                }).ToList()
            };
        }
    }
}