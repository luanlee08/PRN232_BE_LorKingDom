using BLL.DTOs;
using BLL.DTOs.Accounts;

namespace BLL.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<PagedResult<AccountResponse>>> GetAsync(AccountQuery query);
        Task<ApiResponse<AccountResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreateAccountRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateAccountRequest request);
    }
}
