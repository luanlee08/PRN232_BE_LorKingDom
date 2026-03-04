using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{

    public class OrderRefundService : IOrderRefundService
    {
        private readonly AspLorKingDomContext _context;
        private readonly IOrderRepository _orderRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly OrderValidationHelper _validationHelper;
        private readonly OrderMappingHelper _mapper;
        private readonly ILogger<OrderRefundService> _logger;

        public OrderRefundService(
            AspLorKingDomContext context,
            IOrderRepository orderRepo,
            IWalletRepository walletRepo,
            OrderValidationHelper validationHelper,
            OrderMappingHelper mapper,
            ILogger<OrderRefundService> logger)
        {
            _context = context;
            _orderRepo = orderRepo;
            _walletRepo = walletRepo;
            _validationHelper = validationHelper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderRefundDto> CreateRefundAsync(int orderId, CreateRefundRequest request)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                // Validate refund request
                var (isValid, errorMessage) = _validationHelper.ValidateRefundRequest(order, order.AccountId, request.RefundAmount);
                if (!isValid)
                {
                    throw new InvalidOperationException(errorMessage ?? "Invalid refund request");
                }

                // Create refund record
                var refund = new OrderRefund
                {
                    OrderId = orderId,
                    AccountId = order.AccountId,
                    RefundMode = request.RefundMode,
                    RefundStatus = RefundStatus.Requested,
                    TotalAmount = order.TotalAmount,
                    RefundAmount = request.RefundAmount,
                    Reason = request.Reason,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderRefunds.Add(refund);
                await _context.SaveChangesAsync();

                return _mapper.MapRefundToDto(refund);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refund for order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<OrderRefundDto> ProcessRefundAsync(int refundId, ProcessRefundRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var refund = await _context.OrderRefunds
                    .Include(r => r.Order)
                    .FirstOrDefaultAsync(r => r.RefundId == refundId);

                if (refund == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy yêu cầu hoàn tiền");
                }

                if (refund.RefundStatus != RefundStatus.Requested)
                {
                    throw new InvalidOperationException("Yêu cầu hoàn tiền đã được xử lý");
                }

                if (request.IsApproved)
                {
                    // Approve refund
                    refund.RefundStatus = RefundStatus.Approved;
                    refund.ApprovedAt = DateTime.UtcNow;
                    refund.ApprovedBy = request.ApprovedBy;

                    // Process refund to wallet
                    if (refund.RefundMode == RefundModes.Wallet)
                    {
                        var wallet = await _walletRepo.GetByAccountIdAsync(refund.AccountId);
                        if (wallet != null)
                        {
                            wallet.Balance += refund.RefundAmount;
                            wallet.LastTransactionAt = DateTime.UtcNow;
                            wallet.UpdatedAt = DateTime.UtcNow;
                            await _walletRepo.UpdateWalletAsync(wallet);

                            // Create wallet transaction
                            await _walletRepo.AddWalletTransactionAsync(new WalletTransaction
                            {
                                AccountId = refund.AccountId,
                                TxnType = WalletTransactionTypes.Refund,
                                Direction = WalletDirection.In,
                                Amount = refund.RefundAmount,
                                BalanceBefore = wallet.Balance - refund.RefundAmount,
                                BalanceAfter = wallet.Balance,
                                RelatedOrderId = refund.OrderId,
                                Status = "Completed",
                                IdempotencyKey = $"refund_{refundId}_{DateTime.UtcNow.Ticks}",
                                Reason = $"Hoàn tiền đơn hàng #{refund.OrderId}",
                                CreatedAt = DateTime.UtcNow,
                                CompletedAt = DateTime.UtcNow
                            });
                        }

                        refund.ProcessedAt = DateTime.UtcNow;
                    }

                    // Update order refund status
                    if (refund.Order != null)
                    {
                        refund.Order.RefundStatus = RefundStatus.Approved;
                        refund.Order.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Reject refund
                    refund.RefundStatus = RefundStatus.Rejected;
                    refund.ApprovedAt = DateTime.UtcNow;
                    refund.ApprovedBy = request.ApprovedBy;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.MapRefundToDto(refund);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing refund {RefundId}", refundId);
                throw;
            }
        }

        public async Task<PagedResult<OrderRefundDto>> GetRefundsAsync(
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.OrderRefunds
                    .Include(r => r.Order)
                    .Include(r => r.ApprovedByNavigation)
                    .AsQueryable();

                if (status != null)
                {
                    query = query.Where(r => r.RefundStatus == status);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt <= toDate.Value);
                }

                var totalCount = await query.CountAsync();
                var skip = (pageNumber - 1) * pageSize;

                var refunds = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                var refundDtos = refunds.Select(r => _mapper.MapRefundToDto(r)).ToList();

                return new PagedResult<OrderRefundDto>
                {
                    Items = refundDtos,
                    TotalCount = totalCount,
                    Page = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refunds");
                throw;
            }
        }

        public async Task<OrderRefundDto> GetRefundByIdAsync(int refundId)
        {
            try
            {
                var refund = await _context.OrderRefunds
                    .Include(r => r.Order)
                    .Include(r => r.ApprovedByNavigation)
                    .FirstOrDefaultAsync(r => r.RefundId == refundId);

                if (refund == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy yêu cầu hoàn tiền");
                }

                return _mapper.MapRefundToDto(refund);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund {RefundId}", refundId);
                throw;
            }
        }
    }
}
