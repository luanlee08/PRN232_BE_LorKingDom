using BLL.DTOs;
using BLL.DTOs.Brands;
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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repo;

        public BrandService(IBrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<BrandResponse>>> GetAsync(BrandQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<BrandResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Brand",
                Data = new PagedResult<BrandResponse>
                {
                    Items = items.Select(x => new BrandResponse
                    {
                        BrandId = x.BrandId,
                        BrandName = x.BrandName,
                        IsDeleted = x.IsDeleted,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateBrandRequest request)
        {
            if (await _repo.IsNameExistAsync(request.BrandName))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Brand đã tồn tại"
                };
            }

            var entity = new Brand
            {
                BrandName = request.BrandName,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo Brand thành công",
                Data = entity.BrandId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateBrandRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Brand",
                    Data = false
                };
            }

            if (await _repo.IsNameExistAsync(request.BrandName, id))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Brand đã tồn tại",
                    Data = false
                };
            }

            entity.BrandName = request.BrandName;
            entity.IsDeleted = request.IsDeleted;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật Brand thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<List<BrandResponse>>> GetActiveAsync()
        {
            var items = await _repo.GetActiveAsync();

            return new ApiResponse<List<BrandResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Brand đang hoạt động",
                Data = items.Select(x => new BrandResponse
                {
                    BrandId = x.BrandId,
                    BrandName = x.BrandName,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
