using BLL.DTOs.Orders;
using BLL.Events;
using BLL.Events.Order;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Infrastructure;
using DAL.Interface;
using DAL.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{

    public class OrderCommandService : IOrderCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly OrderValidationHelper _validationHelper;
        private readonly OrderCalculationHelper _calculationHelper;
        private readonly IOrderPaymentService _paymentService;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ILogger<OrderCommandService> _logger;

        public OrderCommandService(
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IVoucherRepository voucherRepo,
            OrderValidationHelper validationHelper,
            OrderCalculationHelper calculationHelper,
            IOrderPaymentService paymentService,
            IDomainEventDispatcher dispatcher,
            ILogger<OrderCommandService> logger)
        {
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _voucherRepo = voucherRepo;
            _validationHelper = validationHelper;
            _calculationHelper = calculationHelper;
            _paymentService = paymentService;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request, string ipAddress)
        {
            await _unitOfWork.BeginTransactionAsync();
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
                    if (voucher?.VoucherTypeId != null)
                    {
                        voucherType = await _voucherRepo.GetVoucherTypeByIdAsync(voucher.VoucherTypeId);
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

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // 12. Reload order with details to return
                var createdOrder = await _orderRepo.GetByIdWithDetailsAsync(order.OrderId);
                var dto = MapOrderToDto(createdOrder!);

                // 13. Dispatch domain event AFTER transaction — notification failure won't roll back the order
                await _dispatcher.DispatchAsync(new OrderCreatedEvent
                {
                    OrderId = order.OrderId,
                    AccountId = request.AccountId,
                    TotalAmount = totalAmount,
                    PaymentMethod = request.PaymentMethod,
                    ShippingName = request.ShippingName ?? ""
                });

                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
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
                var walletResult = await _paymentService.ProcessWalletPaymentAsync(order.OrderId, accountId);
                if (!walletResult.Success)
                {
                    throw new Exception(walletResult.Message);
                }
            }
            else if (paymentMethod == PaymentMethods.COD)
            {
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

                var gatewayTxn = await _orderRepo.AddPaymentGatewayTransactionAsync(new PaymentGatewayTransaction
                {
                    PaymentHistoryId = paymentHistory.PaymentHistoryId,
                    Provider = paymentMethod,
                    Status = PaymentStatus.Pending,
                    Amount = totalAmount,
                    CreatedAt = DateTime.UtcNow
                });

                var paymentUrl = await _paymentService.GeneratePaymentUrlAsync(
                    order.OrderId,
                    paymentMethod,
                    "https://localhost",
                    ipAddress);

                gatewayTxn.PaymentUrl = paymentUrl;
                await _orderRepo.UpdatePaymentGatewayTransactionAsync(gatewayTxn);

                order.PaidByExternalAmount = totalAmount;
            }
        }

        public async Task CancelOrderAsync(int orderId, CancelOrderRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null || order.IsDeleted)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                var requesterId = request.CancelledBy ?? order.AccountId;
                var (canCancel, errorMessage) = _validationHelper.ValidateCancellation(order, requesterId);
                if (!canCancel)
                {
                    throw new InvalidOperationException(errorMessage ?? "Cannot cancel order");
                }

                // Validate state transition via domain rule
                Domain.OrderStatusTransitions.ThrowIfInvalid(order.Status.StatusName, OrderStatusNames.Cancelled);

                var cancelledStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Cancelled);
                if (cancelledStatus == null)
                {
                    throw new Exception("Cancelled status not found");
                }

                var hasPayment = order.PaymentCompletedAt.HasValue && order.PaidByWalletAmount > 0;

                order.StatusId = cancelledStatus.StatusId;
                order.UpdatedAt = DateTime.UtcNow;

                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.Quantity += detail.Quantity;
                        await _productRepo.UpdateAsync(detail.Product);
                    }
                }

                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = cancelledStatus.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = request.CancelledBy,
                    Note = request.Reason ?? "Khách hàng hủy đơn",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Dispatch event — notification sent by OrderCancelledNotificationHandler
                await _dispatcher.DispatchAsync(new OrderCancelledEvent
                {
                    OrderId = orderId,
                    AccountId = order.AccountId,
                    TotalAmount = order.TotalAmount,
                    Reason = request.Reason,
                    HasPaymentToRefund = hasPayment
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
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

                var newStatus = await _orderRepo.GetStatusByIdAsync(request.StatusId);
                if (newStatus == null)
                {
                    throw new KeyNotFoundException("Trạng thái không hợp lệ");
                }

                var oldStatusName = order.Status.StatusName;

                // Validate state transition via domain rule
                Domain.OrderStatusTransitions.ThrowIfInvalid(oldStatusName, newStatus.StatusName);

                order.StatusId = request.StatusId;
                order.UpdatedAt = DateTime.UtcNow;

                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = request.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = request.ChangedBy,
                    Note = request.Note ?? $"Chuyển sang trạng thái {newStatus.StatusName}",
                    CreatedAt = DateTime.UtcNow
                });

                await _unitOfWork.SaveChangesAsync();

                // Dispatch event — notification sent by OrderStatusChangedNotificationHandler
                var shippingTx = order.ShippingProviderTransactions.FirstOrDefault();
                await _dispatcher.DispatchAsync(new OrderStatusChangedEvent
                {
                    OrderId = orderId,
                    AccountId = order.AccountId,
                    CustomerName = order.Account?.AccountName,
                    OldStatus = oldStatusName,
                    NewStatus = newStatus.StatusName,
                    Note = request.Note,
                    TrackingNumber = shippingTx?.TrackingNumber ?? shippingTx?.ProviderOrderCode,
                    ShippingProvider = shippingTx?.Provider
                });
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

                // Use repository instead of DbContext directly
                var paymentHistory = await _orderRepo.GetPaymentHistoryByOrderIdAndMethodAsync(orderId, PaymentMethods.COD);

                if (paymentHistory != null)
                {
                    paymentHistory.PaymentStatus = PaymentStatus.Success;
                }

                order.PaymentCompletedAt = DateTime.UtcNow;
                order.PaidByExternalAmount = order.TotalAmount;

                await _unitOfWork.SaveChangesAsync();

                // Dispatch payment confirmed event
                await _dispatcher.DispatchAsync(new OrderPaidEvent
                {
                    OrderId = orderId,
                    AccountId = order.AccountId,
                    Amount = order.TotalAmount,
                    PaymentMethod = PaymentMethods.COD
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming COD payment for order {OrderId}", orderId);
                throw;
            }
        }

        private static OrderDto MapOrderToDto(DAL.Models.Order order)
        {
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