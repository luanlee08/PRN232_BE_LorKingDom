namespace BLL.DTOs.Orders
{
    /// <summary>
    /// Payment method information (alias for PaymentMethodDTO)
    /// </summary>
    public class PaymentMethodInfo : PaymentMethodDTO
    {
    }

    /// <summary>
    /// Payment processing result
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Payment callback result from gateway
    /// </summary>
    public class PaymentCallbackResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string? TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }
}
