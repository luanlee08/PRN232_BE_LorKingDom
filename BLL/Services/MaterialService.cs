using BLL.DTOs;
using BLL.DTOs.Materials;
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
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _repo;

        public MaterialService(IMaterialRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<MaterialResponse>>> GetAsync(MaterialQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<MaterialResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Material",
                Data = new PagedResult<MaterialResponse>
                {
                    Items = items.Select(x => new MaterialResponse
                    {
                        MaterialId = x.MaterialId,
                        MaterialName = x.MaterialName,
                        Description = x.Description,
                        IsDeleted = x.IsDeleted,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateMaterialRequest request)
        {
            if (await _repo.IsNameExistAsync(request.MaterialName))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Material đã tồn tại"
                };
            }

            var entity = new Material
            {
                MaterialName = request.MaterialName,
                Description = request.Description,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo Material thành công",
                Data = entity.MaterialId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateMaterialRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Material",
                    Data = false
                };
            }

            if (await _repo.IsNameExistAsync(request.MaterialName, id))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Tên Material đã tồn tại",
                    Data = false
                };
            }

            entity.MaterialName = request.MaterialName;
            entity.Description = request.Description;
            entity.IsDeleted = request.IsDeleted;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật Material thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<List<MaterialResponse>>> GetActiveAsync()
        {
            var items = await _repo.GetActiveAsync();

            return new ApiResponse<List<MaterialResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách Material đang hoạt động",
                Data = items.Select(x => new MaterialResponse
                {
                    MaterialId = x.MaterialId,
                    MaterialName = x.MaterialName,
                    Description = x.Description,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
