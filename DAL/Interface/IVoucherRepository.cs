using DAL.Models;

namespace DAL.Interface
{
    public interface IVoucherRepository
    {
        Task<(List<Voucher> Items, int TotalCount)> GetVouchersAsync(
            string? voucherCode,
            int? voucherTypeId,
            string? status,
            int pageNumber,
            int pageSize);

        Task<Voucher?> GetVoucherByIdAsync(int voucherId);
        Task<Voucher?> GetVoucherByCodeAsync(string voucherCode);
        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<Voucher> UpdateVoucherAsync(Voucher voucher);
        Task<bool> VoucherCodeExistsAsync(string voucherCode, int? excludeVoucherId = null);
        //Task<List<VoucherType>> GetVoucherTypesAsync();
        //Task<VoucherType?> GetVoucherTypeByIdAsync(int voucherTypeId);
    }
}
