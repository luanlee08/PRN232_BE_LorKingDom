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
    }
}