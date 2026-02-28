using DAL.Models;

namespace DAL.Interface
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByAccountIdAsync(int accountId);
        Task<Wallet?> GetByAccountIdWithLockAsync(int accountId);
        Task<Wallet?> GetByIdAsync(int walletId);
        Task<Wallet> CreateWalletAsync(Wallet wallet);
        Task UpdateWalletAsync(Wallet wallet);
        Task<WalletTransaction> AddWalletTransactionAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetWalletTransactionByIdAsync(long transactionId);
        Task<WalletTransaction?> GetTransactionByIdempotencyKeyAsync(string idempotencyKey);
        Task UpdateWalletTransactionAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetTransactionsByAccountIdAsync(
            int accountId, int skip, int take, string? txnType = null, string? direction = null);
        Task<int> GetTransactionCountByAccountIdAsync(
            int accountId, string? txnType = null, string? direction = null);
    }
}
