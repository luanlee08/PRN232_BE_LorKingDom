using BLL.DTOs;
using BLL.DTOs.Blog;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepo;
        private readonly IBlogCategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _env;

        public BlogService(
            IBlogRepository blogRepo,
            IBlogCategoryRepository categoryRepo,
            IWebHostEnvironment env)
        {
            _blogRepo = blogRepo;
            _categoryRepo = categoryRepo;
            _env = env;
        }

        /* ================= ADMIN ================= */

        public async Task<ApiResponse<int>> CreateAsync(
            CreateBlogRequest request,
            int accountId)
        {
            var categories = await _categoryRepo
                .GetByIdsAsync(new List<int> { request.BlogCategoryId });

            if (!categories.Any())
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "BlogCategory không hợp lệ"
                };
            }

            string? thumbnailUrl = null;

            if (request.BlogThumbnail != null)
                thumbnailUrl = await SaveThumbnailAsync(request.BlogThumbnail);

            var blog = new BlogPost
            {
                BlogTitle = request.BlogTitle,
                BlogContent = request.BlogContent,
                BlogThumbnail = thumbnailUrl,
                AccountId = accountId,
                IsPublished = request.IsPublished,
                IsFeatured = request.IsFeatured,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                BlogCategories = categories
            };

            await _blogRepo.AddAsync(blog);
            await _blogRepo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo Blog thành công",
                Data = blog.BlogPostId
            };
        }

        public async Task<ApiResponse<PagedResult<BlogAdminResponse>>>
            SearchForAdminAsync(SearchBlogAdminRequest request)
        {
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var (blogs, total) =
                await _blogRepo.SearchForAdminAsync(
                    request.Keyword,
                    request.Page,
                    request.PageSize);

            var result = new PagedResult<BlogAdminResponse>
            {
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize,
                Items = blogs.Select(b => new BlogAdminResponse
                {
                    BlogPostId = b.BlogPostId,
                    BlogTitle = b.BlogTitle,
                    BlogContent = b.BlogContent,
                    BlogThumbnail = b.BlogThumbnail,
                    CategoryId = b.BlogCategories.FirstOrDefault()?.BlogCategoryId ?? 0,
                    BlogCategory = b.BlogCategories.FirstOrDefault()?.BlogCategoryName ?? "—",
                    AuthorEmail = b.Account.Email,
                    IsPublished = b.IsPublished,
                    IsFeatured = b.IsFeatured,
                    IsDeleted = b.IsDeleted,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };

            return new ApiResponse<PagedResult<BlogAdminResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Blog (Admin)",
                Data = result
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(
            int blogId,
            UpdateBlogRequest request)
        {
            var blog = await _blogRepo.GetByIdAsync(blogId);

            if (blog == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Blog không tồn tại",
                    Data = false
                };
            }

            var categories = await _categoryRepo
                .GetByIdsAsync(new List<int> { request.BlogCategoryId });

            if (!categories.Any())
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "BlogCategory không hợp lệ",
                    Data = false
                };
            }

            blog.BlogTitle = request.BlogTitle;
            blog.BlogContent = request.BlogContent;
            blog.IsPublished = request.IsPublished;
            blog.IsFeatured = request.IsFeatured;
            blog.IsDeleted = request.IsDeleted;
            blog.BlogCategories = categories;

            if (request.BlogThumbnail != null)
                blog.BlogThumbnail = await SaveThumbnailAsync(request.BlogThumbnail);

            await _blogRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật Blog thành công",
                Data = true
            };
        }

        /* ================= PUBLIC ================= */

        public async Task<ApiResponse<PagedResult<BlogPublicResponse>>> GetPublicAsync(
            string? keyword,
            int? categoryId,
            int page,
            int pageSize)
        {
            var (blogs, total) =
                await _blogRepo.GetPublicAsync(
                    keyword, categoryId, page, pageSize);

            var result = new PagedResult<BlogPublicResponse>
            {
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                Items = blogs.Select(b => new BlogPublicResponse
                {
                    BlogPostId = b.BlogPostId,
                    BlogTitle = b.BlogTitle,
                    BlogExcerpt = b.BlogContent.Length > 150
                        ? b.BlogContent.Substring(0, 150) + "..."
                        : b.BlogContent,
                    BlogThumbnail = b.BlogThumbnail,
                    BlogCategory = b.BlogCategories
                    .FirstOrDefault()?.BlogCategoryName ?? "—",
                    AuthorEmail = b.Account.Email,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };

            return new ApiResponse<PagedResult<BlogPublicResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Blog (Public)",
                Data = result
            };
        }

        public async Task<ApiResponse<BlogDetailResponse>> GetPublicDetailAsync(int blogId)
        {
            var blog = await _blogRepo.GetPublicDetailAsync(blogId);

            if (blog == null)
            {
                return new ApiResponse<BlogDetailResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Blog không tồn tại"
                };
            }

            return new ApiResponse<BlogDetailResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Chi tiết Blog",
                Data = new BlogDetailResponse
                {
                    BlogPostId = blog.BlogPostId,
                    BlogTitle = blog.BlogTitle,
                    BlogContent = blog.BlogContent,
                    BlogThumbnail = blog.BlogThumbnail,
                    BlogCategory = blog.BlogCategories
                    .FirstOrDefault()?.BlogCategoryName ?? "—",
                    AuthorEmail = blog.Account.Email,
                    CreatedAt = blog.CreatedAt
                }
            };
        }

        public async Task<ApiResponse<List<BlogPublicResponse>>> GetRecentAsync(int limit)
        {
            var blogs = await _blogRepo.GetRecentAsync(limit);

            return new ApiResponse<List<BlogPublicResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Blog mới nhất",
                Data = blogs.Select(b => new BlogPublicResponse
                {
                    BlogPostId = b.BlogPostId,
                    BlogTitle = b.BlogTitle,
                    BlogExcerpt = b.BlogContent.Length > 120
                        ? b.BlogContent.Substring(0, 120) + "..."
                        : b.BlogContent,
                    BlogThumbnail = b.BlogThumbnail,
                    BlogCategory = b.BlogCategories
                   .FirstOrDefault()?.BlogCategoryName ?? "—",
                    AuthorEmail = b.Account.Email,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };
        }

        /* ================= HELPER ================= */

        private async Task<string> SaveThumbnailAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/blogs");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/blogs/{fileName}";
        }

        public async Task<ApiResponse<List<BlogPublicResponse>>> GetFeaturedAsync(int limit)
        {
            var blogs = await _blogRepo.GetFeaturedAsync(limit);

            return new ApiResponse<List<BlogPublicResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Blog nổi bật",
                Data = blogs.Select(b => new BlogPublicResponse
                {
                    BlogPostId = b.BlogPostId,
                    BlogTitle = b.BlogTitle,
                    BlogExcerpt = b.BlogContent.Length > 120
                        ? b.BlogContent.Substring(0, 120) + "..."
                        : b.BlogContent,
                    BlogThumbnail = b.BlogThumbnail,
                    BlogCategory = b.BlogCategories
                    .FirstOrDefault()?.BlogCategoryName ?? "—",
                    AuthorEmail = b.Account.Email,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };
        }
    }
}