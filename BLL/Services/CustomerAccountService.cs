using BLL.DTOs;
using BLL.DTOs.Accounts;
using BLL.Interfaces;
using DAL.Interface;

namespace BLL.Services
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly IAccountRepository _repo;
        private const int CUSTOMER_ROLE_ID = 1;

        public CustomerAccountService(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<AccountResponse>>> GetCustomersAsync(CustomerAccountQuery query)
        {
            var (items, total) = await _repo.GetAsync(
                query.Keyword,
                CUSTOMER_ROLE_ID,
                query.Status,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<AccountResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách tài khoản khách hàng",
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

        public async Task<ApiResponse<AccountResponse>> GetCustomerByIdAsync(int id)
        {
            var account = await _repo.GetByIdAsync(id);

            if (account == null || account.RoleId != CUSTOMER_ROLE_ID)
            {
                return new ApiResponse<AccountResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản khách hàng",
                    Data = null
                };
            }

            return new ApiResponse<AccountResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin tài khoản khách hàng thành công",
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

        public async Task<ApiResponse<bool>> UpdateCustomerAsync(int id, UpdateCustomerAccountRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null || entity.RoleId != CUSTOMER_ROLE_ID)
            {
                return new ApiResponse<bool>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy tài khoản khách hàng",
                    Data = false
                };
            }

            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Cập nhật tài khoản khách hàng thành công",
                Data = true
            };
        }
    }
}
