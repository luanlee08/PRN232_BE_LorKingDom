using BLL.DTOs;
using BLL.DTOs.Wallet;

namespace BLL.Interfaces.Wallet;

/// <summary>
/// Query service for wallet read operations
/// </summary>
public interface IWalletQueryService
{
    /// <summary>
    /// Get wallet balance and info for a customer
    /// </summary>
    Task<ApiResponse<WalletResponseDTO>> GetWalletAsync(int accountId);

    /// <summary>
    /// Get paginated transaction history for a customer
    /// </summary>
    Task<ApiResponse<PagedResult<WalletTransactionResponseDTO>>> GetTransactionHistoryAsync(
        int accountId,
        int page = 1,
        int pageSize = 10,
        string? txnType = null,
        string? direction = null);
}
