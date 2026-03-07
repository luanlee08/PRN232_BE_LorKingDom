using BLL.DTOs;
using BLL.DTOs.Vouchers;

namespace BLL.Interfaces
{
    public interface IVoucherService
    {
        Task<ApiResponse<PagedResult<VoucherResponse>>> GetVouchersAsync(VoucherQuery searchDTO);
        Task<ApiResponse<VoucherResponse?>> GetVoucherByIdAsync(int voucherId);
        Task<ApiResponse<VoucherResponse?>> GetVoucherByCodeAsync(string voucherCode);
        Task<ApiResponse<VoucherResponse>> CreateVoucherAsync(CreateVoucherRequest createDTO);
        Task<ApiResponse<VoucherResponse>> UpdateVoucherAsync(int voucherId, UpdateVoucherRequest updateDTO);
        Task<ApiResponse<List<VoucherTypeDTO>>> GetVoucherTypesAsync();
        Task<ApiResponse<ValidateVoucherResponse>> ValidateVoucherForCustomerAsync(string code, decimal orderAmount);
    }
}
