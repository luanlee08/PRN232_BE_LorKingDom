using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AspLorKingDomContext _context;

        public VoucherRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Voucher> Items, int TotalCount)> GetVouchersAsync(
            string? voucherCode,
            int? voucherTypeId,
            string? status,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Vouchers
                .Include(v => v.VoucherType)
                .Include(v => v.CreateByNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(voucherCode))
                query = query.Where(v => v.VoucherCode.Contains(voucherCode));

            if (voucherTypeId.HasValue)
                query = query.Where(v => v.VoucherTypeId == voucherTypeId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(v => v.Status == status);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Voucher?> GetVoucherByIdAsync(int voucherId)
        {
            return await _context.Vouchers
                .Include(v => v.VoucherType)
                .Include(v => v.CreateByNavigation)
                .FirstOrDefaultAsync(v => v.VoucherId == voucherId);
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string voucherCode)
        {
            return await _context.Vouchers
                .Include(v => v.VoucherType)
                .Include(v => v.CreateByNavigation)
                .FirstOrDefaultAsync(v => v.VoucherCode == voucherCode);
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            //Reload with navigation properties
            return await GetVoucherByIdAsync(voucher.VoucherId) ?? voucher;
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
            //Reload with navigation properties
            return await GetVoucherByIdAsync(voucher.VoucherId) ?? voucher;
        }

        public async Task<bool> VoucherCodeExistsAsync(string voucherCode, int? excludeVoucherId = null)
        {
            var query = _context.Vouchers.Where(v => v.VoucherCode == voucherCode);

            if (excludeVoucherId.HasValue)
                query = query.Where(v => v.VoucherId != excludeVoucherId.Value);

            return await query.AnyAsync();
        }

        //public async Task<List<VoucherType>> GetVoucherTypesAsync()
        //{
        //    return await _context.VoucherTypes
        //        .OrderBy(vt => vt.VoucherTypeId)
        //        .ToListAsync();
        //}

        //public async Task<VoucherType?> GetVoucherTypeByIdAsync(int voucherTypeId)
        //{
        //    return await _context.VoucherTypes
        //        .FirstOrDefaultAsync(vt => vt.VoucherTypeId == voucherTypeId);
        //}
    }
}
