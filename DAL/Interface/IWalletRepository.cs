using DAL.Models;

namespace DAL.Interface
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByAccountIdAsync(int accountId);
        Task<Wallet?> GetByAccountIdWithLockAsync(int accountId);
        Task<Wallet?> GetByIdAsync(int walletId);
        Task UpdateWalletAsync(Wallet wallet);
        Task<WalletTransaction> AddWalletTransactionAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetWalletTransactionByIdAsync(long transactionId);
        Task UpdateWalletTransactionAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetTransactionsByAccountIdAsync(int accountId, int skip, int take);
    }
}
