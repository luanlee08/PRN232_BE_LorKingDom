namespace BLL.DTOs.Orders
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public int? VoucherId { get; set; }
        public string? VoucherCode { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        // Shipping Information
        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddressLine { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingDistrict { get; set; }
        public string? ShippingWard { get; set; }
        public string? ShippingMethod { get; set; }
        public decimal ShippingFee { get; set; }

        // Order Details
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidByWalletAmount { get; set; }
        public decimal PaidByExternalAmount { get; set; }
        public DateTime? PaymentCompletedAt { get; set; }
        public string RefundStatus { get; set; } = null!;

        public List<OrderDetailDto> OrderDetails { get; set; } = new();
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
        public PaymentInfoDto? PaymentInfo { get; set; }
        public ShippingInfoDto? ShippingInfo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public bool Reviewed { get; set; }
    }

    public class OrderStatusHistoryDto
    {
        public int OrderStatusHistoryId { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime ChangedAt { get; set; }
        public int? ChangedBy { get; set; }
        public string? ChangedByName { get; set; }
        public string? Note { get; set; }
    }

    public class PaymentInfoDto
    {
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string? TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ShippingInfoDto
    {
        public string Provider { get; set; } = null!;
        public string? TrackingNumber { get; set; }
        public string? Status { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; }
    }

    public class RefundDto
    {
        public long RefundId { get; set; }
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public string RefundMode { get; set; } = null!;
        public string RefundStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ApprovedByName { get; set; }
    }

    // Alias for RefundDto
    public class OrderRefundDto : RefundDto
    {
    }

    public class CreateOrderResponse
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? PaymentUrl { get; set; } // For external payment redirect
        public decimal TotalAmount { get; set; }
        public string Message { get; set; } = null!;
    }
}
