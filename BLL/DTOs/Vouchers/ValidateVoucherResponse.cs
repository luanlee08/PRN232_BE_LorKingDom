namespace BLL.DTOs.Vouchers
{
    public class ValidateVoucherResponse
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; } = null!;
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
    }
}
