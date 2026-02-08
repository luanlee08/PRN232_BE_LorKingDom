using BLL.DTOs;
using BLL.DTOs.Address;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class AddressServices : IAddressServices
    {
        private readonly IAddressRepositories _repo;

        public AddressServices(IAddressRepositories repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<AddressResponseDTO>> CreateAsync(AddressRequestDTO request, int accountId)
        {
            // Check if this is the first address for the user
            var addressCount = await _repo.GetUserAddressCountAsync(accountId);
            var isDefault = request.IsDefault;

            // Auto-set first address as default
            if (addressCount == 0)
            {
                isDefault = true;
            }

            // If setting as default, unset other default addresses
            if (isDefault)
            {
                await _repo.UnsetDefaultAddressesAsync(accountId);
            }

            var address = new Address
            {
                AccountId = accountId,
                AddressLine = request.AddressLine,
                City = request.City,
                Ward = request.Ward,
                IsDefault = isDefault,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(address); // nếu không exception => SUCCESS

            var responseDto = new AddressResponseDTO
            {
                AddressId = address.AddressId, // EF đã fill
                AccountId = address.AccountId,
                AddressLine = address.AddressLine,
                City = address.City,
                Ward = address.Ward,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt
            };

            return new ApiResponse<AddressResponseDTO>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Địa chỉ được thêm thành công.",
                Data = responseDto
            };
        }


        public async Task<ApiResponse<AddressResponseDTO>> UpdateAsync(AddressUpdateRequestDTO request, int accountId)
        {
            // Get existing address
            var existingAddress = await _repo.GetByIdAsync(request.AddressId);

            if (existingAddress == null)
            {
                return new ApiResponse<AddressResponseDTO>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy địa chỉ",
                    Data = null
                };
            }

            // Security check: Ensure user owns the address
            if (existingAddress.AccountId != accountId)
            {
                return new ApiResponse<AddressResponseDTO>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền cập nhật địa chỉ này",
                    Data = null
                };
            }

            // If setting as default, unset other defaults
            if (request.IsDefault && !existingAddress.IsDefault)
            {
                await _repo.UnsetDefaultAddressesAsync(accountId);
            }

            // Update properties
            existingAddress.AddressLine = request.AddressLine;
            existingAddress.City = request.City;
            existingAddress.Ward = request.Ward;
            existingAddress.IsDefault = request.IsDefault;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existingAddress);

            var responseDto = new AddressResponseDTO
            {
                AddressId = existingAddress.AddressId,
                AccountId = existingAddress.AccountId,
                AddressLine = existingAddress.AddressLine,
                City = existingAddress.City,
                Ward = existingAddress.Ward,
                IsDefault = existingAddress.IsDefault,
                CreatedAt = existingAddress.CreatedAt,
                UpdatedAt = existingAddress.UpdatedAt
            };

            return new ApiResponse<AddressResponseDTO>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Địa chỉ được cập nhật thành công.",
                Data = responseDto
            };
        }


        public async Task<ApiResponse<List<AddressResponseDTO>>> GetAllAsync(int accountId)
        {
            var addresses = await _repo.GetAllByAccountIdAsync(accountId);

            var addressDtos = addresses.Select(a => new AddressResponseDTO
            {
                AddressId = a.AddressId,
                AccountId = a.AccountId,
                AddressLine = a.AddressLine,
                City = a.City,
                Ward = a.Ward,
                IsDefault = a.IsDefault,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            return new ApiResponse<List<AddressResponseDTO>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = $"Lấy danh sách {addressDtos.Count} địa chỉ thành công.",
                Data = addressDtos
            };
        }

        public async Task<ApiResponse<AddressResponseDTO>> GetByIdAsync(int addressId, int accountId)
        {
            var address = await _repo.GetByIdAsync(addressId);

            if (address == null || address.IsDeleted)
            {
                return new ApiResponse<AddressResponseDTO>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy địa chỉ",
                    Data = null
                };
            }

            // Security check: Ensure user owns the address
            if (address.AccountId != accountId)
            {
                return new ApiResponse<AddressResponseDTO>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền xem địa chỉ này",
                    Data = null
                };
            }

            var addressDto = new AddressResponseDTO
            {
                AddressId = address.AddressId,
                AccountId = address.AccountId,
                AddressLine = address.AddressLine,
                City = address.City,
                Ward = address.Ward,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            return new ApiResponse<AddressResponseDTO>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin địa chỉ thành công.",
                Data = addressDto
            };
        }

        public async Task<ApiResponse<object>> DeleteAsync(int addressId, int accountId)
        {
            var address = await _repo.GetByIdAsync(addressId);

            if (address == null || address.IsDeleted)
            {
                return new ApiResponse<object>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Không tìm thấy địa chỉ",
                    Data = null
                };
            }

            // Security check: Ensure user owns the address
            if (address.AccountId != accountId)
            {
                return new ApiResponse<object>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền xóa địa chỉ này",
                    Data = null
                };
            }

            await _repo.DeleteAsync(addressId, accountId);

            return new ApiResponse<object>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Xóa địa chỉ thành công.",
                Data = null
            };
        }
    }
}
