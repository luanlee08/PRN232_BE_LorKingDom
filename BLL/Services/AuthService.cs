using BLL.DTOs;
using BLL.DTOs.Auth;
using BLL.DTOs.Notifications;
using BLL.Interfaces;
using BLL.Interfaces.Notification;
using DAL.Infrastructure.Email;
using DAL.Infrastructure.Redis;
using DAL.Interface;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IRedisService _redis;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INotificationCommandService? _notificationService;

        public AuthService(
            IAccountRepository accountRepo,
            IRedisService redis,
            IEmailService emailService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            INotificationCommandService? notificationService = null)
        {
            _accountRepo = accountRepo;
            _redis = redis;
            _emailService = emailService;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
        {
            // Kiểm tra email đã tồn tại
            if (await _accountRepo.IsEmailExistAsync(request.Email))
            {
                return new ApiResponse<string>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Email đã được đăng ký",
                    Data = null
                };
            }

            // Tạo mã OTP
            var otpCode = GenerateOtpCode();

            // Lưu thông tin đăng ký tạm thời vào Redis (5 phút)
            var registerKey = $"register:{request.Email}";
            var registerData = System.Text.Json.JsonSerializer.Serialize(new
            {
                request.AccountName,
                request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                request.PhoneNumber,
                request.Provider,
                OtpCode = otpCode
            });

            await _redis.SetAsync(registerKey, registerData, TimeSpan.FromMinutes(5));

            // Gửi email OTP
            var emailSent = await _emailService.SendOtpEmailAsync(
                request.Email,
                request.AccountName,
                otpCode);

            if (!emailSent)
            {
                return new ApiResponse<string>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Không thể gửi email xác thực",
                    Data = null
                };
            }

            return new ApiResponse<string>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Vui lòng kiểm tra email để xác thực tài khoản",
                Data = request.Email
            };
        }

        public async Task<ApiResponse<LoginResponse>> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var registerKey = $"register:{request.Email}";
            var registerDataJson = await _redis.GetAsync(registerKey);

            if (string.IsNullOrEmpty(registerDataJson))
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Mã OTP đã hết hạn hoặc không tồn tại",
                    Data = null
                };
            }

            var registerData = System.Text.Json.JsonSerializer.Deserialize<RegisterTempData>(registerDataJson);

            if (registerData?.OtpCode != request.OtpCode)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Mã OTP không chính xác",
                    Data = null
                };
            }

            // Tạo tài khoản
            var account = new Account
            {
                AccountName = registerData.AccountName,
                Email = registerData.Email,
                Password = registerData.Password,
                PhoneNumber = registerData.PhoneNumber,
                Provider = registerData.Provider ?? "Email",
                RoleId = 1, // Customer role
                Status = "Active",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();

            // Xóa dữ liệu tạm trong Redis
            await _redis.DeleteAsync(registerKey);

            // Gửi email chào mừng
            await _emailService.SendWelcomeEmailAsync(account.Email, account.AccountName);

            // Gửi thông báo in-app chào mừng
            if (_notificationService != null)
            {
                try
                {
                    await _notificationService.SendNotificationAsync(
                        new SendNotificationRequest
                        {
                            TemplateCode = "WELCOME",
                            TargetType = "User",
                            TargetUserIds = new List<int> { account.AccountId },
                            Parameters = new Dictionary<string, string>
                            {
                                ["userName"] = account.AccountName
                            }
                        },
                        createdByAccountId: 0,
                        isSystemGenerated: true);
                }
                catch
                {
                    // Notification failure must not block registration
                }
            }

            // Tạo JWT tokens để auto login
            var (accessToken, refreshToken, expiresAt) = await GenerateTokensAsync(account);

            return new ApiResponse<LoginResponse>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Đăng ký tài khoản thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserInfo
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        Email = account.Email,
                        PhoneNumber = account.PhoneNumber,
                        Image = account.Image,
                        RoleName = account.Role?.RoleName ?? "Customer"
                    }
                }
            };
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var account = await _accountRepo.GetByEmailAsync(request.Email);

            if (account == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 401,
                    StatusMessage = "FAILED",
                    Message = "Email hoặc mật khẩu không chính xác",
                    Data = null
                };
            }

            if (account.Status != "Active")
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 403,
                    StatusMessage = "FAILED",
                    Message = "Tài khoản đã bị khóa hoặc chưa kích hoạt",
                    Data = null
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 401,
                    StatusMessage = "FAILED",
                    Message = "Email hoặc mật khẩu không chính xác",
                    Data = null
                };
            }

            // Tạo JWT tokens
            var (accessToken, refreshToken, expiresAt) = await GenerateTokensAsync(account);

            return new ApiResponse<LoginResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đăng nhập thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserInfo
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        Email = account.Email,
                        PhoneNumber = account.PhoneNumber,
                        Image = account.Image,
                        RoleName = account.Role?.RoleName ?? "Customer"
                    }
                }
            };
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            // Lấy thông tin từ Redis
            var refreshKey = $"refresh:{refreshToken}";
            var accountIdStr = await _redis.GetAsync(refreshKey);

            if (string.IsNullOrEmpty(accountIdStr))
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 401,
                    StatusMessage = "FAILED",
                    Message = "Refresh token không hợp lệ hoặc đã hết hạn",
                    Data = null
                };
            }

            var account = await _accountRepo.GetByIdAsync(int.Parse(accountIdStr));

            if (account == null || account.Status != "Active")
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 403,
                    StatusMessage = "FAILED",
                    Message = "Tài khoản không tồn tại hoặc đã bị khóa",
                    Data = null
                };
            }

            // Xóa refresh token cũ
            await _redis.DeleteAsync(refreshKey);

            // Tạo tokens mới
            var (accessToken, newRefreshToken, expiresAt) = await GenerateTokensAsync(account);

            return new ApiResponse<LoginResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Làm mới token thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserInfo
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        Email = account.Email,
                        PhoneNumber = account.PhoneNumber,
                        Image = account.Image,
                        RoleName = account.Role?.RoleName ?? "Customer"
                    }
                }
            };
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string accessToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);

                var accountId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var jti = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var exp = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(jti))
                {
                    return new ApiResponse<bool>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Token không hợp lệ",
                        Data = false
                    };
                }

                // Tính thời gian còn lại của token
                TimeSpan expiryTime;
                if (!string.IsNullOrEmpty(exp))
                {
                    var expiryDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
                    expiryTime = expiryDate - DateTimeOffset.UtcNow;
                }
                else
                {
                    // Fallback: sử dụng ValidTo từ token
                    expiryTime = token.ValidTo - DateTime.UtcNow;
                }

                // Chỉ thêm vào blacklist nếu token chưa hết hạn
                if (expiryTime.TotalSeconds > 0)
                {
                    var blacklistKey = $"blacklist:{jti}";
                    await _redis.SetAsync(blacklistKey, "revoked", expiryTime);
                }

                // Xóa refresh token của user này
                var userRefreshKey = $"user:{accountId}:refresh";
                var refreshToken = await _redis.GetAsync(userRefreshKey);

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Xóa refresh token
                    await _redis.DeleteAsync($"refresh:{refreshToken}");
                    await _redis.DeleteAsync(userRefreshKey);
                }

                return new ApiResponse<bool>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Đăng xuất thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = $"Không thể đăng xuất: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(int accountId, ChangePasswordRequest request)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);

            if (account == null)
            {
                return new ApiResponse<bool> { Status = 404, StatusMessage = "FAILED", Message = "Tài khoản không tồn tại", Data = false };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.Password))
            {
                return new ApiResponse<bool> { Status = 400, StatusMessage = "FAILED", Message = "Mật khẩu hiện tại không đúng", Data = false };
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            account.Provider = "Email";
            await _accountRepo.SaveChangesAsync();

            return new ApiResponse<bool> { Status = 200, StatusMessage = "SUCCESS", Message = "Đổi mật khẩu thành công", Data = true };
        }

        public async Task<ApiResponse<LoginResponse>> GoogleLoginAsync(GoogleAuthRequest request)
        {
            var googleUser = await VerifyGoogleTokenAsync(request.AccessToken);

            if (googleUser == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Token Google không hợp lệ",
                    Data = null
                };
            }

            var account = await _accountRepo.GetByEmailAsync(googleUser.Email);

            if (account == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 404,
                    StatusMessage = "FAILED",
                    Message = "Tài khoản chưa có trong hệ thống. Vui lòng đăng ký trước.",
                    Data = null
                };
            }

            if (account.IsDeleted || account.Status != "Active")
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 403,
                    StatusMessage = "FAILED",
                    Message = "Tài khoản đã bị khóa hoặc chưa kích hoạt",
                    Data = null
                };
            }

            var (accessToken, refreshToken, expiresAt) = await GenerateTokensAsync(account);

            return new ApiResponse<LoginResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Đăng nhập thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserInfo
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        Email = account.Email,
                        PhoneNumber = account.PhoneNumber,
                        Image = account.Image,
                        RoleName = account.Role?.RoleName ?? "Customer"
                    }
                }
            };
        }

        public async Task<ApiResponse<LoginResponse>> GoogleRegisterAsync(GoogleAuthRequest request)
        {
            var googleUser = await VerifyGoogleTokenAsync(request.AccessToken);

            if (googleUser == null)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Token Google không hợp lệ",
                    Data = null
                };
            }

            var existingAccount = await _accountRepo.GetByEmailAsync(googleUser.Email);

            if (existingAccount != null)
            {
                if (existingAccount.Provider == "Google")
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Email này đã được đăng ký bằng Google. Vui lòng đăng nhập.",
                        Data = null
                    };
                }

                return new ApiResponse<LoginResponse>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Email này đã được đăng ký bằng phương thức khác. Vui lòng đăng nhập bằng Email.",
                    Data = null
                };
            }

            var defaultPassword = "Customer123@";
            var account = new Account
            {
                AccountName = googleUser.Name ?? googleUser.Email,
                Email = googleUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                PhoneNumber = null,
                Provider = "Google",
                RoleId = 1,
                Status = "Active",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync();

            await _emailService.SendWelcomeEmailAsync(account.Email, account.AccountName);

            var (accessToken, refreshToken, expiresAt) = await GenerateTokensAsync(account);

            return new ApiResponse<LoginResponse>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Đăng ký tài khoản thành công",
                Data = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    User = new UserInfo
                    {
                        AccountId = account.AccountId,
                        AccountName = account.AccountName,
                        Email = account.Email,
                        PhoneNumber = account.PhoneNumber,
                        Image = account.Image,
                        RoleName = account.Role?.RoleName ?? "Customer"
                    }
                }
            };
        }

        // Private helpers
        private string GenerateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        private async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GenerateTokensAsync(Account account)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddHours(1);
            var jti = Guid.NewGuid().ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Name, account.AccountName),
                new Claim(ClaimTypes.Role, account.Role?.RoleName ?? "Customer"),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Tạo refresh token
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // Lưu refresh token vào Redis (7 ngày)
            var refreshKey = $"refresh:{refreshToken}";
            await _redis.SetAsync(refreshKey, account.AccountId.ToString(), TimeSpan.FromDays(7));

            // Lưu mapping user -> refresh token để có thể revoke khi logout
            await _redis.SetAsync($"user:{account.AccountId}:refresh", refreshToken, TimeSpan.FromDays(7));

            return (accessToken, refreshToken, expiresAt);
        }

        private class RegisterTempData
        {
            public string AccountName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? PhoneNumber { get; set; }
            public string? Provider { get; set; }
            public string OtpCode { get; set; } = string.Empty;
        }

        private async Task<GoogleUserInfo?> VerifyGoogleTokenAsync(string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var payload = JsonSerializer.Deserialize<GoogleTokenPayload>(json);

                if (payload == null || string.IsNullOrEmpty(payload.Email))
                    return null;

                return new GoogleUserInfo
                {
                    Email = payload.Email,
                    Name = payload.Name ?? payload.Email
                };
            }
            catch
            {
                return null;
            }
        }

        private class GoogleUserInfo
        {
            public string Email { get; set; } = string.Empty;
            public string? Name { get; set; }
        }

        private class GoogleTokenPayload
        {
            [JsonPropertyName("email")]
            public string? Email { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("email_verified")]
            public bool? EmailVerified { get; set; }
        }
    }
}