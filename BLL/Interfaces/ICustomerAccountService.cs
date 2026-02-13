using BLL.DTOs;
using BLL.DTOs.Accounts;

namespace BLL.Interfaces
{
    public interface ICustomerAccountService
    {
        Task<ApiResponse<PagedResult<AccountResponse>>> GetCustomersAsync(CustomerAccountQuery query);
        Task<ApiResponse<AccountResponse>> GetCustomerByIdAsync(int id);
        Task<ApiResponse<bool>> UpdateCustomerAsync(int id, UpdateCustomerAccountRequest request);
    }
}
