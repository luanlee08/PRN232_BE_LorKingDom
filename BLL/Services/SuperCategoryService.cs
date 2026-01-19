using BLL.DTOs;
using BLL.DTOs.SuperCategories;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SuperCategoryService : ISuperCategoryService
    {
        private readonly ISuperCategoryRepository _repo;

        public SuperCategoryService(ISuperCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<SuperCategoryResponse>>> GetAsync(SuperCategoryQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword, query.Page, query.PageSize);

            return new ApiResponse<PagedResult<SuperCategoryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách SuperCategory",
                Data = new PagedResult<SuperCategoryResponse>
                {
                    Items = items.Select(x => new SuperCategoryResponse
                    {
                        SuperCategoryId = x.SuperCategoryId,
                        SuperCategoryName = x.SuperCategoryName,
                        IsDeleted = x.IsDeleted,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateSuperCategoryRequest request)
        {
            if (await _repo.IsNameExistAsync(request.SuperCategoryName))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên SuperCategory đã tồn tại"
                };
            }

            var entity = new SuperCategory
            {
                SuperCategoryName = request.SuperCategoryName,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo SuperCategory thành công",
                Data = entity.SuperCategoryId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateSuperCategoryRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy SuperCategory",
                    Data = false
                };
            }

            if (await _repo.IsNameExistAsync(request.SuperCategoryName, id))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên SuperCategory đã tồn tại",
                    Data = false
                };
            }

            entity.SuperCategoryName = request.SuperCategoryName;
            entity.IsDeleted = request.IsDeleted;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật SuperCategory thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<List<SuperCategoryResponse>>> GetActiveAsync()
        {
            var items = await _repo.GetActiveAsync();

            return new ApiResponse<List<SuperCategoryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách SuperCategory đang hoạt động",
                Data = items.Select(x => new SuperCategoryResponse
                {
                    SuperCategoryId = x.SuperCategoryId,
                    SuperCategoryName = x.SuperCategoryName,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
