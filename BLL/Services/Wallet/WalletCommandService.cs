using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.DTOs.PaymentGateway;
using BLL.DTOs.Wallet;
using BLL.Interfaces;
using BLL.Interfaces.Wallet;
using DAL.Interface;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Wallet;

public class WalletCommandService : IWalletCommandService
{
    private readonly IWalletRepository _walletRepo;
    private readonly AspLorKingDomContext _context;
    private readonly IVNPayService _vnPayService;
    private readonly IMoMoService _moMoService;
    private readonly ISepayService _sepayService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WalletCommandService> _logger;

    public WalletCommandService(
        IWalletRepository walletRepo,
        AspLorKingDomContext context,
        IVNPayService vnPayService,
        IMoMoService moMoService,
        ISepayService sepayService,
        IConfiguration configuration,
        ILogger<WalletCommandService> logger)
    {
        _walletRepo = walletRepo;
        _context = context;
        _vnPayService = vnPayService;
        _moMoService = moMoService;
        _sepayService = sepayService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<TopUpResponseDTO>> InitiateTopUpAsync(
        int accountId, TopUpRequestDTO request, string ipAddress)
    {
        try
        {
            // Ensure wallet exists (create if absent)
            var wallet = await _walletRepo.GetByAccountIdAsync(accountId);
            if (wallet == null)
            {
                wallet = await _walletRepo.CreateWalletAsync(new DAL.Models.Wallet
                {
                    AccountId = accountId,
                    Currency = "VND",
                    Balance = 0,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (wallet.Status != "Active")
            {
                return new ApiResponse<TopUpResponseDTO>
                {
                    Status = 400,
                    Message = "Ví đã bị khóa, không thể nạp tiền"
                };
            }

            // Generate idempotency key
            var idempotencyKey = $"TOPUP_{accountId}_{Guid.NewGuid():N}";

            // Create pending wallet transaction
            var walletTxn = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                AccountId = accountId,
                TxnType = WalletTransactionTypes.TopUp,
                Direction = WalletDirection.In,
                Amount = request.Amount,
                BalanceBefore = wallet.Balance,
                BalanceAfter = wallet.Balance, // Will be updated on completion
                Method = request.Gateway,
                IdempotencyKey = idempotencyKey,
                Status = "Pending",
                Reason = $"Nạp tiền vào ví qua {request.Gateway}",
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepo.AddWalletTransactionAsync(walletTxn);

            // Generate payment URL based on gateway
            string paymentUrl;
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7219";

            switch (request.Gateway)
            {
                case PaymentMethods.VNPay:
                    var vnpayRequest = new VNPayRequest
                    {
                        OrderId = idempotencyKey,
                        Amount = request.Amount,
                        OrderInfo = $"Nạp ví LorKingDom - {request.Amount:N0} VND",
                        ReturnUrl = $"{baseUrl}/api/CWallet/topup/callback/vnpay",
                        IpAddress = ipAddress
                    };
                    var vnpayRes = await _vnPayService.CreatePaymentUrlAsync(vnpayRequest);
                    paymentUrl = vnpayRes.PaymentUrl;
                    break;

                case PaymentMethods.MoMo:
                    var momoRequest = new MoMoRequest
                    {
                        OrderId = idempotencyKey,
                        Amount = request.Amount,
                        OrderInfo = $"Nạp ví LorKingDom - {request.Amount:N0} VND",
                        ReturnUrl = request.ReturnUrl,
                        NotifyUrl = $"{baseUrl}/api/CWallet/topup/callback/momo"
                    };
                    var momoRes = await _moMoService.CreatePaymentAsync(momoRequest);
                    paymentUrl = momoRes.PayUrl;
                    break;

                case PaymentMethods.Sepay:
                    var sepayRequest = new SepayRequest
                    {
                        OrderId = idempotencyKey,
                        Amount = request.Amount,
                        OrderInfo = $"Nạp ví LorKingDom - {request.Amount:N0} VND",
                        ReturnUrl = request.ReturnUrl,
                        CancelUrl = request.ReturnUrl,
                        NotifyUrl = $"{baseUrl}/api/CWallet/topup/webhook/sepay"
                    };
                    var sepayRes = await _sepayService.CreatePaymentAsync(sepayRequest);
                    paymentUrl = sepayRes.PaymentUrl;
                    break;

                default:
                    return new ApiResponse<TopUpResponseDTO>
                    {
                        Status = 400,
                        Message = $"Gateway không được hỗ trợ: {request.Gateway}"
                    };
            }

            return new ApiResponse<TopUpResponseDTO>
            {
                Status = 200,
                Message = "Tạo yêu cầu nạp tiền thành công",
                Data = new TopUpResponseDTO
                {
                    PaymentUrl = paymentUrl,
                    IdempotencyKey = idempotencyKey,
                    TransactionId = walletTxn.WalletTransactionId,
                    Gateway = request.Gateway
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating top-up for account {AccountId}", accountId);
            return new ApiResponse<TopUpResponseDTO>
            {
                Status = 500,
                Message = "Lỗi khi tạo yêu cầu nạp tiền: " + ex.Message
            };
        }
    }

    public async Task<ApiResponse<WalletTransactionResponseDTO>> HandleVNPayCallbackAsync(
        Dictionary<string, string> queryParams)
    {
        try
        {
            var callback = _vnPayService.ParseCallback(queryParams);

            // Validate signature
            if (!_vnPayService.ValidateCallback(callback))
            {
                _logger.LogWarning("Invalid VNPay callback signature for txnRef {TxnRef}", callback.vnp_TxnRef);
                return new ApiResponse<WalletTransactionResponseDTO>
                {
                    Status = 400,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            var idempotencyKey = callback.vnp_TxnRef;
            var isSuccess = callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00";

            return await CompleteTopUpAsync(idempotencyKey, isSuccess, callback.vnp_TransactionNo, "VNPay");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling VNPay callback");
            return new ApiResponse<WalletTransactionResponseDTO>
            {
                Status = 500,
                Message = "Lỗi xử lý callback VNPay"
            };
        }
    }

    public async Task<ApiResponse<WalletTransactionResponseDTO>> HandleMoMoCallbackAsync(
        Dictionary<string, string> queryParams)
    {
        try
        {
            var orderId = queryParams.GetValueOrDefault("orderId", "");
            var resultCode = queryParams.GetValueOrDefault("resultCode", "-1");
            var transId = queryParams.GetValueOrDefault("transId", "");

            var isSuccess = resultCode == "0";

            return await CompleteTopUpAsync(orderId, isSuccess, transId, "MoMo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MoMo callback");
            return new ApiResponse<WalletTransactionResponseDTO>
            {
                Status = 500,
                Message = "Lỗi xử lý callback MoMo"
            };
        }
    }

    public async Task<ApiResponse<WalletTransactionResponseDTO>> HandleSepayCallbackAsync(
        Dictionary<string, string> queryParams)
    {
        try
        {
            var callback = _sepayService.ParseCallback(queryParams);

            if (!_sepayService.ValidateCallback(callback))
            {
                _logger.LogWarning("Invalid Sepay callback signature for order {OrderId}", callback.order_id);
                return new ApiResponse<WalletTransactionResponseDTO>
                {
                    Status = 400,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            var isSuccess = callback.status == "success";

            return await CompleteTopUpAsync(callback.order_id, isSuccess, callback.transaction_id, "Sepay");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Sepay callback");
            return new ApiResponse<WalletTransactionResponseDTO>
            {
                Status = 500,
                Message = "Lỗi xử lý callback Sepay"
            };
        }
    }

    /// <summary>
    /// Complete a top-up transaction: credit balance if successful, mark failed otherwise.
    /// Uses row-level locking + DB transaction to prevent double-credit.
    /// </summary>
    private async Task<ApiResponse<WalletTransactionResponseDTO>> CompleteTopUpAsync(
        string idempotencyKey, bool isSuccess, string externalRef, string provider)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Find pending transaction by idempotency key
            var walletTxn = await _walletRepo.GetTransactionByIdempotencyKeyAsync(idempotencyKey);

            if (walletTxn == null)
            {
                _logger.LogWarning("Top-up transaction not found for idempotency key {Key}", idempotencyKey);
                return new ApiResponse<WalletTransactionResponseDTO>
                {
                    Status = 404,
                    Message = "Giao dịch nạp tiền không tồn tại"
                };
            }

            // Idempotency check: if already completed/failed, return existing result
            if (walletTxn.Status != "Pending")
            {
                _logger.LogInformation("Top-up {Key} already processed with status {Status}", idempotencyKey, walletTxn.Status);
                return new ApiResponse<WalletTransactionResponseDTO>
                {
                    Status = 200,
                    Message = walletTxn.Status == "Completed"
                        ? "Nạp tiền thành công (đã xử lý trước đó)"
                        : "Nạp tiền thất bại (đã xử lý trước đó)",
                    Data = MapToDTO(walletTxn)
                };
            }

            walletTxn.ExternalRef = externalRef;

            if (isSuccess)
            {
                // Lock wallet row to prevent race conditions
                var wallet = await _walletRepo.GetByAccountIdWithLockAsync(walletTxn.AccountId);
                if (wallet == null)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<WalletTransactionResponseDTO>
                    {
                        Status = 404,
                        Message = "Ví không tồn tại"
                    };
                }

                // Credit balance
                walletTxn.BalanceBefore = wallet.Balance;
                wallet.Balance += walletTxn.Amount;
                walletTxn.BalanceAfter = wallet.Balance;

                walletTxn.Status = "Completed";
                walletTxn.CompletedAt = DateTime.UtcNow;

                wallet.LastTransactionAt = DateTime.UtcNow;
                wallet.UpdatedAt = DateTime.UtcNow;

                await _walletRepo.UpdateWalletAsync(wallet);
            }
            else
            {
                walletTxn.Status = "Failed";
                walletTxn.Reason = $"Thanh toán qua {provider} thất bại";
            }

            await _walletRepo.UpdateWalletTransactionAsync(walletTxn);
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Top-up {Key} completed: {Status}, Amount={Amount}, Provider={Provider}",
                idempotencyKey, walletTxn.Status, walletTxn.Amount, provider);

            return new ApiResponse<WalletTransactionResponseDTO>
            {
                Status = 200,
                Message = isSuccess ? "Nạp tiền thành công" : "Nạp tiền thất bại",
                Data = MapToDTO(walletTxn)
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error completing top-up for key {Key}", idempotencyKey);
            return new ApiResponse<WalletTransactionResponseDTO>
            {
                Status = 500,
                Message = "Lỗi xử lý nạp tiền: " + ex.Message
            };
        }
    }

    private static WalletTransactionResponseDTO MapToDTO(WalletTransaction txn) => new()
    {
        WalletTransactionId = txn.WalletTransactionId,
        TxnType = txn.TxnType,
        Direction = txn.Direction,
        Amount = txn.Amount,
        BalanceBefore = txn.BalanceBefore,
        BalanceAfter = txn.BalanceAfter,
        Method = txn.Method,
        ExternalRef = txn.ExternalRef,
        Status = txn.Status,
        Reason = txn.Reason,
        RelatedOrderId = txn.RelatedOrderId,
        CreatedAt = txn.CreatedAt,
        CompletedAt = txn.CompletedAt
    };
}
