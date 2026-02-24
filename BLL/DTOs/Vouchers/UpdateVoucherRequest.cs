namespace BLL.DTOs.Vouchers
{
    public class UpdateVoucherRequest
    {
        public int? VoucherTypeId { get; set; }
        public string? VoucherCode { get; set; }
        public string? DiscountType { get; set; } // "Fixed" | "Percentage"
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public bool? IsStackable { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
