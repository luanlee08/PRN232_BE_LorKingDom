using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AspLorKingDomContext _context;

        public WalletRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByAccountIdAsync(int accountId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.AccountId == accountId);
        }

        public async Task<Wallet?> GetByAccountIdWithLockAsync(int accountId)
        {
            // Use row-level lock to prevent race conditions
            return await _context.Wallets
                .FromSqlRaw("SELECT * FROM Wallets WITH (UPDLOCK, ROWLOCK) WHERE AccountId = {0}", accountId)
                .FirstOrDefaultAsync();
        }

        public async Task<Wallet?> GetByIdAsync(int walletId)
        {
            return await _context.Wallets.FindAsync(walletId);
        }

        public async Task<Wallet> CreateWalletAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task UpdateWalletAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<WalletTransaction> AddWalletTransactionAsync(WalletTransaction transaction)
        {
            await _context.WalletTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<WalletTransaction?> GetWalletTransactionByIdAsync(long transactionId)
        {
            return await _context.WalletTransactions
                .Include(wt => wt.Wallet)
                .Include(wt => wt.Account)
                .FirstOrDefaultAsync(wt => wt.WalletTransactionId == transactionId);
        }

        public async Task UpdateWalletTransactionAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<WalletTransaction?> GetTransactionByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _context.WalletTransactions
                .FirstOrDefaultAsync(wt => wt.IdempotencyKey == idempotencyKey);
        }

        public async Task<IEnumerable<WalletTransaction>> GetTransactionsByAccountIdAsync(
            int accountId, int skip, int take, string? txnType = null, string? direction = null)
        {
            var query = _context.WalletTransactions
                .Where(wt => wt.AccountId == accountId);

            if (!string.IsNullOrEmpty(txnType))
                query = query.Where(wt => wt.TxnType == txnType);

            if (!string.IsNullOrEmpty(direction))
                query = query.Where(wt => wt.Direction == direction);

            return await query
                .OrderByDescending(wt => wt.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTransactionCountByAccountIdAsync(
            int accountId, string? txnType = null, string? direction = null)
        {
            var query = _context.WalletTransactions
                .Where(wt => wt.AccountId == accountId);

            if (!string.IsNullOrEmpty(txnType))
                query = query.Where(wt => wt.TxnType == txnType);

            if (!string.IsNullOrEmpty(direction))
                query = query.Where(wt => wt.Direction == direction);

            return await query.CountAsync();
        }
    }
}
