using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class AddressRepositories : IAddressRepositories
    {
        private readonly AspLorKingDomContext _context;

        public AddressRepositories(AspLorKingDomContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Address entity)
        {

            await _context.Addresses.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address entity)
        {
            _context.Addresses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Address>> GetAllByAccountIdAsync(int accountId)
        {
            return await _context.Addresses
                .Where(a => a.AccountId == accountId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            throw new NotImplementedException();
        }


        public async Task<Address?> GetByIdAsync(int AddressID)
        {
            return await _context.Addresses
              .FirstOrDefaultAsync(v => v.AddressId == AddressID);
        }

        public async Task UnsetDefaultAddressesAsync(int accountId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.AccountId == accountId && a.IsDefault)
                .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserAddressCountAsync(int accountId)
        {
            return await _context.Addresses
                .CountAsync(a => a.AccountId == accountId && !a.IsDeleted);
        }

        public async Task DeleteAsync(int addressId, int accountId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.AccountId == accountId);

            if (address != null)
            {
                address.IsDeleted = true;
                address.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
