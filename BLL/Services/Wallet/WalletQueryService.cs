using BLL.DTOs;
using BLL.DTOs.Wallet;
using BLL.Interfaces.Wallet;
using DAL.Interface;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Wallet;

public class WalletQueryService : IWalletQueryService
{
    private readonly IWalletRepository _walletRepo;
    private readonly ILogger<WalletQueryService> _logger;

    public WalletQueryService(
        IWalletRepository walletRepo,
        ILogger<WalletQueryService> logger)
    {
        _walletRepo = walletRepo;
        _logger = logger;
    }

    public async Task<ApiResponse<WalletResponseDTO>> GetWalletAsync(int accountId)
    {
        try
        {
            var wallet = await _walletRepo.GetByAccountIdAsync(accountId);

            if (wallet == null)
            {
                return new ApiResponse<WalletResponseDTO>
                {
                    Status = 404,
                    Message = "Ví chưa được tạo. Vui lòng nạp tiền lần đầu để kích hoạt ví.",
                    Data = null
                };
            }

            var dto = new WalletResponseDTO
            {
                WalletId = wallet.WalletId,
                AccountId = wallet.AccountId,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                Status = wallet.Status,
                LastTransactionAt = wallet.LastTransactionAt,
                CreatedAt = wallet.CreatedAt
            };

            return new ApiResponse<WalletResponseDTO>
            {
                Status = 200,
                Message = "Lấy thông tin ví thành công",
                Data = dto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wallet for account {AccountId}", accountId);
            return new ApiResponse<WalletResponseDTO>
            {
                Status = 500,
                Message = "Lỗi khi lấy thông tin ví"
            };
        }
    }

    public async Task<ApiResponse<PagedResult<WalletTransactionResponseDTO>>> GetTransactionHistoryAsync(
        int accountId,
        int page = 1,
        int pageSize = 10,
        string? txnType = null,
        string? direction = null)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var skip = (page - 1) * pageSize;

            var totalCount = await _walletRepo.GetTransactionCountByAccountIdAsync(accountId, txnType, direction);
            var transactions = await _walletRepo.GetTransactionsByAccountIdAsync(accountId, skip, pageSize, txnType, direction);

            var items = transactions.Select(t => new WalletTransactionResponseDTO
            {
                WalletTransactionId = t.WalletTransactionId,
                TxnType = t.TxnType,
                Direction = t.Direction,
                Amount = t.Amount,
                BalanceBefore = t.BalanceBefore,
                BalanceAfter = t.BalanceAfter,
                Method = t.Method,
                ExternalRef = t.ExternalRef,
                Status = t.Status,
                Reason = t.Reason,
                RelatedOrderId = t.RelatedOrderId,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt
            }).ToList();

            var result = new PagedResult<WalletTransactionResponseDTO>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = items
            };

            return new ApiResponse<PagedResult<WalletTransactionResponseDTO>>
            {
                Status = 200,
                Message = "Lấy lịch sử giao dịch thành công",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction history for account {AccountId}", accountId);
            return new ApiResponse<PagedResult<WalletTransactionResponseDTO>>
            {
                Status = 500,
                Message = "Lỗi khi lấy lịch sử giao dịch"
            };
        }
    }
}
