namespace BLL.DTOs.Orders
{
    public class PaymentMethodDTO
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal TransactionFee { get; set; }
        public string TransactionFeeType { get; set; } = "Percentage"; // "Percentage" or "Fixed"
    }

    public class GetPaymentMethodsResponse
    {
        public List<PaymentMethodDTO> PaymentMethods { get; set; } = new();
    }
}
