
using BLL.DTOs;
using BLL.DTOs.Address;

namespace BLL.Interfaces
{
    public interface IAddressServices
    {
        Task<ApiResponse<AddressResponseDTO>> CreateAsync(AddressRequestDTO entity, int accountId);
        Task<ApiResponse<AddressResponseDTO>> UpdateAsync(AddressUpdateRequestDTO entity, int accountId);
        Task<ApiResponse<AddressResponseDTO>> SetDefaultAsync(int addressId, int accountId);
        Task<ApiResponse<object>> DeleteAsync(int addressId, int accountId);
        Task<ApiResponse<List<AddressResponseDTO>>> GetAllAsync(int accountId);
        Task<ApiResponse<AddressResponseDTO>> GetByIdAsync(int addressId, int accountId);
    }
}
