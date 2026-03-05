using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.DTOs.Orders;
using BLL.DTOs.PaymentGateway;
using BLL.DTOs.Shipping;
using BLL.Helpers.Notification;
using BLL.Interfaces;
using BLL.Interfaces.Notification;
using DAL.Interface;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly AspLorKingDomContext _context;
        private readonly IOrderRepository _orderRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IVNPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly ISepayService _sepayService;
        private readonly IGHNService _ghnService;
        private readonly INotificationCommandService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            AspLorKingDomContext context,
            IOrderRepository orderRepo,
            IWalletRepository walletRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IVNPayService vnPayService,
            IMoMoService moMoService,
            ISepayService sepayService,
            IGHNService ghnService,
            INotificationCommandService notificationService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<OrderService> logger)
        {
            _context = context;
            _orderRepo = orderRepo;
            _walletRepo = walletRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _sepayService = sepayService;
            _ghnService = ghnService;
            _notificationService = notificationService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #region Payment Methods

        public async Task<ApiResponse<GetPaymentMethodsResponse>> GetAvailablePaymentMethodsAsync()
        {
            try
            {
                var paymentMethods = new List<PaymentMethodDTO>
                {
                    new PaymentMethodDTO
                    {
                        Code = PaymentMethods.COD,
                        Name = "Thanh toán khi nhận hàng (COD)",
                        Description = "Thanh toán bằng tiền mặt khi nhận hàng",
                        Icon = "💵",
                        IsAvailable = true,
                        MinAmount = 0,
                        MaxAmount = 50000000, // 50 triệu
                        TransactionFee = 0,
                        TransactionFeeType = "Fixed"
                    },
                    new PaymentMethodDTO
                    {
                        Code = PaymentMethods.Wallet,
                        Name = "Ví điện tử LorKingDom",
                        Description = "Thanh toán bằng số dư ví trong hệ thống",
                        Icon = "💰",
                        IsAvailable = true,
                        MinAmount = 0,
                        MaxAmount = decimal.MaxValue,
                        TransactionFee = 0,
                        TransactionFeeType = "Fixed"
                    },
                    new PaymentMethodDTO
                    {
                        Code = PaymentMethods.VNPay,
                        Name = "VNPay",
                        Description = "Thanh toán qua ví VNPay, thẻ ATM, thẻ tín dụng",
                        Icon = "🏦",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["VNPay:TmnCode"]) &&
                                     _configuration["VNPay:TmnCode"] != "YOUR_VNPAY_TMNCODE",
                        MinAmount = 10000,
                        MaxAmount = 200000000, // 200 triệu
                        TransactionFee = 1.5m,
                        TransactionFeeType = "Percentage"
                    },
                    new PaymentMethodDTO
                    {
                        Code = PaymentMethods.MoMo,
                        Name = "MoMo E-Wallet",
                        Description = "Thanh toán qua ví điện tử MoMo",
                        Icon = "📱",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["MoMo:PartnerCode"]) &&
                                     _configuration["MoMo:PartnerCode"] != "YOUR_MOMO_PARTNER_CODE",
                        MinAmount = 10000,
                        MaxAmount = 50000000, // 50 triệu
                        TransactionFee = 1.0m,
                        TransactionFeeType = "Percentage"
                    },
                    new PaymentMethodDTO
                    {
                        Code = PaymentMethods.Sepay,
                        Name = "Sepay - Chuyển khoản ngân hàng",
                        Description = "Thanh toán qua chuyển khoản ngân hàng tự động",
                        Icon = "🏧",
                        IsAvailable = !string.IsNullOrEmpty(_configuration["Sepay:MerchantId"]),
                        MinAmount = 10000,
                        MaxAmount = 500000000, // 500 triệu
                        TransactionFee = 0,
                        TransactionFeeType = "Fixed"
                    }
                };

                return new ApiResponse<GetPaymentMethodsResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách phương thức thanh toán thành công",
                    Data = new GetPaymentMethodsResponse
                    {
                        PaymentMethods = paymentMethods
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment methods");
                return new ApiResponse<GetPaymentMethodsResponse>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Lỗi khi lấy danh sách phương thức thanh toán"
                };
            }
        }

        #endregion

        #region Create Order

        public async Task<ApiResponse<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request, int accountId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Creating order for account {AccountId} with payment method {PaymentMethod}",
                    accountId, request.PaymentMethod);

                // 1. Validate cart and get items
                var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);

                if (cart == null || !cart.CartItems.Any())
                {
                    _logger.LogWarning("Cart is empty for account {AccountId}", accountId);
                    return new ApiResponse<CreateOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Giỏ hàng trống"
                    };
                }

                _logger.LogInformation("Found cart with {ItemCount} items", cart.CartItems.Count());

                // Filter to only the selected cart items (null = use entire cart)
                var itemsToOrder = (request.CartItemIds is { Count: > 0 })
                    ? cart.CartItems.Where(i => request.CartItemIds.Contains(i.CartItemId)).ToList()
                    : cart.CartItems.ToList();

                if (!itemsToOrder.Any())
                {
                    return new ApiResponse<CreateOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Không có sản phẩm nào được chọn để đặt hàng"
                    };
                }

                // 2. Validate products and calculate subtotal
                decimal subtotal = 0;
                foreach (var item in itemsToOrder)
                {
                    if (item.Product == null || item.Product.IsDeleted)
                    {
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = $"Sản phẩm {item.ProductId} không tồn tại"
                        };
                    }

                    if (item.Product.Quantity < item.Quantity)
                    {
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = $"Sản phẩm {item.Product.ProductName} không đủ số lượng (còn {item.Product.Quantity})"
                        };
                    }

                    subtotal += item.Product.Price * item.Quantity;
                }

                // 3. Validate and apply voucher if provided
                decimal discount = 0;
                Voucher? voucher = null;
                if (request.VoucherId.HasValue)
                {
                    voucher = await _context.Vouchers
                        .FirstOrDefaultAsync(v => v.VoucherId == request.VoucherId.Value && !v.IsDeleted);

                    if (voucher == null || voucher.Status != "Active")
                    {
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = "Voucher không hợp lệ"
                        };
                    }

                    if (DateTime.Now < voucher.StartDate || DateTime.Now > voucher.EndDate)
                    {
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = "Voucher đã hết hạn"
                        };
                    }

                    if (voucher.MinOrderAmount.HasValue && subtotal < voucher.MinOrderAmount)
                    {
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0} VND để áp dụng voucher này"
                        };
                    }

                    // Calculate discount based on DiscountType (Fixed / Percentage)
                    // VoucherType stays as order vs shipping — handled separately
                    decimal rawDiscount;
                    if (voucher.DiscountType == "Percentage")
                    {
                        rawDiscount = subtotal * voucher.DiscountValue / 100;
                        if (voucher.MaxDiscountAmount.HasValue && rawDiscount > voucher.MaxDiscountAmount.Value)
                            rawDiscount = voucher.MaxDiscountAmount.Value;
                    }
                    else
                    {
                        rawDiscount = voucher.DiscountValue;
                    }
                    discount = rawDiscount;
                }

                // 4. Calculate shipping fee
                // Use client-provided fee if available (from GHN real-time calculation)
                // Otherwise fallback to fixed pricing
                decimal shippingFee = request.ShippingFee ?? (request.ShippingMethod switch
                {
                    ShippingMethods.Express => 50000,
                    ShippingMethods.Standard => 30000,
                    ShippingMethods.Economy => 20000,
                    _ => 30000
                });

                decimal totalAmount = subtotal - discount + shippingFee;

                // 5. Get shipping address
                Address? address = null;
                if (request.AddressId.HasValue)
                {
                    address = await _context.Addresses
                        .FirstOrDefaultAsync(a => a.AddressId == request.AddressId.Value
                            && a.AccountId == accountId
                            && !a.IsDeleted);
                }

                // 6. Get pending status
                var pendingStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Pending);

                if (pendingStatus == null)
                {
                    throw new Exception("Order status configuration missing");
                }

                // 7. Create Order
                var order = new DAL.Models.Order
                {
                    AccountId = accountId,
                    VoucherId = request.VoucherId,
                    StatusId = pendingStatus.StatusId,
                    ShippingName = request.ShippingName ?? "",
                    ShippingPhone = request.ShippingPhone ?? "",
                    ShippingAddressLine = request.ShippingAddressLine ?? address?.AddressLine ?? "",

                    // Text names (for display)
                    ShippingCity = request.ShippingCity ?? address?.City ?? "",
                    ShippingDistrict = request.ShippingDistrict ?? address?.District ?? "",
                    ShippingWard = request.ShippingWard ?? address?.Ward ?? "",

                    // GHN IDs (for reliable shipping)
                    ShippingProvinceId = request.ShippingProvinceId ?? address?.ProvinceId,
                    ShippingDistrictId = request.ShippingDistrictId ?? address?.DistrictId,
                    ShippingWardCode = request.ShippingWardCode ?? address?.WardCode,

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

                _context.Orders.Add(order);
                order = await _orderRepo.CreateOrderAsync(order);

                // 8. Create OrderDetails and update product stock
                foreach (var cartItem in itemsToOrder)
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

                // 10. Process payment based on method
                string? paymentUrl = null;
                var paymentMethod = request.PaymentMethod;

                if (paymentMethod == PaymentMethods.Wallet)
                {
                    // Process wallet payment
                    var walletResult = await ProcessWalletPaymentAsync(order, accountId, totalAmount, transaction);
                    if (!walletResult.Success)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<CreateOrderResponse>
                        {
                            Status = 400,
                            StatusMessage = "FAILED",
                            Message = walletResult.Message
                        };
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
                    paymentUrl = await GeneratePaymentUrlAsync(paymentMethod, order.OrderId, totalAmount, gatewayTxn.GatewayTransactionId);
                    gatewayTxn.PaymentUrl = paymentUrl;
                    await _orderRepo.UpdatePaymentGatewayTransactionAsync(gatewayTxn);

                    order.PaidByExternalAmount = totalAmount;
                }

                // 11. Clear ordered items from cart (partial if CartItemIds specified)
                if (request.CartItemIds is { Count: > 0 })
                {
                    foreach (var itemId in request.CartItemIds)
                        await _cartRepo.DeleteCartItemAsync(itemId);
                }
                else
                {
                    await _cartRepo.DeleteAllCartItemsAsync(cart.CartId);
                }

                // 12. Send ORDER_CREATED notification (system-generated)
                try
                {
                    var orderCode = $"ORD{order.OrderId:D6}";
                    var payload = JsonSerializer.Serialize(new
                    {
                        type = "order",
                        orderId = order.OrderId,
                        orderCode = orderCode,
                        totalAmount = totalAmount,
                        link = $"/orders/{order.OrderId}"
                    });

                    await _notificationService.SendNotificationAsync(
                        new SendNotificationRequest
                        {
                            TemplateCode = NotificationConstants.SystemOnlyTemplateCodes.OrderCreated,
                            TargetType = NotificationConstants.TargetTypes.User,
                            TargetUserIds = new List<int> { accountId },
                            Parameters = new Dictionary<string, string>
                            {
                                { "orderCode", orderCode },
                                { "totalAmount", totalAmount.ToString("N0") }
                            },
                            Payload = payload
                        },
                        createdByAccountId: 0, // System account
                        isSystemGenerated: true // Bypass admin restrictions
                    );
                }
                catch (Exception notifEx)
                {
                    // Log but don't fail the order creation
                    _logger.LogError(notifEx, "Failed to send ORDER_CREATED notification for order {OrderId}", order.OrderId);
                }

                await transaction.CommitAsync();

                return new ApiResponse<CreateOrderResponse>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = "Đặt hàng thành công",
                    Data = new CreateOrderResponse
                    {
                        OrderId = order.OrderId,
                        PaymentMethod = paymentMethod,
                        PaymentUrl = paymentUrl,
                        TotalAmount = totalAmount,
                        Message = paymentMethod == PaymentMethods.Wallet
                            ? "Thanh toán thành công"
                            : paymentMethod == PaymentMethods.COD
                                ? "Đơn hàng sẽ thanh toán khi nhận hàng"
                                : "Vui lòng thanh toán để hoàn tất đơn hàng"
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order for account {AccountId}", accountId);
                return new ApiResponse<CreateOrderResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra khi tạo đơn hàng: " + ex.Message
                };
            }
        }

        private async Task<(bool Success, string Message)> ProcessWalletPaymentAsync(
            DAL.Models.Order order, int accountId, decimal amount, Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
        {
            try
            {
                // Lock wallet row
                var wallet = await _walletRepo.GetByAccountIdWithLockAsync(accountId);

                if (wallet == null)
                {
                    return (false, "Ví không tồn tại");
                }

                if (wallet.Balance < amount)
                {
                    return (false, $"Số dư ví không đủ (hiện có: {wallet.Balance:N0} VND)");
                }

                // Deduct wallet balance
                wallet.Balance -= amount;
                wallet.LastTransactionAt = DateTime.UtcNow;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepo.UpdateWalletAsync(wallet);

                // Create WalletTransaction
                var walletTxn = new WalletTransaction
                {
                    WalletId = wallet.WalletId,
                    AccountId = accountId,
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
                    AccountId = accountId,
                    PaymentMethod = PaymentMethods.Wallet,
                    PaymentStatus = PaymentStatus.Success,
                    Amount = amount,
                    Currency = "VND",
                    WalletTransactionId = walletTxn.WalletTransactionId,
                    TransactionCode = walletTxn.IdempotencyKey,
                    CreatedAt = DateTime.UtcNow
                });

                // Update order
                order.PaidByWalletAmount = amount;
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

                // Send payment success notification (fire-and-forget, don't await to avoid blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var orderCode = $"ORD{order.OrderId:D6}";
                        await SendPaymentNotificationAsync(order.OrderId, accountId, orderCode, true, PaymentMethods.Wallet, amount);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send payment notification for Order {OrderId}", order.OrderId);
                    }
                });

                return (true, "Thanh toán ví thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing wallet payment");
                return (false, "Lỗi xử lý thanh toán ví: " + ex.Message);
            }
        }

        private async Task<string> GeneratePaymentUrlAsync(string provider, int orderId, decimal amount, long gatewayTransactionId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";

            try
            {
                switch (provider)
                {
                    case PaymentMethods.VNPay:
                        var vnpayRequest = new VNPayRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/vnpay-return",
                            IpAddress = ipAddress
                        };
                        var vnpayResponse = await _vnPayService.CreatePaymentUrlAsync(vnpayRequest);
                        return vnpayResponse.PaymentUrl;

                    case PaymentMethods.MoMo:
                        var momoRequest = new MoMoRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/momo-return",
                            NotifyUrl = $"{baseUrl}/api/order/webhook/payment/momo"
                        };
                        var momoResponse = await _moMoService.CreatePaymentAsync(momoRequest);
                        return momoResponse.PayUrl;

                    case PaymentMethods.Sepay:
                        var sepayRequest = new SepayRequest
                        {
                            OrderId = orderId.ToString(),
                            Amount = amount,
                            OrderInfo = $"Thanh toán đơn hàng #{orderId}",
                            ReturnUrl = $"{baseUrl}/api/order/sepay-return",
                            CancelUrl = $"{baseUrl}/api/order/sepay-cancel",
                            NotifyUrl = $"{baseUrl}/api/order/webhook/payment/sepay"
                        };
                        var sepayResponse = await _sepayService.CreatePaymentAsync(sepayRequest);
                        return sepayResponse.PaymentUrl;

                    default:
                        throw new Exception($"Unsupported payment provider: {provider}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment URL for provider {Provider}", provider);
                throw;
            }
        }

        #endregion

        #region Get Orders

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId, int accountId)
        {
            try
            {
                var order = await _orderRepo.GetByIdForAccountAsync(orderId, accountId);

                if (order == null)
                {
                    return new ApiResponse<OrderDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                var orderDto = MapToDto(order);

                return new ApiResponse<OrderDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy thông tin đơn hàng thành công",
                    Data = orderDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                return new ApiResponse<OrderDto>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<PagedResult<OrderDto>>> GetMyOrdersAsync(
            int accountId, int pageNumber = 1, int pageSize = 10, string? statusFilter = null)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;
                var orders = await _orderRepo.GetOrdersByAccountIdAsync(accountId, skip, pageSize, statusFilter);
                var totalCount = await _orderRepo.GetOrdersCountByAccountIdAsync(accountId, statusFilter);

                var orderDtos = orders.Select(MapToDto).ToList();

                return new ApiResponse<PagedResult<OrderDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách đơn hàng thành công",
                    Data = new PagedResult<OrderDto>
                    {
                        Items = orderDtos,
                        TotalCount = totalCount,
                        Page = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for account {AccountId}", accountId);
                return new ApiResponse<PagedResult<OrderDto>>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<PagedResult<OrderDto>>> GetAllOrdersAsync(
            int pageNumber = 1, int pageSize = 10, string? statusFilter = null)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;
                var orders = await _orderRepo.GetAllOrdersAsync(skip, pageSize, statusFilter);
                var totalCount = await _orderRepo.GetTotalOrdersCountAsync(statusFilter);

                var orderDtos = orders.Select(MapToDto).ToList();

                return new ApiResponse<PagedResult<OrderDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách đơn hàng thành công",
                    Data = new PagedResult<OrderDto>
                    {
                        Items = orderDtos,
                        TotalCount = totalCount,
                        Page = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return new ApiResponse<PagedResult<OrderDto>>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        #endregion

        #region Cancel Order

        public async Task<ApiResponse<object>> CancelOrderAsync(int orderId, int accountId, string? reason = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null || order.AccountId != accountId || order.IsDeleted)
                {
                    return new ApiResponse<object>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                // Can only cancel Pending or Processing orders
                if (order.Status.StatusName != OrderStatusNames.Pending &&
                    order.Status.StatusName != OrderStatusNames.Processing)
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Không thể hủy đơn hàng ở trạng thái hiện tại"
                    };
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
                    }
                }

                // Create status history
                await _orderRepo.AddOrderStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = cancelledStatus.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = accountId,
                    Note = reason ?? "Khách hàng hủy đơn",
                    CreatedAt = DateTime.UtcNow
                });

                // If paid by wallet, refund
                if (order.PaidByWalletAmount > 0)
                {
                    await RefundToWalletAsync(order, accountId, order.PaidByWalletAmount, "Hoàn tiền do hủy đơn");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Hủy đơn hàng thành công"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                return new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        #endregion

        #region Status Management

        public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(
            int orderId, UpdateOrderStatusRequest request, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.IsDeleted);

                if (order == null)
                {
                    return new ApiResponse<OrderDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                var newStatus = await _context.StatusOrders.FindAsync(request.StatusId);
                if (newStatus == null)
                {
                    return new ApiResponse<OrderDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Trạng thái không hợp lệ"
                    };
                }

                // Update order status
                order.StatusId = request.StatusId;
                order.UpdatedAt = DateTime.UtcNow;

                // Restore stock and refund wallet when admin cancels an order
                if (newStatus.StatusName == OrderStatusNames.Cancelled)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        if (detail.Product != null)
                        {
                            detail.Product.Quantity += detail.Quantity;
                        }
                    }

                    if (order.PaidByWalletAmount > 0)
                    {
                        await RefundToWalletAsync(order, order.AccountId, order.PaidByWalletAmount, "Hoàn tiền do admin hủy đơn");
                    }
                }

                // Create status history
                _context.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = request.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = adminId,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Send notification AFTER commit — prevents notification failure from rolling back the status update
                var orderCode = $"ORD{order.OrderId:D6}";
                await SendOrderStatusNotificationAsync(order, newStatus.StatusName, orderCode);

                // Auto-create GHN shipping order if requested and status is Processing or Confirmed
                CreateShippingOrderResponse? shippingResponse = null;
                string? shippingError = null;
                if (request.AutoCreateShipping &&
                    (newStatus.StatusName == "Processing" || newStatus.StatusName == "Confirmed"))
                {
                    _logger.LogInformation($"Auto-creating GHN shipping for order {orderId} (Status: {newStatus.StatusName})");

                    // Check if shipping already exists
                    var existingShipping = await _context.ShippingProviderTransactions
                        .FirstOrDefaultAsync(s => s.OrderId == orderId && s.Provider == "GHN");

                    if (existingShipping == null)
                    {
                        try
                        {
                            var shippingRequest = new CreateShippingOrderRequest
                            {
                                OrderId = orderId,
                                Provider = "GHN",
                                ServiceId = request.ShippingServiceId ?? 53321,
                                ServiceTypeId = 2, // Standard Express
                                Note = request.ShippingNote ?? "Đơn hàng từ LorKingdom",
                                RequiredNote = request.ShippingRequiredNote ?? "KHONGCHOXEMHANG"
                            };

                            var shippingResult = await CreateShippingOrderAsync(shippingRequest, adminId);

                            if (shippingResult.Status == 200)
                            {
                                shippingResponse = shippingResult.Data;
                                _logger.LogInformation($"✅ Auto-created GHN shipping: {shippingResponse?.OrderCode}");
                            }
                            else
                            {
                                shippingError = shippingResult.Message;
                                _logger.LogWarning($"⚠️ Failed to auto-create GHN shipping: {shippingError} (Status: {shippingResult.Status})");
                            }
                        }
                        catch (Exception shippingEx)
                        {
                            // Don't fail the whole operation if shipping creation fails
                            shippingError = shippingEx.Message;
                            _logger.LogError(shippingEx, "Error auto-creating GHN shipping order: {Error}", shippingError);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Shipping already exists for order {orderId}, skipping auto-create");
                    }
                }

                // Get updated order
                var updatedOrder = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstAsync(o => o.OrderId == orderId);

                var response = new ApiResponse<OrderDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Cập nhật trạng thái đơn hàng thành công",
                    Data = MapToDto(updatedOrder)
                };

                // Add shipping info to response if auto-created
                if (shippingResponse != null)
                {
                    response.Message += $" và đã tạo đơn GHN (Mã vận đơn: {shippingResponse.OrderCode})";
                }
                else if (request.AutoCreateShipping && shippingError != null)
                {
                    response.Message += $". ⚠️ Không tạo được đơn GHN: {shippingError}";
                }

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating order status");
                return new ApiResponse<OrderDto>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        #endregion

        #region Shipping Management

        public async Task<ApiResponse<CreateShippingOrderResponse>> CreateShippingOrderAsync(
            CreateShippingOrderRequest request, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get order
                var order = await _context.Orders
                    .Include(o => o.Status)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && !o.IsDeleted);

                if (order == null)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                // 2. Check if order already has shipping
                var existingShipping = await _context.ShippingProviderTransactions
                    .FirstOrDefaultAsync(s => s.OrderId == request.OrderId && s.Provider == request.Provider);

                if (existingShipping != null)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Đơn hàng đã có vận đơn {request.Provider}: {existingShipping.ProviderOrderCode}"
                    };
                }

                // 3. Get shop configuration
                var shopName = _configuration["ShopAddress:Name"] ?? "LorKingdom Shop";
                var shopPhone = _configuration["ShopAddress:Phone"] ?? "0987654321";
                var shopAddress = _configuration["ShopAddress:AddressLine"] ?? "123 Đường chính, Bình Thủy, Cần Thơ";
                var shopWardName = _configuration["ShopAddress:WardName"] ?? "Phường Bình Thủy";
                var shopWardCode = _configuration["ShopAddress:WardCode"] ?? "";
                var shopDistrictName = _configuration["ShopAddress:DistrictName"] ?? "Quận Bình Thủy";
                var shopDistrictId = int.Parse(_configuration["ShopAddress:DistrictId"] ?? "3695");
                var shopProvinceName = _configuration["ShopAddress:ProvinceName"] ?? "Cần Thơ";

                // 4. Prepare items
                var items = order.OrderDetails.Select(od => new BLL.DTOs.Shipping.GHNItem
                {
                    Name = od.Product?.ProductName ?? "Product",
                    Code = $"PRD{od.ProductId}",
                    Quantity = od.Quantity,
                    Price = (int)od.UnitPrice,
                    Weight = 500 // Default 500g per item
                }).ToArray();

                // 5. Validate shipping address completeness
                // Skip text-field check when GHN IDs are already stored (they take priority in step 6)
                if (string.IsNullOrEmpty(order.ShippingCity) && !order.ShippingDistrictId.HasValue)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Đơn hàng thiếu thông tin Tỉnh/Thành phố. Vui lòng cập nhật địa chỉ giao hàng đầy đủ."
                    };
                }

                if (string.IsNullOrEmpty(order.ShippingDistrict) && !order.ShippingDistrictId.HasValue)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Đơn hàng thiếu thông tin Quận/Huyện. Vui lòng cập nhật địa chỉ giao hàng đầy đủ."
                    };
                }

                // 6. Get district ID and ward code from shipping address
                int? toDistrictId = null;
                string? toWardCode = null;

                // PRIORITY 1: Use stored GHN IDs from order (fastest, most reliable)
                if (order.ShippingDistrictId.HasValue)
                {
                    toDistrictId = order.ShippingDistrictId;
                    toWardCode = order.ShippingWardCode;

                    _logger.LogInformation($"[CreateGHNShipping] ✅ Using stored GHN IDs: DistrictId={toDistrictId}, WardCode={toWardCode}");
                }
                // FALLBACK: Legacy orders without GHN IDs - use dynamic lookup
                else if (!string.IsNullOrEmpty(order.ShippingCity) && !string.IsNullOrEmpty(order.ShippingDistrict))
                {
                    _logger.LogWarning($"[CreateGHNShipping] ⚠️ No stored GHN IDs, attempting text-based lookup for '{order.ShippingDistrict}, {order.ShippingCity}'");

                    toDistrictId = await _ghnService.GetDistrictIdByNameAsync(
                        order.ShippingCity,
                        order.ShippingDistrict);

                    // Get ward code if district found and ward specified
                    if (toDistrictId.HasValue && !string.IsNullOrEmpty(order.ShippingWard))
                    {
                        toWardCode = await _ghnService.GetWardCodeByNameAsync(
                            toDistrictId.Value,
                            order.ShippingWard);

                        if (!string.IsNullOrEmpty(toWardCode))
                        {
                            _logger.LogInformation($"[CreateGHNShipping] ✅ Fallback lookup successful: DistrictId={toDistrictId}, WardCode={toWardCode}");
                        }
                    }
                }

                if (!toDistrictId.HasValue)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Không thể xác định quận/huyện giao hàng cho '{order.ShippingDistrict}, {order.ShippingCity}'. " +
                                  $"Vui lòng cập nhật lại địa chỉ giao hàng với thông tin GHN IDs đầy đủ."
                    };
                }

                // 7. Create GHN order request
                var ghnRequest = new BLL.DTOs.Shipping.GHNCreateOrderRequest
                {
                    PaymentTypeId = order.Status.StatusName == OrderStatusNames.Pending ? "2" : "1", // 2=COD, 1=Shop pays
                    Note = request.Note ?? $"Đơn hàng ORD{order.OrderId:D6}",
                    RequiredNote = request.RequiredNote,
                    // Sender/Pickup Information
                    FromName = shopName,
                    FromPhone = shopPhone,
                    FromAddress = shopAddress,
                    FromWardName = shopWardName,
                    FromDistrictName = shopDistrictName,
                    FromProvinceName = shopProvinceName,
                    FromDistrictId = shopDistrictId, // REQUIRED for GHN warehouse
                    // Return Information
                    ReturnPhone = shopPhone,
                    ReturnAddress = shopAddress,
                    ReturnDistrictId = shopDistrictId.ToString(),
                    ReturnWardCode = shopWardCode,
                    // Order Information
                    ClientOrderCode = $"ORD{order.OrderId:D6}",
                    // Recipient Information
                    ToName = order.ShippingName ?? "Customer",
                    ToPhone = order.ShippingPhone ?? "",
                    ToAddress = order.ShippingAddressLine ?? "",
                    ToWardCode = toWardCode ?? "", // Ward code from lookup
                    ToDistrictId = toDistrictId.Value,
                    ToWardName = order.ShippingWard ?? "", // REQUIRED by GHN docs
                    ToDistrictName = order.ShippingDistrict ?? "", // REQUIRED by GHN docs
                    ToProvinceName = order.ShippingCity ?? "", // REQUIRED by GHN docs
                    // Package Details
                    CodAmount = (int)order.TotalAmount,
                    Content = "Sản phẩm thú cưng",
                    Weight = items.Sum(i => i.Weight * i.Quantity),
                    Length = 30,
                    Width = 20,
                    Height = 10,
                    ServiceId = request.ServiceId,
                    ServiceTypeId = request.ServiceTypeId,
                    InsuranceValue = order.TotalAmount > 1000000 ? (int)order.TotalAmount : null, // Insurance if > 1M VND
                    Items = items
                };

                // 8. Call GHN API
                var ghnResponse = await _ghnService.CreateOrderAsync(ghnRequest);

                if (ghnResponse.Code != 200 || ghnResponse.Data == null)
                {
                    return new ApiResponse<CreateShippingOrderResponse>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = $"Tạo đơn GHN thất bại: {ghnResponse.Message}"
                    };
                }

                // 9. Save shipping transaction
                var shippingTransaction = new ShippingProviderTransaction
                {
                    OrderId = order.OrderId,
                    Provider = "GHN",
                    ProviderOrderCode = ghnResponse.Data.OrderCode,
                    TrackingNumber = ghnResponse.Data.OrderCode,
                    ServiceType = $"Service ID: {request.ServiceId}",
                    Status = "ready_to_pick",
                    ShippingFee = ghnResponse.Data.TotalFee,
                    EstimatedDelivery = DateTime.TryParse(ghnResponse.Data.ExpectedDeliveryTime, out var estDate)
                        ? estDate
                        : null,
                    Metadata = JsonSerializer.Serialize(ghnResponse.Data),
                    CreatedAt = DateTime.UtcNow
                };

                _context.ShippingProviderTransactions.Add(shippingTransaction);
                await _context.SaveChangesAsync();

                // 10. Update order status to "Processing" or "Shipped"
                var shippedStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Processing);
                if (shippedStatus != null)
                {
                    order.StatusId = shippedStatus.StatusId;
                    order.UpdatedAt = DateTime.UtcNow;

                    // Add status history
                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        StatusId = shippedStatus.StatusId,
                        ChangedAt = DateTime.UtcNow,
                        ChangedBy = adminId,
                        Note = $"Đã tạo vận đơn GHN: {ghnResponse.Data.OrderCode}",
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();

                    // Send notification
                    var orderCode = $"ORD{order.OrderId:D6}";
                    await SendOrderStatusNotificationAsync(order, shippedStatus.StatusName, orderCode);
                }

                await transaction.CommitAsync();

                return new ApiResponse<CreateShippingOrderResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Tạo đơn vận chuyển thành công",
                    Data = new CreateShippingOrderResponse
                    {
                        Success = true,
                        Message = "Đã tạo vận đơn GHN thành công",
                        OrderCode = ghnResponse.Data.OrderCode,
                        TrackingNumber = ghnResponse.Data.OrderCode,
                        Fee = ghnResponse.Data.TotalFee,
                        ExpectedDeliveryTime = ghnResponse.Data.ExpectedDeliveryTime,
                        Provider = "GHN"
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating shipping order for OrderId {OrderId}", request.OrderId);
                return new ApiResponse<CreateShippingOrderResponse>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> HandleShippingWebhookAsync(
            string provider, GHNWebhookRequest webhookData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Received shipping webhook from {Provider}: {OrderCode} - {Status}",
                    provider, webhookData.Data.OrderCode, webhookData.Data.Status);

                // 1. Find shipping transaction
                var shippingTransaction = await _context.ShippingProviderTransactions
                    .Include(s => s.Order)
                        .ThenInclude(o => o.Status)
                    .FirstOrDefaultAsync(s => s.Provider == provider &&
                                            s.ProviderOrderCode == webhookData.Data.OrderCode);

                if (shippingTransaction == null)
                {
                    _logger.LogWarning("Shipping transaction not found for OrderCode: {OrderCode}", webhookData.Data.OrderCode);
                    return new ApiResponse<object>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy vận đơn"
                    };
                }

                // 2. Update shipping transaction status
                shippingTransaction.Status = webhookData.Data.Status;
                shippingTransaction.UpdatedAt = DateTime.UtcNow;

                // Update metadata
                var metadata = string.IsNullOrEmpty(shippingTransaction.Metadata)
                    ? new Dictionary<string, object>()
                    : JsonSerializer.Deserialize<Dictionary<string, object>>(shippingTransaction.Metadata) ?? new Dictionary<string, object>();

                metadata[$"status_update_{DateTime.UtcNow:yyyyMMddHHmmss}"] = new
                {
                    status = webhookData.Data.Status,
                    statusText = webhookData.Data.StatusText,
                    reason = webhookData.Data.Reason,
                    time = webhookData.Data.Time
                };

                shippingTransaction.Metadata = JsonSerializer.Serialize(metadata);

                // 3. Update actual delivery time if delivered
                if (webhookData.Data.Status == "delivered")
                {
                    shippingTransaction.ActualDelivery = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // 4. Update order status based on shipping status
                var order = shippingTransaction.Order;
                StatusOrder? newStatus = null;

                switch (webhookData.Data.Status)
                {
                    case "ready_to_pick":
                    case "picking":
                    case "picked":
                        // Order is being prepared for shipping
                        newStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Processing);
                        break;

                    case "storing":
                    case "transporting":
                    case "delivering":
                        // Order is in transit
                        newStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Shipped);
                        break;

                    case "delivered":
                        // Order has been delivered
                        newStatus = await _orderRepo.GetStatusByNameAsync(OrderStatusNames.Delivered);
                        break;

                    case "return":
                    case "returned":
                    case "exception":
                        // Order has issues - keep current status or log
                        _logger.LogWarning("Order {OrderId} has shipping issue: {Status} - {Reason}",
                            order.OrderId, webhookData.Data.Status, webhookData.Data.Reason);
                        break;
                }

                // 5. Update order status if needed
                if (newStatus != null && order.StatusId != newStatus.StatusId)
                {
                    var oldStatusName = order.Status.StatusName;
                    order.StatusId = newStatus.StatusId;
                    order.UpdatedAt = DateTime.UtcNow;

                    // Add status history
                    _context.OrderStatusHistories.Add(new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        StatusId = newStatus.StatusId,
                        ChangedAt = DateTime.UtcNow,
                        ChangedBy = null, // System updated
                        Note = $"Tự động cập nhật từ {provider}: {webhookData.Data.StatusText ?? webhookData.Data.Status}",
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();

                    // Send notification
                    var orderCode = $"ORD{order.OrderId:D6}";
                    await SendOrderStatusNotificationAsync(order, newStatus.StatusName, orderCode);

                    _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus} via webhook",
                        order.OrderId, oldStatusName, newStatus.StatusName);
                }

                await transaction.CommitAsync();

                return new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Đã xử lý webhook thành công"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error handling shipping webhook from {Provider}", provider);
                return new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        #endregion

        #region Payment & Webhook

        public async Task<ApiResponse<object>> HandlePaymentWebhookAsync(
            string provider, string payload, string signature)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Store webhook event
                var webhookEvent = new WebhookEvent
                {
                    Provider = provider,
                    EventType = "payment.callback",
                    Payload = payload,
                    Signature = signature,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                _context.WebhookEvents.Add(webhookEvent);
                await _context.SaveChangesAsync();

                // Validate signature based on provider
                bool isValid = false;
                int orderId = 0;
                string? transactionId = null;
                bool isSuccess = false;

                if (provider.Equals(PaymentMethods.VNPay, StringComparison.OrdinalIgnoreCase))
                {
                    var payloadDict = JsonSerializer.Deserialize<Dictionary<string, string>>(payload);
                    if (payloadDict == null) throw new Exception("Invalid VNPay payload");

                    var vnpayCallback = _vnPayService.ParseCallback(payloadDict);
                    isValid = _vnPayService.ValidateCallback(vnpayCallback);

                    if (isValid)
                    {
                        orderId = int.Parse(vnpayCallback.vnp_TxnRef);
                        transactionId = vnpayCallback.vnp_TransactionNo;
                        isSuccess = vnpayCallback.vnp_ResponseCode == "00" && vnpayCallback.vnp_TransactionStatus == "00";
                    }
                }
                else if (provider.Equals(PaymentMethods.MoMo, StringComparison.OrdinalIgnoreCase))
                {
                    var momoCallback = JsonSerializer.Deserialize<MoMoCallbackRequest>(payload);
                    if (momoCallback == null) throw new Exception("Invalid MoMo payload");

                    isValid = _moMoService.ValidateCallback(momoCallback);

                    if (isValid)
                    {
                        orderId = int.Parse(momoCallback.orderId);
                        transactionId = momoCallback.transId;
                        isSuccess = momoCallback.resultCode == 0;
                    }
                }
                else if (provider.Equals(PaymentMethods.Sepay, StringComparison.OrdinalIgnoreCase))
                {
                    var payloadDict = JsonSerializer.Deserialize<Dictionary<string, string>>(payload);
                    if (payloadDict == null) throw new Exception("Invalid Sepay payload");

                    var sepayCallback = _sepayService.ParseCallback(payloadDict);
                    isValid = _sepayService.ValidateCallback(sepayCallback);

                    if (isValid)
                    {
                        orderId = int.Parse(sepayCallback.order_id);
                        transactionId = sepayCallback.transaction_id;
                        isSuccess = sepayCallback.status.Equals("success", StringComparison.OrdinalIgnoreCase);
                    }
                }
                else
                {
                    throw new Exception($"Unsupported payment provider: {provider}");
                }

                if (!isValid)
                {
                    webhookEvent.Status = "Invalid";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "INVALID_SIGNATURE",
                        Message = "Invalid webhook signature"
                    };
                }

                // Find payment gateway transaction
                var gatewayTxn = await _context.PaymentGatewayTransactions
                    .Include(g => g.PaymentHistory)
                    .ThenInclude(p => p.Order)
                    .FirstOrDefaultAsync(g => g.PaymentHistory.OrderId == orderId && g.Provider == provider);

                if (gatewayTxn == null)
                {
                    throw new Exception("Transaction not found");
                }

                // Update gateway transaction
                gatewayTxn.TransactionId = transactionId;
                gatewayTxn.Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
                gatewayTxn.GatewayResponse = payload;
                gatewayTxn.CompletedAt = DateTime.UtcNow;

                // Update payment history
                gatewayTxn.PaymentHistory.PaymentStatus = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
                gatewayTxn.PaymentHistory.TransactionCode = transactionId;

                if (isSuccess)
                {
                    // Update order
                    var order = gatewayTxn.PaymentHistory.Order;
                    order.PaymentCompletedAt = DateTime.UtcNow;

                    // Update to Processing status
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
                            Note = $"Thanh toán {provider} thành công",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                // Mark webhook as processed
                webhookEvent.Status = "Processed";
                webhookEvent.ProcessedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Send payment notification after commit (success or failure)
                if (gatewayTxn?.PaymentHistory?.Order != null)
                {
                    var order = gatewayTxn.PaymentHistory.Order;
                    var orderCode = $"ORD{order.OrderId:D6}";
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await SendPaymentNotificationAsync(
                                order.OrderId,
                                order.AccountId,
                                orderCode,
                                isSuccess,
                                provider,
                                gatewayTxn.Amount
                            );
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send payment notification for Order {OrderId}", order.OrderId);
                        }
                    });
                }

                return new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Webhook processed successfully"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing webhook from {Provider}", provider);
                return new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Error processing webhook: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> ConfirmCODPaymentAsync(int orderId, int shipperId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.PaymentHistories)
                    .Include(o => o.Status)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.IsDeleted);

                if (order == null)
                {
                    return new ApiResponse<object>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                if (order.Status.StatusName != OrderStatusNames.Delivered)
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Chỉ có thể xác nhận thanh toán COD khi đã giao hàng"
                    };
                }

                var codPayment = order.PaymentHistories
                    .FirstOrDefault(p => p.PaymentMethod == PaymentMethods.COD);

                if (codPayment == null)
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Đơn hàng không phải COD"
                    };
                }

                // Update payment
                codPayment.PaymentStatus = PaymentStatus.Success;
                codPayment.TransactionCode = $"COD_{orderId}_{DateTime.UtcNow.Ticks}";

                // Update order
                order.PaymentCompletedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                _context.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = orderId,
                    StatusId = order.StatusId,
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = shipperId,
                    Note = "Đã thu tiền COD",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Send payment success notification for COD
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SendPaymentNotificationAsync(
                            order.OrderId,
                            order.AccountId,
                            $"ORD{order.OrderId:D6}",
                            true,
                            PaymentMethods.COD,
                            codPayment.Amount
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send COD payment notification for Order {OrderId}", order.OrderId);
                    }
                });

                return new ApiResponse<object>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Xác nhận thanh toán COD thành công"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error confirming COD payment");
                return new ApiResponse<object>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        #endregion

        #region Refund

        public async Task<ApiResponse<RefundDto>> CreateRefundRequestAsync(
            CreateRefundRequest request, int accountId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Status)
                    .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.AccountId == accountId && !o.IsDeleted);

                if (order == null)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                if (!order.Status.StatusName.Equals(OrderStatusNames.Completed, StringComparison.OrdinalIgnoreCase))
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Chỉ có thể hoàn tiền đơn hàng đã hoàn thành"
                    };
                }

                // Check if a pending refund already exists for this order
                var existingRefund = await _context.OrderRefunds
                    .FirstOrDefaultAsync(r => r.OrderId == request.OrderId
                        && r.AccountId == accountId
                        && r.RefundStatus == RefundStatus.Requested);
                if (existingRefund != null)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Bạn đã có yêu cầu hoàn tiền đang chờ xử lý"
                    };
                }

                if (request.RefundAmount > order.TotalAmount)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Số tiền hoàn vượt quá giá trị đơn hàng"
                    };
                }

                var refund = new OrderRefund
                {
                    OrderId = request.OrderId,
                    AccountId = accountId,
                    RequestedBy = accountId,
                    RefundMode = request.RefundMode,
                    RefundStatus = RefundStatus.Requested,
                    TotalAmount = order.TotalAmount,
                    RefundAmount = request.RefundAmount,
                    Reason = request.Reason,
                    CreatedAt = DateTime.UtcNow
                };

                refund = await _orderRepo.CreateRefundAsync(refund);

                // Update the order's RefundStatus so the order list reflects it immediately
                order.RefundStatus = RefundStatus.Requested;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ApiResponse<RefundDto>
                {
                    Status = 201,
                    StatusMessage = "SUCCESS",
                    Message = "Tạo yêu cầu hoàn tiền thành công",
                    Data = new RefundDto
                    {
                        RefundId = refund.RefundId,
                        OrderId = refund.OrderId,
                        AccountId = refund.AccountId,
                        RefundMode = refund.RefundMode,
                        RefundStatus = refund.RefundStatus,
                        TotalAmount = refund.TotalAmount,
                        RefundAmount = refund.RefundAmount,
                        Reason = refund.Reason,
                        CreatedAt = refund.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating refund request");
                return new ApiResponse<RefundDto>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RefundDto>> ApproveRefundAsync(
            ApproveRefundRequest request, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var refund = await _orderRepo.GetRefundByIdAsync(request.RefundId);

                if (refund == null)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy yêu cầu hoàn tiền"
                    };
                }

                if (refund.RefundStatus != RefundStatus.Requested)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 400,
                        StatusMessage = "FAILED",
                        Message = "Yêu cầu hoàn tiền đã được xử lý"
                    };
                }

                if (!request.IsApproved)
                {
                    // Reject refund
                    refund.RefundStatus = RefundStatus.Rejected;
                    refund.ApprovedBy = adminId;
                    refund.ApprovedAt = DateTime.UtcNow;
                    refund.UpdatedAt = DateTime.UtcNow;

                    await _orderRepo.UpdateRefundAsync(refund);
                    await transaction.CommitAsync();

                    return new ApiResponse<RefundDto>
                    {
                        Status = 200,
                        StatusMessage = "SUCCESS",
                        Message = "Đã từ chối yêu cầu hoàn tiền",
                        Data = MapRefundToDto(refund)
                    };
                }
                // Approve and process refund
                if (refund.RefundMode == RefundModes.Wallet)
                {
                    await RefundToWalletAsync(refund.Order, refund.AccountId, refund.RefundAmount,
                        $"Hoàn tiền đơn hàng #{refund.OrderId}: {refund.Reason}");

                    // Link wallet transaction
                    var walletTxn = await _context.WalletTransactions
                        .Where(w => w.RelatedOrderId == refund.OrderId && w.TxnType == WalletTransactionTypes.Refund)
                        .OrderByDescending(w => w.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (walletTxn != null)
                    {
                        refund.WalletTransactionId = walletTxn.WalletTransactionId;
                    }
                }

                refund.RefundStatus = RefundStatus.Completed;
                refund.ApprovedBy = adminId;
                refund.ApprovedAt = DateTime.UtcNow;
                refund.ProcessedAt = DateTime.UtcNow;
                refund.UpdatedAt = DateTime.UtcNow;

                // Update order refund status
                refund.Order.RefundStatus = refund.RefundAmount >= refund.TotalAmount
                    ? RefundStatus.FullRefund
                    : RefundStatus.PartialRefund;
                refund.Order.UpdatedAt = DateTime.UtcNow;

                await _orderRepo.UpdateRefundAsync(refund);
                await _orderRepo.UpdateOrderAsync(refund.Order);
                await transaction.CommitAsync();

                return new ApiResponse<RefundDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Duyệt và xử lý hoàn tiền thành công",
                    Data = MapRefundToDto(refund)
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error approving refund");
                return new ApiResponse<RefundDto>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<PagedResult<RefundDto>>> GetRefundRequestsAsync(
            int pageNumber = 1, int pageSize = 10, string? statusFilter = null)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;
                var refunds = await _orderRepo.GetRefundRequestsAsync(skip, pageSize, statusFilter);
                var totalCount = await _orderRepo.GetRefundsCountAsync(statusFilter);

                var refundDtos = refunds.Select(MapRefundToDto).ToList();

                return new ApiResponse<PagedResult<RefundDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách yêu cầu hoàn tiền thành công",
                    Data = new PagedResult<RefundDto>
                    {
                        Items = refundDtos,
                        TotalCount = totalCount,
                        Page = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund requests");
                return new ApiResponse<PagedResult<RefundDto>>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<RefundDto>> GetRefundByIdAsync(long refundId)
        {
            try
            {
                var refund = await _orderRepo.GetRefundByIdAsync(refundId);

                if (refund == null)
                {
                    return new ApiResponse<RefundDto>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy yêu cầu hoàn tiền"
                    };
                }

                return new ApiResponse<RefundDto>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy thông tin hoàn tiền thành công",
                    Data = MapRefundToDto(refund)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refund {RefundId}", refundId);
                return new ApiResponse<RefundDto>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<PagedResult<RefundDto>>> GetMyRefundsAsync(int accountId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;

                var refunds = await _context.OrderRefunds
                    .Include(r => r.Order)
                    .ThenInclude(o => o.Status)
                    .Where(r => r.AccountId == accountId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                var total = await _context.OrderRefunds
                    .Where(r => r.AccountId == accountId)
                    .CountAsync();

                var dtos = refunds.Select(MapRefundToDto).ToList();

                return new ApiResponse<PagedResult<RefundDto>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách yêu cầu hoàn tiền thành công",
                    Data = new PagedResult<RefundDto>
                    {
                        Items = dtos,
                        TotalCount = total,
                        Page = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my refunds for account {AccountId}", accountId);
                return new ApiResponse<PagedResult<RefundDto>>
                {
                    Status = 500,
                    StatusMessage = "ERROR",
                    Message = "Có lỗi xảy ra: " + ex.Message
                };
            }
        }

        private async Task RefundToWalletAsync(DAL.Models.Order order, int accountId, decimal amount, string reason)
        {
            var wallet = await _walletRepo.GetByAccountIdWithLockAsync(accountId);

            // Auto-create wallet if user doesn’t have one yet
            if (wallet == null)
            {
                wallet = new DAL.Models.Wallet
                {
                    AccountId = accountId,
                    Currency = "VND",
                    Balance = 0,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                wallet = await _walletRepo.CreateWalletAsync(wallet);
            }

            // Add refund to wallet
            wallet.Balance += amount;
            wallet.LastTransactionAt = DateTime.UtcNow;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepo.UpdateWalletAsync(wallet);

            // Create wallet transaction
            var walletTxn = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                AccountId = accountId,
                TxnType = WalletTransactionTypes.Refund,
                Direction = WalletDirection.In,
                Amount = amount,
                BalanceBefore = wallet.Balance - amount,
                BalanceAfter = wallet.Balance,
                RelatedOrderId = order.OrderId,
                Status = "Completed",
                IdempotencyKey = $"{order.OrderId}_refund_{DateTime.UtcNow.Ticks}",
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };
            await _walletRepo.AddWalletTransactionAsync(walletTxn);
        }

        #endregion

        #region Mapping

        private OrderDto MapToDto(DAL.Models.Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderCode = $"ORD{order.OrderId:D6}",
                AccountId = order.AccountId,
                AccountName = order.Account?.AccountName,
                VoucherId = order.VoucherId,
                VoucherCode = order.Voucher?.VoucherCode,
                StatusId = order.Status?.StatusId ?? order.StatusId,
                StatusName = order.Status?.StatusName ?? "",
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingCity = order.ShippingCity,
                ShippingDistrict = order.ShippingDistrict,
                ShippingWard = order.ShippingWard,
                ShippingMethod = order.ShippingMethod,
                ShippingFee = order.ShippingFee,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaidByWalletAmount = order.PaidByWalletAmount,
                PaidByExternalAmount = order.PaidByExternalAmount,
                PaymentCompletedAt = order.PaymentCompletedAt,
                RefundStatus = order.RefundStatus,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.ProductName ?? "",
                    ProductImage = od.Product?.ProductImages?.FirstOrDefault()?.ImageUrl,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Total = od.Total ?? 0,
                    Reviewed = od.Reviewed
                }).ToList() ?? new List<OrderDetailDto>(),
                StatusHistory = order.OrderStatusHistories?.Select(osh => new OrderStatusHistoryDto
                {
                    OrderStatusHistoryId = osh.OrderStatusHistoryId,
                    StatusId = osh.StatusId,
                    StatusName = osh.Status?.StatusName,
                    ChangedAt = osh.ChangedAt,
                    ChangedBy = osh.ChangedBy,
                    ChangedByName = osh.ChangedByNavigation?.AccountName,
                    Note = osh.Note
                }).ToList() ?? new List<OrderStatusHistoryDto>(),
                PaymentInfo = order.PaymentHistories?.FirstOrDefault() != null
                    ? new PaymentInfoDto
                    {
                        PaymentMethod = order.PaymentHistories.First().PaymentMethod,
                        PaymentStatus = order.PaymentHistories.First().PaymentStatus,
                        TransactionCode = order.PaymentHistories.First().TransactionCode,
                        Amount = order.PaymentHistories.First().Amount,
                        CreatedAt = order.PaymentHistories.First().CreatedAt
                    }
                    : null,
                ShippingInfo = order.ShippingProviderTransactions?.FirstOrDefault() != null
                    ? new ShippingInfoDto
                    {
                        Provider = order.ShippingProviderTransactions.First().Provider,
                        TrackingNumber = order.ShippingProviderTransactions.First().TrackingNumber,
                        Status = order.ShippingProviderTransactions.First().Status,
                        EstimatedDelivery = order.ShippingProviderTransactions.First().EstimatedDelivery,
                        ActualDelivery = order.ShippingProviderTransactions.First().ActualDelivery
                    }
                    : null,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }

        private RefundDto MapRefundToDto(OrderRefund refund)
        {
            return new RefundDto
            {
                RefundId = refund.RefundId,
                OrderId = refund.OrderId,
                OrderCode = $"ORD{refund.OrderId:D6}",
                AccountId = refund.AccountId,
                CustomerName = refund.Account?.AccountName,
                CustomerEmail = refund.Account?.Email,
                RefundMode = refund.RefundMode,
                RefundStatus = refund.RefundStatus,
                TotalAmount = refund.TotalAmount,
                RefundAmount = refund.RefundAmount,
                Reason = refund.Reason ?? "",
                CreatedAt = refund.CreatedAt,
                ApprovedAt = refund.ApprovedAt,
                ProcessedAt = refund.ProcessedAt,
                ApprovedByName = refund.ApprovedByNavigation?.AccountName
            };
        }

        #endregion

        #region Admin Management

        public async Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersAsync(OrderQuery query)
        {
            try
            {
                var (items, totalCount) = await _orderRepo.GetPagedAsync(
                    query.Keyword,
                    query.StatusId,
                    query.FromDate,
                    query.ToDate,
                    query.Page,
                    query.PageSize,
                    query.SortBy,
                    query.SortDesc);

                var orderResponses = items.Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    OrderCode = $"ORD{o.OrderId:D6}",
                    CustomerName = o.ShippingName ?? o.Account?.AccountName ?? "Unknown",
                    CustomerPhone = o.ShippingPhone ?? o.Account?.PhoneNumber ?? "",
                    StatusId = o.StatusId,
                    StatusName = o.Status?.StatusName ?? "",
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = $"{o.ShippingAddressLine}, {o.ShippingWard}, {o.ShippingCity}",
                    OrderDate = o.OrderDate,
                    PaymentCompletedAt = o.PaymentCompletedAt,
                    RefundStatus = o.RefundStatus,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemResponse
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName ?? "",
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Total = od.Quantity * od.UnitPrice,
                        ImageUrl = od.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
                    }).ToList()
                }).ToList();

                var pagedResult = new PagedResult<OrderResponse>
                {
                    Items = orderResponses,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize
                };

                return new ApiResponse<PagedResult<OrderResponse>>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy danh sách đơn hàng thành công",
                    Data = pagedResult
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return new ApiResponse<PagedResult<OrderResponse>>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Có lỗi xảy ra khi lấy danh sách đơn hàng"
                };
            }
        }

        public async Task<ApiResponse<OrderDetailResponse>> GetOrderDetailAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
                if (order == null)
                {
                    return new ApiResponse<OrderDetailResponse>
                    {
                        Status = 404,
                        StatusMessage = "NOT_FOUND",
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                var response = new OrderDetailResponse
                {
                    OrderId = order.OrderId,
                    OrderCode = $"ORD{order.OrderId:D6}",
                    CustomerName = order.ShippingName ?? order.Account?.AccountName ?? "Unknown",
                    CustomerPhone = order.ShippingPhone ?? order.Account?.PhoneNumber ?? "",
                    StatusId = order.StatusId,
                    StatusName = order.Status?.StatusName ?? "",
                    TotalAmount = order.TotalAmount,
                    ShippingAddress = $"{order.ShippingAddressLine}, {order.ShippingWard}, {order.ShippingCity}",
                    OrderDate = order.OrderDate,
                    PaymentCompletedAt = order.PaymentCompletedAt,
                    RefundStatus = order.RefundStatus,
                    AccountId = order.AccountId,
                    AccountEmail = order.Account?.Email ?? "",
                    VoucherId = order.VoucherId,
                    VoucherCode = order.Voucher?.VoucherCode,
                    VoucherDiscount = order.Voucher?.DiscountValue,
                    ShippingMethod = order.ShippingMethod,
                    ShippingFee = order.ShippingFee,
                    PaidByWalletAmount = order.PaidByWalletAmount,
                    PaidByExternalAmount = order.PaidByExternalAmount,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemResponse
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product?.ProductName ?? "",
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        Total = od.Quantity * od.UnitPrice,
                        ImageUrl = od.Product?.ProductImages?.FirstOrDefault()?.ImageUrl
                    }).ToList(),
                    StatusHistories = order.OrderStatusHistories.Select(sh => new OrderStatusHistoryResponse
                    {
                        OrderStatusHistoryId = sh.OrderStatusHistoryId,
                        StatusId = sh.StatusId ?? 0,
                        StatusName = sh.Status?.StatusName ?? "",
                        ChangedAt = sh.ChangedAt,
                        ChangedBy = sh.ChangedBy,
                        ChangedByName = sh.ChangedByNavigation?.AccountName,
                        Note = sh.Note
                    }).ToList(),
                    ShippingInfo = order.ShippingProviderTransactions?.FirstOrDefault() != null
                        ? new ShippingInfoDto
                        {
                            Provider = order.ShippingProviderTransactions.First().Provider,
                            TrackingNumber = order.ShippingProviderTransactions.First().TrackingNumber,
                            Status = order.ShippingProviderTransactions.First().Status,
                            EstimatedDelivery = order.ShippingProviderTransactions.First().EstimatedDelivery,
                            ActualDelivery = order.ShippingProviderTransactions.First().ActualDelivery
                        }
                        : null
                };

                return new ApiResponse<OrderDetailResponse>
                {
                    Status = 200,
                    StatusMessage = "SUCCESS",
                    Message = "Lấy chi tiết đơn hàng thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order detail for OrderId: {OrderId}", orderId);
                return new ApiResponse<OrderDetailResponse>
                {
                    Status = 500,
                    StatusMessage = "FAILED",
                    Message = "Có lỗi xảy ra khi lấy chi tiết đơn hàng"
                };
            }
        }

        public async Task<byte[]> ExportOrdersToExcelAsync(OrderQuery query)
        {
            var orders = await _orderRepo.GetOrdersForExportAsync(
                query.Keyword,
                query.StatusId,
                query.FromDate,
                query.ToDate,
                query.SortBy,
                query.SortDesc,
                5000);

            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            // Headers
            worksheet.Cells[1, 1].Value = "Order ID";
            worksheet.Cells[1, 2].Value = "Order Code";
            worksheet.Cells[1, 3].Value = "Customer Name";
            worksheet.Cells[1, 4].Value = "Customer Phone";
            worksheet.Cells[1, 5].Value = "Email";
            worksheet.Cells[1, 6].Value = "Status";
            worksheet.Cells[1, 7].Value = "Total Amount";
            worksheet.Cells[1, 8].Value = "Shipping Fee";
            worksheet.Cells[1, 9].Value = "Shipping Address";
            worksheet.Cells[1, 10].Value = "Order Date";
            worksheet.Cells[1, 11].Value = "Payment Completed";

            // Style header
            using (var range = worksheet.Cells[1, 1, 1, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.OrderId;
                worksheet.Cells[row, 2].Value = $"ORD{order.OrderId:D6}";
                worksheet.Cells[row, 3].Value = order.ShippingName ?? order.Account?.AccountName ?? "Unknown";
                worksheet.Cells[row, 4].Value = order.ShippingPhone ?? "";
                worksheet.Cells[row, 5].Value = order.Account?.Email ?? "";
                worksheet.Cells[row, 6].Value = order.Status?.StatusName ?? "";
                worksheet.Cells[row, 7].Value = order.TotalAmount;
                worksheet.Cells[row, 8].Value = order.ShippingFee;
                worksheet.Cells[row, 9].Value = $"{order.ShippingAddressLine}, {order.ShippingWard}, {order.ShippingCity}";
                worksheet.Cells[row, 10].Value = order.OrderDate.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cells[row, 11].Value = order.PaymentCompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            return package.GetAsByteArray();
        }

        #endregion

        #region Helper Methods - Notifications

        /// <summary>
        /// Send order status notification to customer (system-generated)
        /// </summary>
        private async Task SendOrderStatusNotificationAsync(DAL.Models.Order order, string statusName, string orderCode)
        {
            var orderId = order.OrderId;
            var accountId = order.AccountId;
            try
            {
                string? templateCode = statusName switch
                {
                    OrderStatusNames.Processing => NotificationConstants.SystemOnlyTemplateCodes.OrderConfirmed,
                    OrderStatusNames.Confirmed => NotificationConstants.SystemOnlyTemplateCodes.OrderConfirmed,
                    OrderStatusNames.Shipped => NotificationConstants.SystemOnlyTemplateCodes.OrderShipped,
                    OrderStatusNames.Delivered => NotificationConstants.SystemOnlyTemplateCodes.OrderDelivered,
                    OrderStatusNames.Cancelled => NotificationConstants.SystemOnlyTemplateCodes.OrderCancelled,
                    _ => null // Don't send notification for other statuses (Pending, Refunded)
                };

                if (templateCode == null)
                    return;

                // Build parameters — ShippingName & TotalAmount are already on the entity, no extra query
                var parameters = new Dictionary<string, string>
                {
                    { "orderId",      orderId.ToString() },
                    { "OrderId",      orderId.ToString() },
                    { "orderCode",    orderCode },
                    { "status",       statusName },
                    { "customerName", order.ShippingName ?? string.Empty },
                    { "totalAmount",  order.TotalAmount.ToString("N0") },
                    { "TotalAmount",  order.TotalAmount.ToString("N0") },
                };

                // For shipped orders, load tracking info from ShippingProviderTransaction
                if (statusName == OrderStatusNames.Shipped)
                {
                    var shipping = await _context.ShippingProviderTransactions
                        .Where(s => s.OrderId == orderId)
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefaultAsync();

                    var trackingCode = shipping?.TrackingNumber ?? shipping?.ProviderOrderCode ?? string.Empty;
                    var shippingUnit = shipping?.Provider ?? string.Empty;

                    parameters["trackingCode"] = trackingCode;
                    parameters["TrackingNumber"] = trackingCode;
                    parameters["shippingUnit"] = shippingUnit;
                    parameters["ShippingUnit"] = shippingUnit;
                }

                var payload = JsonSerializer.Serialize(new
                {
                    type = "order",
                    orderId = orderId,
                    orderCode = orderCode,
                    status = statusName.ToLower(),
                    link = $"/orders/{orderId}"
                });

                await _notificationService.SendNotificationAsync(
                    new SendNotificationRequest
                    {
                        TemplateCode = templateCode,
                        TargetType = NotificationConstants.TargetTypes.User,
                        TargetUserIds = new List<int> { accountId },
                        Parameters = parameters,
                        Payload = payload
                    },
                    createdByAccountId: 0, // System account
                    isSystemGenerated: true // Bypass admin restrictions
                );

                _logger.LogInformation("Sent {TemplateCode} notification for Order {OrderId} to Account {AccountId}",
                    templateCode, orderId, accountId);
            }
            catch (Exception ex)
            {
                // Log but don't throw - notification failure shouldn't affect order processing
                _logger.LogError(ex, "Failed to send order status notification for Order {OrderId}", orderId);
            }
        }

        /// <summary>
        /// Send payment notification to customer (system-generated)
        /// </summary>
        private async Task SendPaymentNotificationAsync(int orderId, int accountId, string orderCode, bool isSuccess, string paymentMethod, decimal amount)
        {
            try
            {
                var templateCode = isSuccess
                    ? NotificationConstants.SystemOnlyTemplateCodes.PaymentSuccess
                    : NotificationConstants.SystemOnlyTemplateCodes.PaymentFailed;

                var payload = JsonSerializer.Serialize(new
                {
                    type = "payment",
                    orderId = orderId,
                    orderCode = orderCode,
                    status = isSuccess ? "success" : "failed",
                    paymentMethod = paymentMethod,
                    amount = amount,
                    link = $"/orders/{orderId}"
                });

                await _notificationService.SendNotificationAsync(
                    new SendNotificationRequest
                    {
                        TemplateCode = templateCode,
                        TargetType = NotificationConstants.TargetTypes.User,
                        TargetUserIds = new List<int> { accountId },
                        Parameters = new Dictionary<string, string>
                        {
                            { "orderCode", orderCode },
                            { "amount", amount.ToString("N0") },
                            { "paymentMethod", paymentMethod }
                        },
                        Payload = payload
                    },
                    createdByAccountId: 0, // System account
                    isSystemGenerated: true // Bypass admin restrictions
                );

                _logger.LogInformation("Sent {TemplateCode} notification for Order {OrderId} to Account {AccountId}",
                    templateCode, orderId, accountId);
            }
            catch (Exception ex)
            {
                // Log but don't throw - notification failure shouldn't affect payment processing
                _logger.LogError(ex, "Failed to send payment notification for Order {OrderId}", orderId);
            }
        }

        #endregion
    }
}
