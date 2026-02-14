namespace BLL.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public int? VoucherId { get; set; }
        public string PaymentMethod { get; set; } = null!; // "COD", "Wallet", "VNPay", "MoMo"

        // Shipping Information
        public int? AddressId { get; set; } // Optional: Use saved address
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddressLine { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingWard { get; set; }
        public string ShippingMethod { get; set; } = "Standard"; // "Express", "Standard", "Economy"

        // Payment Split (for hybrid payment)
        public decimal PaidByWalletAmount { get; set; } = 0;
        public decimal PaidByExternalAmount { get; set; } = 0;

        public string? Note { get; set; }
        public string? IdempotencyKey { get; set; } // Prevent duplicate orders
    }

    public class CreateRefundRequest
    {
        public int OrderId { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundMode { get; set; } = "Wallet"; // "Wallet" or "Original"
        public string Reason { get; set; } = null!;
    }

    public class ApproveRefundRequest
    {
        public long RefundId { get; set; }
        public bool IsApproved { get; set; }
        public string? Note { get; set; }
    }
}
