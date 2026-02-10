using DAL.Models;

namespace DAL.Interface
{
    public interface IAddressRepositories
    {
        Task AddAsync(Address entity);
        Task UpdateAsync(Address entity);
        Task<List<Address>> GetAllByAccountIdAsync(int accountId);
        Task<Address?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task UnsetDefaultAddressesAsync(int accountId);
        Task<int> GetUserAddressCountAsync(int accountId);
        Task DeleteAsync(int addressId, int accountId);
    }
}
