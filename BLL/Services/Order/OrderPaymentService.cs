using BLL.DTOs.Orders;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{
    public class OrderPaymentService : IOrderPaymentService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly AspLorKingDomContext _context;
        private readonly PaymentGatewayHelper _paymentGatewayHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderPaymentService> _logger;

        public OrderPaymentService(
            IOrderRepository orderRepo,
            IWalletRepository walletRepo,
            AspLorKingDomContext context,
            PaymentGatewayHelper paymentGatewayHelper,
            IConfiguration configuration,
            ILogger<OrderPaymentService> logger)
        {
            _orderRepo = orderRepo;
            _walletRepo = walletRepo;
            _context = context;
            _paymentGatewayHelper = paymentGatewayHelper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<PaymentMethodInfo>> GetPaymentMethodsAsync(decimal orderTotal)
        {
            try
            {
                var paymentMethods = new List<PaymentMethodInfo>
                {
                    new PaymentMethodInfo
                    {
                        Code = PaymentMethods.COD,
                        Name = "Thanh toán khi nhận hàng (COD)",
                        Description = "Thanh toán bằng tiền mặt khi nhận hàng",
                        Icon = "💵",
                        IsAvailable = orderTotal <= PaymentLimits.COD.MaxAmount,
                        MinAmount = PaymentLimits.COD.MinAmount,
                        MaxAmount = PaymentLimits.COD.MaxAmount,
                        TransactionFee = 0,
                        TransactionFeeType = "Fixed"
                    },
                    new PaymentMethodInfo
                    {
                        Code = PaymentMethods.Wallet,
                        Name = "Ví điện tử LorKingDom",
                        Description = "Thanh toán bằng số dư ví trong hệ thống",
                        Icon = "💰",
                        IsAvailable = true,
                        MinAmount = PaymentLimits.Wallet.MinAmount,
                        MaxAmount = PaymentLimits.Wallet.MaxAmount,
                        TransactionFee = 0,
                        TransactionFeeType = "Fixed"
                    },
                    new PaymentMethodInfo
                    {
                        Code = PaymentMethods.VNPay,
                        Name = "VNPay",
                        Description = "Thanh toán qua ví VNPay, thẻ ATM, thẻ tín dụng",
                        Icon = "🏦",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["VNPay:TmnCode"]) &&
                                     _configuration["VNPay:TmnCode"] != "YOUR_VNPAY_TMNCODE" &&
                                     orderTotal >= PaymentLimits.VNPay.MinAmount &&
                                     orderTotal <= PaymentLimits.VNPay.MaxAmount,
                        MinAmount = PaymentLimits.VNPay.MinAmount,
                        MaxAmount = PaymentLimits.VNPay.MaxAmount,
                        TransactionFee = PaymentLimits.VNPay.TransactionFeePercent,
                        TransactionFeeType = "Percentage"
                    },
                    new PaymentMethodInfo
                    {
                        Code = PaymentMethods.MoMo,
                        Name = "MoMo E-Wallet",
                        Description = "Thanh toán qua ví điện tử MoMo",
                        Icon = "📱",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["MoMo:PartnerCode"]) &&
                                     _configuration["MoMo:PartnerCode"] != "YOUR_MOMO_PARTNER_CODE" &&
                                     orderTotal >= PaymentLimits.MoMo.MinAmount &&
                                     orderTotal <= PaymentLimits.MoMo.MaxAmount,
                        MinAmount = PaymentLimits.MoMo.MinAmount,
                        MaxAmount = PaymentLimits.MoMo.MaxAmount,
                        TransactionFee = PaymentLimits.MoMo.TransactionFeePercent,
                        TransactionFeeType = "Percentage"
                    },
                    new PaymentMethodInfo
                    {
                        Code = PaymentMethods.Sepay,
                        Name = "Sepay - Chuyển khoản ngân hàng",
                        Description = "Thanh toán qua chuyển khoản ngân hàng tự động",
                        Icon = "🏧",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["Sepay:MerchantId"]) &&
                                     orderTotal >= PaymentLimits.Sepay.MinAmount &&
                                     orderTotal <= PaymentLimits.Sepay.MaxAmount,
                        MinAmount = PaymentLimits.Sepay.MinAmount,
                        MaxAmount = PaymentLimits.Sepay.MaxAmount,
                        TransactionFee = PaymentLimits.Sepay.TransactionFeePercent,
                        TransactionFeeType = "Fixed"
                    }
                };

                return paymentMethods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment methods");
                throw;
            }
        }

        public async Task<PaymentResult> ProcessWalletPaymentAsync(int orderId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get order
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
                if (order == null || order.AccountId != userId)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                // Calculate amount to pay
                var amount = order.TotalAmount - order.PaidByWalletAmount;

                // Lock wallet row
                var wallet = await _walletRepo.GetByAccountIdWithLockAsync(userId);

                if (wallet == null)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Ví không tồn tại"
                    };
                }

                if (wallet.Balance < amount)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = $"Số dư ví không đủ (hiện có: {wallet.Balance:N0} VND, cần: {amount:N0} VND)"
                    };
                }

                // Deduct wallet balance
                wallet.Balance -= amount;
                wallet.LastTransactionAt = DateTime.UtcNow;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepo.UpdateWalletAsync(wallet);

                // Create WalletTransaction
                var walletTxn = new WalletTransaction
                {
                    AccountId = userId,
                    TxnType = WalletTransactionTypes.Payment,
                    Direction = WalletDirection.Out,
                    Amount = amount,
                    BalanceBefore = wallet.Balance + amount,
                    BalanceAfter = wallet.Balance,
                    RelatedOrderId = order.OrderId,
                    Status = "Completed",
                    IdempotencyKey = $"{order.OrderId}_wallet_{DateTime.UtcNow.Ticks}",
                    Reason = $"Thanh toán đơn hàng #{order.OrderId}",
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                };
                await _walletRepo.AddWalletTransactionAsync(walletTxn);

                // Create PaymentHistory
                await _orderRepo.AddPaymentHistoryAsync(new PaymentHistory
                {
                    OrderId = order.OrderId,
                    AccountId = userId,
                    PaymentMethod = PaymentMethods.Wallet,
                    PaymentStatus = PaymentStatus.Success,
                    Amount = amount,
                    Currency = "VND",
                    WalletTransactionId = walletTxn.WalletTransactionId,
                    TransactionCode = walletTxn.IdempotencyKey,
                    CreatedAt = DateTime.UtcNow
                });

                // Update order
                order.PaidByWalletAmount = order.PaidByWalletAmount + amount;
                order.PaymentCompletedAt = DateTime.UtcNow;

                // Update status to Processing
                var processingStatus = await _context.StatusOrders
                    .FirstOrDefaultAsync(s => s.StatusName == OrderStatusNames.Processing);

                if (processingStatus != null)
                {
                    order.StatusId = processingStatus.StatusId;
                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        StatusId = processingStatus.StatusId,
                        ChangedAt = DateTime.UtcNow,
                        Note = "Thanh toán ví thành công",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PaymentResult
                {
                    Success = true,
                    Message = "Thanh toán ví thành công",
                    TransactionCode = walletTxn.IdempotencyKey,
                    Amount = amount,
                    CompletedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing wallet payment for order {OrderId}", orderId);
                return new PaymentResult
                {
                    Success = false,
                    Message = "Lỗi xử lý thanh toán ví: " + ex.Message
                };
            }
        }

        public async Task<string> GeneratePaymentUrlAsync(
            int orderId,
            string paymentMethod,
            string baseUrl,
            string ipAddress)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                var amount = order.TotalAmount - order.PaidByWalletAmount;

                return await _paymentGatewayHelper.GeneratePaymentUrlAsync(
                    paymentMethod,
                    orderId,
                    amount,
                    baseUrl,
                    ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment URL for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<PaymentCallbackResult> HandlePaymentCallbackAsync(
            string provider,
            Dictionary<string, string> queryParams)
        {
            try
            {
                // This will be implemented based on the specific provider logic
                // For now, return a basic structure
                throw new NotImplementedException("Payment callback handling will be implemented in webhook service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment callback from {Provider}", provider);
                throw;
            }
        }
    }
}
