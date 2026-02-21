using BLL.DTOs;
using BLL.DTOs.Accounts;
using BLL.Interfaces;
using DAL.Interface;
using DAL.Models;

namespace BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<AccountResponse>>> GetAsync(AccountQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword,
                query.RoleId,
                query.Status,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<AccountResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách tài khoản",
                Data = new PagedResult<AccountResponse>
                {
                    Items = items.Select(x => new AccountResponse
                    {
                        AccountId = x.AccountId,
                        AccountName = x.AccountName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Image = x.Image,
                        Status = x.Status,
                        RoleId = x.RoleId,
                        RoleName = x.Role?.RoleName,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    }).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }

        public async Task<ApiResponse<AccountResponse>> GetByIdAsync(int id)
        {
            var account = await _repo.GetByIdAsync(id);

            if (account == null)
            {
                return new ApiResponse<AccountResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản",
                    Data = null
                };
            }

            return new ApiResponse<AccountResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin tài khoản thành công",
                Data = new AccountResponse
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    Image = account.Image,
                    Status = account.Status,
                    RoleId = account.RoleId,
                    RoleName = account.Role?.RoleName,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                }
            };
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateAccountRequest request)
        {
            if (await _repo.IsEmailExistAsync(request.Email))
            {
                return new ApiResponse<int>
                {
                    Status = 400,
                    StatusMessage = "FAILED",
                    Message = "Email đã tồn tại"
                };
            }

            var entity = new Account
            {
                AccountName = request.AccountName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                Status = request.Status,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                Provider = "System"
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Status = 201,
                StatusMessage = "SUCCESS",
                Message = "Tạo tài khoản thành công",
                Data = entity.AccountId
            };
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateAccountRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản",
                    Data = false
                };
            }

            entity.AccountName = request.AccountName;
            entity.PhoneNumber = request.PhoneNumber;
            entity.RoleId = request.RoleId;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                entity.Status = request.Status;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                entity.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật tài khoản thành công",
                Data = true
            };
        }

    }
}
