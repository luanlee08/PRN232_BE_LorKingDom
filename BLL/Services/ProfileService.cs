using BLL.DTOs;
using BLL.DTOs.Profile;
using BLL.Interfaces;
using DAL.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileService(IAccountRepository accountRepo, IWebHostEnvironment webHostEnvironment)
        {
            _accountRepo = accountRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ApiResponse<ProfileResponse>> GetProfileAsync(int accountId)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);

            if (account == null || account.IsDeleted)
            {
                return new ApiResponse<ProfileResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản",
                    Data = null
                };
            }

            return new ApiResponse<ProfileResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin profile thành công",
                Data = new ProfileResponse
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    Image = account.Image,
                    RoleId = account.RoleId,
                    RoleName = account.Role?.RoleName,
                    Provider = account.Provider,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                }
            };
        }

        public async Task<ApiResponse<bool>> UpdateProfileAsync(int accountId, UpdateProfileRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);

            if (account == null || account.IsDeleted)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản",
                    Data = false
                };
            }

            account.AccountName = request.AccountName;
            account.PhoneNumber = request.PhoneNumber;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật profile thành công",
                Data = true
            };
        }

        public async Task<ApiResponse<string>> UpdateProfileAvatarAsync(int accountId, IFormFile file)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);

            if (account == null || account.IsDeleted)
            {
                return new ApiResponse<string>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản",
                    Data = null
                };
            }

            if (file == null || file.Length == 0)
            {
                return new ApiResponse<string>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "File không hợp lệ",
                    Data = null
                };
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ApiResponse<string>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp)",
                    Data = null
                };
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return new ApiResponse<string>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Kích thước file không được vượt quá 5MB",
                    Data = null
                };
            }

            // Xóa avatar cũ nếu có
            if (!string.IsNullOrWhiteSpace(account.Image))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, account.Image.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    try
                    {
                        File.Delete(oldFilePath);
                    }
                    catch
                    {
                        // Ignore delete errors
                    }
                }
            }

            // Tạo thư mục uploads/avatars nếu chưa có
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file unique
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = $"Upload ảnh thất bại: {ex.Message}",
                    Data = null
                };
            }

            // URL để trả về (relative path)
            var imageUrl = $"/uploads/avatars/{uniqueFileName}";

            account.Image = imageUrl;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepo.SaveChangesAsync();

            return new ApiResponse<string>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật avatar thành công",
                Data = imageUrl
            };
        }
    }
}
