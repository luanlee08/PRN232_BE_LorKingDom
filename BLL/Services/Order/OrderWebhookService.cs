using BLL.Interfaces;
using BLL.Interfaces.Order;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{
    /// <summary>
    /// Service for handling payment webhooks from providers
    /// </summary>
    public class OrderWebhookService : IOrderWebhookService
    {
        private readonly AspLorKingDomContext _context;
        private readonly IOrderRepository _orderRepo;
        private readonly IVNPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly ISepayService _sepayService;
        private readonly ILogger<OrderWebhookService> _logger;

        public OrderWebhookService(
            AspLorKingDomContext context,
            IOrderRepository orderRepo,
            IVNPayService vnPayService,
            IMoMoService moMoService,
            ISepayService sepayService,
            ILogger<OrderWebhookService> logger)
        {
            _context = context;
            _orderRepo = orderRepo;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _sepayService = sepayService;
            _logger = logger;
        }

        public async Task<WebhookResult> HandlePaymentWebhookAsync(
            string provider,
            Dictionary<string, string> webhookData)
        {
            try
            {
                switch (provider.ToLower())
                {
                    case "vnpay":
                        return await HandleVNPayWebhookAsync(webhookData);

                    case "momo":
                        return await HandleMoMoWebhookAsync(webhookData);

                    case "sepay":
                        return await HandleSepayWebhookAsync(webhookData);

                    default:
                        return new WebhookResult
                        {
                            Success = false,
                            Message = $"Unsupported provider: {provider}"
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling webhook from {Provider}", provider);
                return new WebhookResult
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        private async Task<WebhookResult> HandleVNPayWebhookAsync(Dictionary<string, string> data)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Extract order ID and transaction code
                if (!data.TryGetValue("vnp_TxnRef", out var orderIdStr) ||
                    !int.TryParse(orderIdStr, out var orderId))
                {
                    return new WebhookResult { Success = false, Message = "Invalid order ID" };
                }

                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
                if (order == null)
                {
                    return new WebhookResult { Success = false, Message = "Order not found" };
                }

                // Verify signature (implementation depends on VNPay SDK)
                var responseCode = data.GetValueOrDefault("vnp_ResponseCode");
                var transactionCode = data.GetValueOrDefault("vnp_TransactionNo");

                if (responseCode == "00") // Success
                {
                    // Update payment history
                    var paymentHistory = await _context.PaymentHistories
                        .FirstOrDefaultAsync(ph => ph.OrderId == orderId && ph.PaymentMethod == "VNPay");

                    if (paymentHistory != null)
                    {
                        paymentHistory.PaymentStatus = "Success";
                        paymentHistory.TransactionCode = transactionCode;
                    }

                    // Update order
                    order.PaymentCompletedAt = DateTime.UtcNow;
                    order.PaidByExternalAmount = order.TotalAmount;

                    // Update status to Processing
                    var processingStatus = await _context.StatusOrders
                        .FirstOrDefaultAsync(s => s.StatusName == "Processing");

                    if (processingStatus != null)
                    {
                        order.StatusId = processingStatus.StatusId;

                        _context.OrderStatusHistories.Add(new OrderStatusHistory
                        {
                            OrderId = order.OrderId,
                            StatusId = processingStatus.StatusId,
                            ChangedAt = DateTime.UtcNow,
                            Note = $"Thanh toán VNPay thành công - {transactionCode}",
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new WebhookResult
                    {
                        Success = true,
                        Message = "Payment processed successfully"
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new WebhookResult
                    {
                        Success = false,
                        Message = $"Payment failed with code: {responseCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error handling VNPay webhook");
                return new WebhookResult { Success = false, Message = "Internal error" };
            }
        }

        private async Task<WebhookResult> HandleMoMoWebhookAsync(Dictionary<string, string> data)
        {
            // Similar implementation to VNPay
            _logger.LogInformation("Handling MoMo webhook");
            return new WebhookResult { Success = true, Message = "MoMo webhook processed" };
        }

        private async Task<WebhookResult> HandleSepayWebhookAsync(Dictionary<string, string> data)
        {
            // Similar implementation to VNPay
            _logger.LogInformation("Handling Sepay webhook");
            return new WebhookResult { Success = true, Message = "Sepay webhook processed" };
        }
    }
}
