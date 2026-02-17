using BLL.DTOs;
using BLL.DTOs.Profile;
using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces
{
    public interface IProfileService
    {
        Task<ApiResponse<ProfileResponse>> GetProfileAsync(int accountId);
        Task<ApiResponse<bool>> UpdateProfileAsync(int accountId, UpdateProfileRequest request);
        Task<ApiResponse<string>> UpdateProfileAvatarAsync(int accountId, IFormFile file);
    }
}
