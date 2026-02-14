namespace BLL.DTOs.Orders
{
    public static class PaymentMethods
    {
        public const string COD = "COD";
        public const string Wallet = "Wallet";
        public const string VNPay = "VNPay";
        public const string MoMo = "MoMo";
        public const string Sepay = "Sepay";
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
    }

    public static class OrderStatusNames
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";
    }

    public static class RefundStatus
    {
        public const string None = "None";
        public const string Requested = "Requested";
        public const string Approved = "Approved";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Rejected = "Rejected";
        public const string PartialRefund = "PartialRefund";
        public const string FullRefund = "FullRefund";
    }

    public static class RefundModes
    {
        public const string Wallet = "Wallet";
        public const string Original = "Original";
    }

    public static class WalletTransactionTypes
    {
        public const string Payment = "Payment";
        public const string Refund = "Refund";
        public const string TopUp = "TopUp";
    }

    public static class WalletDirection
    {
        public const string In = "In";
        public const string Out = "Out";
    }

    public static class ShippingMethods
    {
        public const string Express = "Express";
        public const string Standard = "Standard";
        public const string Economy = "Economy";
    }
}
