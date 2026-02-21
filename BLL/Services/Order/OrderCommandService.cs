using BLL.DTOs.Orders;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{
    /// <summary>
    /// Service for handling Order command operations (write)
    /// </summary>
    public class OrderCommandService : IOrderCommandService
    {
        private readonly AspLorKingDomContext _context;
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly OrderValidationHelper _validationHelper;
        private readonly OrderCalculationHelper _calculationHelper;
        private readonly IOrderPaymentService _paymentService;
        private readonly ILogger<OrderCommandService> _logger;

        public OrderCommandService(
            AspLorKingDomContext context,
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            OrderValidationHelper validationHelper,
            OrderCalculationHelper calculationHelper,
            IOrderPaymentService paymentService,
            ILogger<OrderCommandService> logger)
        {
            _context = context;
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _validationHelper = validationHelper;
            _calculationHelper = calculationHelper;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, string ipAddress)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate cart
                var cart = await _validationHelper.ValidateCartAsync(request.AccountId);

                // 2. Validate products and calculate subtotal
                decimal subtotal = 0;
                foreach (var item in cart.CartItems)
                {
                    await _validationHelper.ValidateProductStockAsync(item.Product!, item.Quantity);
                    subtotal += item.Product!.Price * item.Quantity;
                }

                // 3. Validate and apply voucher if provided
                decimal discount = 0;
                Voucher? voucher = null;
                VoucherType? voucherType = null;
                if (request.VoucherId.HasValue)
                {
                    var (isValid, errorMsg, validatedVoucher) = await _validationHelper.ValidateVoucherAsync(request.VoucherId.Value, subtotal);
                    if (!isValid)
                    {
                        throw new InvalidOperationException(errorMsg ?? "Voucher không hợp lệ");
                    }
                    voucher = validatedVoucher;
                    if (voucher != null)
                    {
                        voucherType = await _context.VoucherTypes.FindAsync(voucher.VoucherTypeId);
                        discount = _calculationHelper.CalculateDiscount(voucher, voucherType, subtotal);
                    }
                }

                // 4. Calculate shipping fee
                decimal shippingFee = _calculationHelper.GetShippingFee(request.ShippingMethod);
                decimal totalAmount = _calculationHelper.CalculateTotalAmount(subtotal, discount, shippingFee);

                // 5. Get and validate shipping address
                var address = await _validationHelper.ValidateAndGetAddressAsync(
                    request.AddressId,
                    request.AccountId);

                // 6. Get pending status
                var pendingStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Pending);
                if (pendingStatus == null)
                {
                    throw new Exception("Order status configuration missing");
                }

                // 7. Create Order entity
                var order = new DAL.Models.Order
                {
                    AccountId = request.AccountId,
                    VoucherId = request.VoucherId,
                    StatusId = pendingStatus.StatusId,
                    ShippingName = request.ShippingName ?? "",
                    ShippingPhone = request.ShippingPhone ?? "",
                    ShippingAddressLine = request.ShippingAddressLine ?? address?.AddressLine ?? "",
                    ShippingCity = request.ShippingCity ?? address?.City ?? "",
                    ShippingWard = request.ShippingWard ?? address?.Ward ?? "",
                    ShippingMethod = request.ShippingMethod,
                    ShippingFee = shippingFee,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    PaidByWalletAmount = 0,
                    PaidByExternalAmount = 0,
                    RefundStatus = RefundStatus.None,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                order = await _orderRepo.CreateOrderAsync(order);

                // 8. Create OrderDetails and update product stock
                foreach (var cartItem in cart.CartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product!.Price,
                        Total = cartItem.Product.Price * cartItem.Quantity,
                        Reviewed = false,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _orderRepo.AddOrderDetailAsync(orderDetail);

                    // Update product quantity
                    cartItem.Product.Quantity -= cartItem.Quantity;
                    await _productRepo.UpdateAsync(cartItem.Product);
                }

                // 9. Create initial OrderStatusHistory
                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    StatusId = pendingStatus.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    Note = "Đơn hàng được tạo",
                    CreatedAt = DateTime.UtcNow
                });

                // 10. Process payment
                await ProcessPaymentAsync(order, request.PaymentMethod, request.AccountId, totalAmount, ipAddress);

                // 11. Clear cart
                await _cartRepo.DeleteAllCartItemsAsync(cart.CartId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 12. Reload order with details to return
                var createdOrder = await _orderRepo.GetByIdWithDetailsAsync(order.OrderId);
                return MapOrderToDto(createdOrder!);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        private async Task ProcessPaymentAsync(
            DAL.Models.Order order,
            string paymentMethod,
            int accountId,
            decimal totalAmount,
            string ipAddress)
        {
            if (paymentMethod == PaymentMethods.Wallet)
            {
                // Process wallet payment
                var walletResult = await _paymentService.ProcessWalletPaymentAsync(order.OrderId, accountId);
                if (!walletResult.Success)
                {
                    throw new Exception(walletResult.Message);
                }
            }
            else if (paymentMethod == PaymentMethods.COD)
            {
                // COD - just create payment history
                await _orderRepo.AddPaymentHistoryAsync(new PaymentHistory
                {
                    OrderId = order.OrderId,
                    AccountId = accountId,
                    PaymentMethod = PaymentMethods.COD,
                    PaymentStatus = PaymentStatus.Pending,
                    Amount = totalAmount,
                    Currency = "VND",
                    CreatedAt = DateTime.UtcNow
                });
            }
            else // External payment gateways
            {
                // Create payment history for external payment
                var paymentHistory = await _orderRepo.AddPaymentHistoryAsync(new PaymentHistory
                {
                    OrderId = order.OrderId,
                    AccountId = accountId,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = PaymentStatus.Pending,
                    Amount = totalAmount,
                    Currency = "VND",
                    CreatedAt = DateTime.UtcNow
                });

                // Create PaymentGatewayTransaction
                var gatewayTxn = await _orderRepo.AddPaymentGatewayTransactionAsync(new PaymentGatewayTransaction
                {
                    PaymentHistoryId = paymentHistory.PaymentHistoryId,
                    Provider = paymentMethod,
                    Status = PaymentStatus.Pending,
                    Amount = totalAmount,
                    CreatedAt = DateTime.UtcNow
                });

                // Generate payment URL
                var paymentUrl = await _paymentService.GeneratePaymentUrlAsync(
                    order.OrderId,
                    paymentMethod,
                    "https://localhost", // Will be passed from controller
                    ipAddress);

                gatewayTxn.PaymentUrl = paymentUrl;
                await _orderRepo.UpdatePaymentGatewayTransactionAsync(gatewayTxn);

                order.PaidByExternalAmount = totalAmount;
            }
        }

        public async Task CancelOrderAsync(int orderId, CancelOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null || order.IsDeleted)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                // Validate cancellation
                var (canCancel, errorMessage) = _validationHelper.ValidateCancellation(order, order.AccountId);
                if (!canCancel)
                {
                    throw new InvalidOperationException(errorMessage ?? "Cannot cancel order");
                }

                // Get Cancelled status
                var cancelledStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Cancelled);
                if (cancelledStatus == null)
                {
                    throw new Exception("Cancelled status not found");
                }

                // Update order status
                order.StatusId = cancelledStatus.StatusId;
                order.UpdatedAt = DateTime.UtcNow;

                // Restore product quantity
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.Quantity += detail.Quantity;
                        await _productRepo.UpdateAsync(detail.Product);
                    }
                }

                // Add status history
                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = cancelledStatus.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = request.CancelledBy,
                    Note = request.Reason ?? "Khách hàng hủy đơn",
                    CreatedAt = DateTime.UtcNow
                });

                // Refund if payment was completed
                if (order.PaymentCompletedAt.HasValue && order.PaidByWalletAmount > 0)
                {
                    // Create refund (will be handled by RefundService)
                    _logger.LogInformation("Order {OrderId} cancelled with payment. Refund needed.", orderId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null || order.IsDeleted)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                // Get new status
                var newStatus = await _orderRepo.GetStatusByIdAsync(request.StatusId);
                if (newStatus == null)
                {
                    throw new KeyNotFoundException("Trạng thái không hợp lệ");
                }

                // Update order
                order.StatusId = request.StatusId;
                order.UpdatedAt = DateTime.UtcNow;

                // Add status history
                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = request.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = request.ChangedBy,
                    Note = request.Note ?? $"Chuyển sang trạng thái {newStatus.StatusName}",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderId}", orderId);
                throw;
            }
        }

        public async Task ConfirmCODPaymentAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                // Update payment history
                var paymentHistory = await _context.PaymentHistories
                    .FirstOrDefaultAsync(ph => ph.OrderId == orderId && ph.PaymentMethod == PaymentMethods.COD);

                if (paymentHistory != null)
                {
                    paymentHistory.PaymentStatus = PaymentStatus.Success;
                }

                // Update order
                order.PaymentCompletedAt = DateTime.UtcNow;
                order.PaidByExternalAmount = order.TotalAmount;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming COD payment for order {OrderId}", orderId);
                throw;
            }
        }

        private OrderDto MapOrderToDto(DAL.Models.Order order)
        {
            // Simplified mapping - use OrderMappingHelper in production
            return new OrderDto
            {
                OrderId = order.OrderId,
                AccountId = order.AccountId,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingCity = order.ShippingCity,
                ShippingWard = order.ShippingWard,
                ShippingMethod = order.ShippingMethod,
                ShippingFee = order.ShippingFee,
                RefundStatus = order.RefundStatus,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }
}
