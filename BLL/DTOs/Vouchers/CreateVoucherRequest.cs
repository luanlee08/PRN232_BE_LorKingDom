namespace BLL.DTOs.Vouchers
{
    public class CreateVoucherRequest
    {
        public int VoucherTypeId { get; set; }
        public int? CreateBy { get; set; }
        public string VoucherCode { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public bool IsStackable { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
