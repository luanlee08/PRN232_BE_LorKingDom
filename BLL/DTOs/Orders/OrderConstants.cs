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
        public const string Confirmed = "Confirmed"; // StatusId 2 in database
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Completed = "Completed"; // StatusId 5 in database – fully finished
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

    /// <summary>
    /// Payment gateway limits and fees
    /// </summary>
    public static class PaymentLimits
    {
        public static class COD
        {
            public const decimal MinAmount = 0;
            public const decimal MaxAmount = 50_000_000m; // 50 triệu
        }

        public static class Wallet
        {
            public const decimal MinAmount = 0;
            public const decimal MaxAmount = decimal.MaxValue;
        }

        public static class VNPay
        {
            public const decimal MinAmount = 10_000m;
            public const decimal MaxAmount = 200_000_000m; // 200 triệu
            public const decimal TransactionFeePercent = 1.5m;
        }

        public static class MoMo
        {
            public const decimal MinAmount = 10_000m;
            public const decimal MaxAmount = 50_000_000m;
            public const decimal TransactionFeePercent = 1.0m;
        }

        public static class Sepay
        {
            public const decimal MinAmount = 10_000m;
            public const decimal MaxAmount = 500_000_000m;
            public const decimal TransactionFeePercent = 0m;
        }
    }

    /// <summary>
    /// Shipping fees by method
    /// </summary>
    public static class ShippingFees
    {
        public const decimal Express = 50_000m;
        public const decimal Standard = 30_000m;
        public const decimal Economy = 20_000m;
        public const decimal Default = 30_000m;

        public static decimal GetFee(string method) => method switch
        {
            ShippingMethods.Express => Express,
            ShippingMethods.Standard => Standard,
            ShippingMethods.Economy => Economy,
            _ => Default
        };
    }

    /// <summary>
    /// Order business rules
    /// </summary>
    public static class OrderBusinessRules
    {
        public static bool CanCancel(string statusName) =>
            statusName == OrderStatusNames.Pending ||
            statusName == OrderStatusNames.Processing;

        public static bool CanRefund(string statusName) =>
            statusName == OrderStatusNames.Delivered;

        public static bool CanConfirmCOD(string statusName) =>
            statusName == OrderStatusNames.Delivered;
    }
}
