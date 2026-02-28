using BLL.DTOs.Orders;
using DAL.Models;

namespace BLL.Helpers.Order
{
    /// <summary>
    /// Helper for mapping Order entities to DTOs
    /// </summary>
    public class OrderMappingHelper
    {
        public OrderDto MapToDto(DAL.Models.Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                AccountId = order.AccountId,
                AccountName = order.Account?.AccountName,
                VoucherId = order.VoucherId,
                VoucherCode = order.Voucher?.VoucherCode,
                ShippingName = order.ShippingName,
                ShippingPhone = order.ShippingPhone,
                ShippingAddressLine = order.ShippingAddressLine,
                ShippingCity = order.ShippingCity,
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

        public OrderRefundDto MapRefundToDto(OrderRefund refund)
        {
            return new OrderRefundDto
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
    }
}
