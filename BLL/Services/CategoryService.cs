using BLL.DTOs;
using BLL.DTOs.Categories;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ISuperCategoryRepository _superRepo;

        public CategoryService(
            ICategoryRepository repo,
            ISuperCategoryRepository superRepo)
        {
            _repo = repo;
            _superRepo = superRepo;
        }

        public async Task<ApiResponse<PagedResult<CategoryResponse>>> GetAsync(CategoryQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword,
                query.SuperCategoryId,
                query.Page,
                query.PageSize
            );

            return new ApiResponse<PagedResult<CategoryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Category",
                Data = new PagedResult<CategoryResponse>
                {
                    Items = items.Select(x => new CategoryResponse
                    {
                        CategoryId = x.CategoryId,
                        SuperCategoryId = x.SuperCategoryId,
                        CategoryName = x.CategoryName,
                        IsDeleted = x.IsDeleted,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateCategoryRequest request)
        {
            // check SuperCategory tồn tại
            if (!await _superRepo.ExistsAsync(request.SuperCategoryId))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "SuperCategory không tồn tại"
                };
            }

            // check trùng tên trong cùng SuperCategory
            if (await _repo.IsNameExistAsync(
                request.CategoryName,
                request.SuperCategoryId))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Category đã tồn tại"
                };
            }

            var entity = new Category
            {
                CategoryName = request.CategoryName,
                SuperCategoryId = request.SuperCategoryId,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo Category thành công",
                Data = entity.CategoryId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Category",
                    Data = false
                };
            }

            // check SuperCategory tồn tại
            if (!await _superRepo.ExistsAsync(request.SuperCategoryId))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "SuperCategory không tồn tại",
                    Data = false
                };
            }

            // check trùng tên (exclude chính nó)
            if (await _repo.IsNameExistAsync(
                request.CategoryName,
                request.SuperCategoryId,
                id))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Category đã tồn tại",
                    Data = false
                };
            }

            entity.CategoryName = request.CategoryName;
            entity.SuperCategoryId = request.SuperCategoryId;
            entity.IsDeleted = request.IsDeleted;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật Category thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<List<CategoryResponse>>> GetActiveAsync(int? superCategoryId)
        {
            var items = await _repo.GetActiveAsync(superCategoryId);

            return new ApiResponse<List<CategoryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Category đang hoạt động",
                Data = items.Select(x => new CategoryResponse
                {
                    CategoryId = x.CategoryId,
                    SuperCategoryId = x.SuperCategoryId,
                    CategoryName = x.CategoryName,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
