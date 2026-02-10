using BLL.DTOs;
using BLL.DTOs.Auth;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponse>> VerifyOtpAsync(VerifyOtpRequest request);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<bool>> LogoutAsync(string accessToken);
    }
}