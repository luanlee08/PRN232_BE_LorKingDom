using BLL.DTOs;
using BLL.DTOs.Templates;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepo;

        public TemplateService(ITemplateRepository templateRepo)
        {
            _templateRepo = templateRepo;
        }

        public async Task<ApiResponse<PagedResult<TemplateResponse>>> GetAsync(TemplateQuery query)
        {
            var (items, total) = await _templateRepo.GetAsync(
                query.Keyword,
                query.IsActive,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<TemplateResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Template thành công",
                Data = new PagedResult<TemplateResponse>
                {
                    Items = items.Select(t => MapToResponse(t)).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<TemplateResponse>> GetByIdAsync(short id)
        {
            var template = await _templateRepo.GetByIdAsync(id);

            if (template == null)
            {
                return new ApiResponse<TemplateResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Template"
                };
            }

            return new ApiResponse<TemplateResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin Template thành công",
                Data = MapToResponse(template)
            };
        }

        public async Task<ApiResponse<TemplateResponse>> GetByCodeAsync(string templateCode)
        {
            var template = await _templateRepo.GetByCodeAsync(templateCode);

            if (template == null)
            {
                return new ApiResponse<TemplateResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Template"
                };
            }

            return new ApiResponse<TemplateResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin Template thành công",
                Data = MapToResponse(template)
            };
        }

        public async Task<ApiResponse<List<TemplateResponse>>> GetActiveAsync()
        {
            var templates = await _templateRepo.GetActiveTemplatesAsync();

            return new ApiResponse<List<TemplateResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Template active thành công",
                Data = templates.Select(t => MapToResponse(t)).ToList()
            };
        }

        public async Task<ApiResponse<short>> CreateAsync(CreateTemplateRequest request)
        {
            if (await _templateRepo.IsCodeExistAsync(request.TemplateCode))
            {
                return new ApiResponse<short>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TemplateCode đã tồn tại"
                };
            }

            var entity = new Template
            {
                TemplateCode = request.TemplateCode,
                TitleTemplate = request.TitleTemplate,
                MessageTemplate = request.MessageTemplate,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _templateRepo.AddAsync(entity);
            await _templateRepo.SaveChangesAsync();

            return new ApiResponse<short>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo Template thành công",
                Data = entity.TemplateId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(short id, UpdateTemplateRequest request)
        {
            var entity = await _templateRepo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Template",
                    Data = false
                };
            }

            if (await _templateRepo.IsCodeExistAsync(request.TemplateCode, id))
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "TemplateCode đã tồn tại",
                    Data = false
                };
            }

            entity.TemplateCode = request.TemplateCode;
            entity.TitleTemplate = request.TitleTemplate;
            entity.MessageTemplate = request.MessageTemplate;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _templateRepo.UpdateAsync(entity);
            await _templateRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật Template thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(short id)
        {
            var entity = await _templateRepo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy Template",
                    Data = false
                };
            }

            entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _templateRepo.UpdateAsync(entity);
            await _templateRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Đã {(entity.IsActive ? "kích hoạt" : "vô hiệu hóa")} Template",
                Data = true
            };
        }

        private TemplateResponse MapToResponse(Template template)
        {
            return new TemplateResponse
            {
                TemplateId = template.TemplateId,
                TemplateCode = template.TemplateCode,
                TitleTemplate = template.TitleTemplate,
                MessageTemplate = template.MessageTemplate,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }
    }
}
