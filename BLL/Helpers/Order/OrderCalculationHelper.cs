using BLL.DTOs.Orders;
using DAL.Models;

namespace BLL.Helpers.Order
{
    /// <summary>
    /// Helper for order calculations (subtotal, discount, shipping, total)
    /// </summary>
    public class OrderCalculationHelper
    {
        /// <summary>
        /// Calculate subtotal from cart items
        /// </summary>
        public decimal CalculateSubtotal(IEnumerable<CartItem> items)
        {
            return items.Sum(item => item.Product!.Price * item.Quantity);
        }

        /// <summary>
        /// Calculate discount based on voucher type
        /// </summary>
        public decimal CalculateDiscount(Voucher voucher, VoucherType? voucherType, decimal subtotal)
        {
            if (voucherType?.VoucherTypeName == "Percentage")
            {
                return subtotal * voucher.DiscountValue / 100;
            }
            else if (voucherType?.VoucherTypeName == "Fixed")
            {
                return voucher.DiscountValue;
            }
            return 0;
        }

        /// <summary>
        /// Get shipping fee by method
        /// </summary>
        public decimal GetShippingFee(string shippingMethod)
        {
            return ShippingFees.GetFee(shippingMethod);
        }

        /// <summary>
        /// Calculate final total amount
        /// </summary>
        public decimal CalculateTotalAmount(decimal subtotal, decimal discount, decimal shippingFee)
        {
            return subtotal - discount + shippingFee;
        }
    }
}
