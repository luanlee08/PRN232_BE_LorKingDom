using BLL.DTOs.Orders;
using BLL.Domain;
using DAL.Interface;
using DAL.Models;

namespace BLL.Helpers.Order
{
    /// <summary>
    /// Helper for order validation logic.
    /// Depends only on repository interfaces — no DbContext reference.
    /// </summary>
    public class OrderValidationHelper
    {
        private readonly ICartRepository _cartRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly IAddressRepositories _addressRepo;

        public OrderValidationHelper(
            ICartRepository cartRepo,
            IVoucherRepository voucherRepo,
            IAddressRepositories addressRepo)
        {
            _cartRepo = cartRepo;
            _voucherRepo = voucherRepo;
            _addressRepo = addressRepo;
        }

        /// <summary>
        /// Validate cart and return it (with CartItems + Products included via repository).
        /// </summary>
        public async Task<Cart> ValidateCartAsync(int accountId)
        {
            var cart = await _cartRepo.GetCartByAccountIdAsync(accountId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Giỏ hàng trống");
            }

            return cart;
        }

        /// <summary>
        /// Validate single product stock async
        /// </summary>
        public Task ValidateProductStockAsync(Product product, int requestedQuantity)
        {
            if (product == null || product.IsDeleted)
            {
                throw new InvalidOperationException("Sản phẩm không tồn tại");
            }

            if (product.Quantity < requestedQuantity)
            {
                throw new InvalidOperationException($"Sản phẩm '{product.ProductName}' không đủ số lượng (còn {product.Quantity})");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Validate cart has items and products exist
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateCart(Cart? cart)
        {
            if (cart == null || !cart.CartItems.Any())
            {
                return (false, "Giỏ hàng trống");
            }
            return (true, null);
        }

        /// <summary>
        /// Validate product stock availability
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateProductStock(IEnumerable<CartItem> items)
        {
            foreach (var item in items)
            {
                if (item.Product == null || item.Product.IsDeleted)
                {
                    return (false, $"Sản phẩm không tồn tại");
                }

                if (item.Product.Quantity < item.Quantity)
                {
                    return (false, $"Sản phẩm '{item.Product.ProductName}' không đủ số lượng (còn {item.Product.Quantity})");
                }
            }
            return (true, null);
        }

        /// <summary>
        /// Validate voucher and check conditions using IVoucherRepository.
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage, Voucher? Voucher)> ValidateVoucherAsync(int? voucherId, decimal subtotal)
        {
            if (!voucherId.HasValue)
            {
                return (true, null, null);
            }

            var voucher = await _voucherRepo.GetVoucherByIdAsync(voucherId.Value);

            if (voucher == null || voucher.Status != "Active")
            {
                return (false, "Voucher không hợp lệ", null);
            }

            if (DateTime.Now < voucher.StartDate || DateTime.Now > voucher.EndDate)
            {
                return (false, "Voucher đã hết hạn", null);
            }

            if (voucher.MinOrderAmount.HasValue && subtotal < voucher.MinOrderAmount)
            {
                return (false, $"Đơn hàng tối thiểu {voucher.MinOrderAmount:N0} VND để áp dụng voucher này", null);
            }

            return (true, null, voucher);
        }

        /// <summary>
        /// Validate address belongs to account using IAddressRepositories.
        /// </summary>
        public async Task<Address?> ValidateAndGetAddressAsync(int? addressId, int accountId)
        {
            if (!addressId.HasValue)
            {
                return null;
            }

            var address = await _addressRepo.GetByIdAsync(addressId.Value);
            // Ownership check in application layer (domain repository doesn't filter by accountId)
            return address?.AccountId == accountId && address.IsDeleted != true ? address : null;
        }

        /// <summary>
        /// Validate order can be cancelled by the given account.
        /// Uses domain state machine to enforce customer-cancellable statuses.
        /// </summary>
        public (bool CanCancel, string? ErrorMessage) ValidateCancellation(DAL.Models.Order order, int accountId)
        {
            if (order.AccountId != accountId)
            {
                return (false, "Bạn không có quyền hủy đơn hàng này");
            }

            if (!OrderStatusTransitions.IsCustomerCancellable(order.Status.StatusName))
            {
                return (false, $"Không thể hủy đơn hàng ở trạng thái '{order.Status.StatusName}'");
            }

            return (true, null);
        }

        /// <summary>
        /// Validate refund request.
        /// Refund is only valid when order has been Delivered (can transition to Refunded).
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateRefundRequest(DAL.Models.Order order, int accountId, decimal refundAmount)
        {
            if (order.AccountId != accountId)
            {
                return (false, "Bạn không có quyền yêu cầu hoàn tiền đơn hàng này");
            }

            // Only Delivered orders can be refunded (Delivered → Refunded is a valid transition)
            if (!OrderStatusTransitions.CanTransition(order.Status.StatusName, OrderStatusNames.Refunded))
            {
                return (false, "Chỉ có thể hoàn tiền đơn hàng đã giao");
            }

            if (refundAmount > order.TotalAmount)
            {
                return (false, "Số tiền hoàn vượt quá giá trị đơn hàng");
            }

            return (true, null);
        }
    }
}
