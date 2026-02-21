using BLL.DTOs.Orders;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Helpers.Order
{
    /// <summary>
    /// Helper for order validation logic
    /// </summary>
    public class OrderValidationHelper
    {
        private readonly AspLorKingDomContext _context;

        public OrderValidationHelper(AspLorKingDomContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Validate cart async and return cart
        /// </summary>
        public async Task<Cart> ValidateCartAsync(int accountId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

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
        /// Validate voucher and check conditions
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage, Voucher? Voucher)> ValidateVoucherAsync(int? voucherId, decimal subtotal)
        {
            if (!voucherId.HasValue)
            {
                return (true, null, null);
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherId == voucherId.Value && !v.IsDeleted);

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
        /// Validate address belongs to account
        /// </summary>
        public async Task<Address?> ValidateAndGetAddressAsync(int? addressId, int accountId)
        {
            if (!addressId.HasValue)
            {
                return null;
            }

            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId.Value
                    && a.AccountId == accountId
                    && !a.IsDeleted);
        }

        /// <summary>
        /// Validate order can be cancelled
        /// </summary>
        public (bool CanCancel, string? ErrorMessage) ValidateCancellation(DAL.Models.Order order, int accountId)
        {
            if (order.AccountId != accountId)
            {
                return (false, "Bạn không có quyền hủy đơn hàng này");
            }

            if (!OrderBusinessRules.CanCancel(order.Status.StatusName))
            {
                return (false, "Không thể hủy đơn hàng ở trạng thái hiện tại");
            }

            return (true, null);
        }

        /// <summary>
        /// Validate refund request
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateRefundRequest(DAL.Models.Order order, int accountId, decimal refundAmount)
        {
            if (order.AccountId != accountId)
            {
                return (false, "Bạn không có quyền yêu cầu hoàn tiền đơn hàng này");
            }

            if (!OrderBusinessRules.CanRefund(order.Status.StatusName))
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
