using BLL.DTOs;
using BLL.DTOs.Wallet;

namespace BLL.Interfaces.Wallet;

/// <summary>
/// Command service for wallet write operations (top-up, callbacks)
/// </summary>
public interface IWalletCommandService
{
    /// <summary>
    /// Initiate a wallet top-up via payment gateway
    /// </summary>
    Task<ApiResponse<TopUpResponseDTO>> InitiateTopUpAsync(int accountId, TopUpRequestDTO request, string ipAddress);

    /// <summary>
    /// Handle VNPay callback after top-up payment
    /// </summary>
    Task<ApiResponse<WalletTransactionResponseDTO>> HandleVNPayCallbackAsync(Dictionary<string, string> queryParams);

    /// <summary>
    /// Handle MoMo IPN callback after top-up payment
    /// </summary>
    Task<ApiResponse<WalletTransactionResponseDTO>> HandleMoMoCallbackAsync(Dictionary<string, string> queryParams);

    /// <summary>
    /// Handle Sepay webhook after top-up payment
    /// </summary>
    Task<ApiResponse<WalletTransactionResponseDTO>> HandleSepayCallbackAsync(Dictionary<string, string> queryParams);
}
