using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AspLorKingDomContext _context;

        public AccountRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && !a.IsDeleted);
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Accounts
                .AnyAsync(a => a.Email == email);
        }

        public async Task<bool> IsEmailExistAsync(string email, int? excludeId)
        {
            var query = _context.Accounts
                .Where(x => x.Email == email);

            if (excludeId.HasValue)
                query = query.Where(x => x.AccountId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task<(List<Account>, int)> GetAsync(
            string? keyword,
            int? roleId,
            string? status,
            int page,
            int pageSize)
        {
            var query = _context.Accounts
                .Include(a => a.Role)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.AccountName.Contains(keyword) ||
                    x.Email.Contains(keyword) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)));
            }

            if (roleId.HasValue)
            {
                query = query.Where(x => x.RoleId == roleId.Value);
            }
            else
            {
                // Khi không chỉ định roleId, chỉ lấy Staff (2) và Warehouse (3) - không lấy Admin (1) và Customer (4)
                query = query.Where(x => x.RoleId == 2 || x.RoleId == 3);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}