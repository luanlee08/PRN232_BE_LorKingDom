using BLL.DTOs;
using BLL.DTOs.Vouchers;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<ApiResponse<PagedResult<VoucherResponse>>> GetVouchersAsync(VoucherQuery request)
        {
            var (items, totalCount) = await _voucherRepository.GetVouchersAsync(
                request.VoucherCode,
                request.VoucherTypeId,
                request.Status,
                request.Page,
                request.PageSize);

            return new ApiResponse<PagedResult<VoucherResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách Voucher",
                Data = new PagedResult<VoucherResponse>
                {
                    Items = items.Select(MapToDTO).ToList(),
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                }
            };
        }

        public async Task<ApiResponse<VoucherResponse?>> GetVoucherByIdAsync(int voucherId)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
            {
                return new ApiResponse<VoucherResponse?>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Voucher không tồn tại",
                };
            }
            return new ApiResponse<VoucherResponse?>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Lấy voucher có id = {voucher.VoucherId}",
                Data = MapToDTO(voucher)
            };
        }

        public async Task<ApiResponse<VoucherResponse?>> GetVoucherByCodeAsync(string voucherCode)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(voucherCode);
            if (voucher == null)
            {
                return new ApiResponse<VoucherResponse?>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Voucher không tồn tại",
                };
            }
            return new ApiResponse<VoucherResponse?>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Lấy voucher có code = {voucher.VoucherCode}",
                Data = MapToDTO(voucher)
            };
        }

        public async Task<ApiResponse<VoucherResponse>> CreateVoucherAsync(CreateVoucherRequest request)
        {
            // Check if voucher code already exists
            if (await _voucherRepository.VoucherCodeExistsAsync(request.VoucherCode))
            {
                return new ApiResponse<VoucherResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Voucher code '{request.VoucherCode}' đã tồn tại",
                };
            }

            // Validate VoucherTypeId
            var voucherType = await _voucherRepository.GetVoucherTypeByIdAsync(request.VoucherTypeId);
            if (voucherType == null)
            {
                return new ApiResponse<VoucherResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Loại voucher không tồn tại (VoucherTypeId={request.VoucherTypeId}). Vui lòng chọn loại voucher hợp lệ.",
                };
            }

            var voucher = new Voucher
            {
                VoucherTypeId = request.VoucherTypeId,
                CreateBy = request.CreateBy,
                VoucherCode = request.VoucherCode,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinOrderAmount = request.MinOrderAmount,
                UsageLimitPerUser = request.UsageLimitPerUser,
                IsStackable = request.IsStackable,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            var createdVoucher = await _voucherRepository.CreateVoucherAsync(voucher);
            return new ApiResponse<VoucherResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Tạo voucher thành công",
                Data = MapToDTO(createdVoucher)
            };
        }

        public async Task<ApiResponse<VoucherResponse>> UpdateVoucherAsync(int voucherId, UpdateVoucherRequest request)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
            {
                return new ApiResponse<VoucherResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Voucher với id {voucherId} không tồn tại",
                };
            }

            // Check voucher code uniqueness if changed
            if (!string.IsNullOrEmpty(request.VoucherCode) && request.VoucherCode != voucher.VoucherCode)
            {
                if (await _voucherRepository.VoucherCodeExistsAsync(request.VoucherCode, voucherId))
                {
                    return new ApiResponse<VoucherResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Voucher code '{request.VoucherCode}' đã tồn tại",
                    };
                }
                voucher.VoucherCode = request.VoucherCode;
            }

            if (request.DiscountValue.HasValue)
                voucher.DiscountValue = request.DiscountValue.Value;

            if (!string.IsNullOrEmpty(request.DiscountType))
                voucher.DiscountType = request.DiscountType;

            if (request.MaxDiscountAmount.HasValue)
                voucher.MaxDiscountAmount = request.MaxDiscountAmount;

            if (request.MinOrderAmount.HasValue)
                voucher.MinOrderAmount = request.MinOrderAmount;

            if (request.UsageLimitPerUser.HasValue)
                voucher.UsageLimitPerUser = request.UsageLimitPerUser;

            if (request.IsStackable.HasValue)
                voucher.IsStackable = request.IsStackable.Value;

            if (request.StartDate.HasValue)
                voucher.StartDate = request.StartDate.Value;

            if (request.EndDate.HasValue)
                voucher.EndDate = request.EndDate.Value;

            if (!string.IsNullOrEmpty(request.Status))
                voucher.Status = request.Status;

            if (request.IsDeleted.HasValue)
                voucher.IsDeleted = request.IsDeleted.Value;

            voucher.UpdatedAt = DateTime.UtcNow;

            var updatedVoucher = await _voucherRepository.UpdateVoucherAsync(voucher);
            return new ApiResponse<VoucherResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Cập nhật voucher thành công",
                Data = MapToDTO(updatedVoucher)
            };
        }

        private VoucherResponse MapToDTO(Voucher voucher)
        {
            return new VoucherResponse
            {
                VoucherId = voucher.VoucherId,
                VoucherTypeId = voucher.VoucherTypeId,
                VoucherTypeName = voucher.VoucherType?.VoucherTypeName ?? string.Empty,
                CreateBy = voucher.CreateBy,
                CreateByName = voucher.CreateByNavigation?.AccountName,
                VoucherCode = voucher.VoucherCode,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue,
                MaxDiscountAmount = voucher.MaxDiscountAmount,
                MinOrderAmount = voucher.MinOrderAmount,
                UsageLimitPerUser = voucher.UsageLimitPerUser,
                IsStackable = voucher.IsStackable,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                Status = voucher.Status,
                IsDeleted = voucher.IsDeleted,
                CreatedAt = voucher.CreatedAt,
                UpdatedAt = voucher.UpdatedAt
            };
        }

        public async Task<ApiResponse<ValidateVoucherResponse>> ValidateVoucherForCustomerAsync(string code, decimal orderAmount)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);

            if (voucher == null || voucher.IsDeleted || voucher.Status != "Active")
                return new ApiResponse<ValidateVoucherResponse> { Status = 400, StatusMessage = "FAILED", Message = "Mã voucher không hợp lệ hoặc không tồn tại" };

            if (DateTime.Now < voucher.StartDate || DateTime.Now > voucher.EndDate)
                return new ApiResponse<ValidateVoucherResponse> { Status = 400, StatusMessage = "FAILED", Message = "Mã voucher đã hết hạn" };

            if (voucher.MinOrderAmount.HasValue && orderAmount > 0 && orderAmount < voucher.MinOrderAmount)
                return new ApiResponse<ValidateVoucherResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0}₫ để áp dụng voucher này"
                };

            decimal discount;
            if (voucher.DiscountType == "Percentage")
            {
                discount = orderAmount > 0 ? orderAmount * voucher.DiscountValue / 100 : voucher.DiscountValue;
                if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
                    discount = voucher.MaxDiscountAmount.Value;
            }
            else
            {
                discount = voucher.DiscountValue;
            }

            if (orderAmount > 0)
                discount = Math.Min(discount, orderAmount);

            return new ApiResponse<ValidateVoucherResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Voucher hợp lệ",
                Data = new ValidateVoucherResponse
                {
                    VoucherId = voucher.VoucherId,
                    VoucherCode = voucher.VoucherCode,
                    DiscountAmount = discount,
                    DiscountType = voucher.DiscountType,
                    DiscountValue = voucher.DiscountValue,
                    MaxDiscountAmount = voucher.MaxDiscountAmount,
                    MinOrderAmount = voucher.MinOrderAmount
                }
            };
        }

        public async Task<ApiResponse<List<VoucherTypeDTO>>> GetVoucherTypesAsync()
        {
            var types = await _voucherRepository.GetAllVoucherTypesAsync();

            // Auto-seed defaults if the table is empty
            if (types.Count == 0)
            {
                await _voucherRepository.SeedDefaultVoucherTypesAsync();
                types = await _voucherRepository.GetAllVoucherTypesAsync();
            }

            return new ApiResponse<List<VoucherTypeDTO>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Danh sách loại voucher",
                Data = types.Select(t => new VoucherTypeDTO
                {
                    VoucherTypeId = t.VoucherTypeId,
                    VoucherTypeName = t.VoucherTypeName
                }).ToList()
            };
        }
    }
}
