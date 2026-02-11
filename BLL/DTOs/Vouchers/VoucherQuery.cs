namespace BLL.DTOs.Vouchers
{
    public class VoucherQuery
    {
        public string? VoucherCode { get; set; }
        public int? VoucherTypeId { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
