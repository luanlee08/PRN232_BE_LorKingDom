using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IAccountRepository
    {
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetByIdAsync(int accountId);
        Task<bool> IsEmailExistAsync(string email);
        Task AddAsync(Account account);
        Task SaveChangesAsync();
        Task<(List<Account> Items, int TotalCount)> GetAsync(
            string? keyword,
            int? roleId,
            string? status,
            int page,
            int pageSize);
        Task<bool> IsEmailExistAsync(string email, int? excludeId);
    }
}