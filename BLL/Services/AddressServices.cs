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
                RecipientName = request.RecipientName,
                PhoneNumber = request.PhoneNumber,
                AddressLine = request.AddressLine,

                // Text names (for display)
                City = request.City,
                District = request.District,
                Ward = request.Ward,

                // GHN IDs (for shipping)
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardCode = request.WardCode,

                IsDefault = isDefault,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(address); // nếu không exception => SUCCESS

            var responseDto = new AddressResponseDTO
            {
                AddressId = address.AddressId, // EF đã fill
                AccountId = address.AccountId,
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,

                // Text names
                City = address.City,
                District = address.District,
                Ward = address.Ward,

                // GHN IDs
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                WardCode = address.WardCode,

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
            existingAddress.RecipientName = request.RecipientName;
            existingAddress.PhoneNumber = request.PhoneNumber;
            existingAddress.AddressLine = request.AddressLine;

            // Text names
            existingAddress.City = request.City;
            existingAddress.District = request.District;
            existingAddress.Ward = request.Ward;

            // GHN IDs
            existingAddress.ProvinceId = request.ProvinceId;
            existingAddress.DistrictId = request.DistrictId;
            existingAddress.WardCode = request.WardCode;

            existingAddress.IsDefault = request.IsDefault;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existingAddress);

            var responseDto = new AddressResponseDTO
            {
                AddressId = existingAddress.AddressId,
                AccountId = existingAddress.AccountId,
                RecipientName = existingAddress.RecipientName,
                PhoneNumber = existingAddress.PhoneNumber,
                AddressLine = existingAddress.AddressLine,

                // Text names
                City = existingAddress.City,
                District = existingAddress.District,
                Ward = existingAddress.Ward,

                // GHN IDs
                ProvinceId = existingAddress.ProvinceId,
                DistrictId = existingAddress.DistrictId,
                WardCode = existingAddress.WardCode,

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
                RecipientName = a.RecipientName,
                PhoneNumber = a.PhoneNumber,
                AddressLine = a.AddressLine,

                // Text names
                City = a.City,
                District = a.District,
                Ward = a.Ward,

                // GHN IDs
                ProvinceId = a.ProvinceId,
                DistrictId = a.DistrictId,
                WardCode = a.WardCode,

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
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,

                // Text names
                City = address.City,
                District = address.District,
                Ward = address.Ward,

                // GHN IDs
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                WardCode = address.WardCode,

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

        public async Task<ApiResponse<AddressResponseDTO>> SetDefaultAsync(int addressId, int accountId)
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

            if (address.AccountId != accountId)
            {
                return new ApiResponse<AddressResponseDTO>
                {
                    Status = 403,
                    StatusMessage = "FORBIDDEN",
                    Message = "Bạn không có quyền thay đổi địa chỉ này",
                    Data = null
                };
            }

            // Unset all current defaults, then set this one
            await _repo.UnsetDefaultAddressesAsync(accountId);
            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(address);

            var responseDto = new AddressResponseDTO
            {
                AddressId = address.AddressId,
                AccountId = address.AccountId,
                RecipientName = address.RecipientName,
                PhoneNumber = address.PhoneNumber,
                AddressLine = address.AddressLine,
                City = address.City,
                District = address.District,
                Ward = address.Ward,
                ProvinceId = address.ProvinceId,
                DistrictId = address.DistrictId,
                WardCode = address.WardCode,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            return new ApiResponse<AddressResponseDTO>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đã đặt làm địa chỉ mặc định.",
                Data = responseDto
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
