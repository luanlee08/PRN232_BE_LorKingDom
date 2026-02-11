using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AspLorKingDomContext _context;

        public NotificationRepository(AspLorKingDomContext context)
        {
            _context = context;
        }

        public async Task<(List<Delivery>, int)> GetDeliveriesAsync(
            int? accountId,
            string? templateCode,
            string? status,
            string? keyword,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            var query = _context.Deliveries
                .Include(d => d.Account)
                .Include(d => d.TemplateCodeNavigation)
                .Include(d => d.CreatedByJob)
                .AsQueryable();

            if (accountId.HasValue)
            {
                query = query.Where(d => d.AccountId == accountId.Value);
            }

            if (!string.IsNullOrWhiteSpace(templateCode))
            {
                query = query.Where(d => d.TemplateCode == templateCode);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d =>
                    d.Title.Contains(keyword) ||
                    d.Message.Contains(keyword));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= toDate.Value);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Delivery?> GetDeliveryByIdAsync(long id)
        {
            return await _context.Deliveries
                .Include(d => d.Account)
                .Include(d => d.TemplateCodeNavigation)
                .Include(d => d.CreatedByJob)
                .FirstOrDefaultAsync(d => d.DeliveryId == id);
        }

        public async Task<List<Delivery>> GetUserDeliveriesAsync(int accountId, string? status, int limit)
        {
            var query = _context.Deliveries
                .Where(d => d.AccountId == accountId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            return await query
                .OrderByDescending(d => d.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int accountId)
        {
            return await _context.Deliveries
                .CountAsync(d => d.AccountId == accountId && d.Status == "Unread");
        }

        public async Task<Delivery> CreateDeliveryAsync(Delivery entity)
        {
            await _context.Deliveries.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task CreateDeliveriesAsync(List<Delivery> entities)
        {
            await _context.Deliveries.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(long deliveryId, int accountId)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.DeliveryId == deliveryId && d.AccountId == accountId);

            if (delivery != null && delivery.Status == "Unread")
            {
                delivery.Status = "Read";
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int accountId)
        {
            var unreadDeliveries = await _context.Deliveries
                .Where(d => d.AccountId == accountId && d.Status == "Unread")
                .ToListAsync();

            foreach (var delivery in unreadDeliveries)
            {
                delivery.Status = "Read";
            }

            if (unreadDeliveries.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteDeliveryAsync(long deliveryId)
        {
            var delivery = await _context.Deliveries
                .FirstOrDefaultAsync(d => d.DeliveryId == deliveryId);

            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DeliveryStatsDto> GetStatsAsync()
        {
            var today = DateTime.Today;

            var totalDeliveries = await _context.Deliveries.CountAsync();
            var unreadDeliveries = await _context.Deliveries
                .CountAsync(d => d.Status == "Unread");
            var readDeliveries = await _context.Deliveries
                .CountAsync(d => d.Status == "Read");
            var todayDeliveries = await _context.Deliveries
                .CountAsync(d => d.CreatedAt >= today);

            var deliveriesByTemplate = await _context.Deliveries
                .GroupBy(d => d.TemplateCode)
                .Select(g => new { TemplateCode = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TemplateCode, x => x.Count);

            return new DeliveryStatsDto
            {
                TotalDeliveries = totalDeliveries,
                UnreadDeliveries = unreadDeliveries,
                ReadDeliveries = readDeliveries,
                TodayDeliveries = todayDeliveries,
                DeliveriesByTemplate = deliveriesByTemplate
            };
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
