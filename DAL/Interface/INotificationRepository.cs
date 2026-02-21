using DAL.Models;

namespace DAL.Interface
{
    public interface INotificationRepository
    {
        // Query deliveries (sent notifications)
        Task<(List<Delivery> Items, int TotalCount)> GetDeliveriesAsync(
            int? accountId,
            string? templateCode,
            string? status,
            string? keyword,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize);

        Task<Delivery?> GetDeliveryByIdAsync(long id);
        Task<(List<Delivery> Items, int TotalCount)> GetUserDeliveriesAsync(
            int accountId, string? status, string? templateCode, string? keyword,
            DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        Task<int> GetUnreadCountAsync(int accountId);

        // Create deliveries
        Task<Delivery> CreateDeliveryAsync(Delivery entity);
        Task CreateDeliveriesAsync(List<Delivery> entities);

        // Update delivery status
        Task MarkAsReadAsync(long deliveryId, int accountId);
        Task MarkAllAsReadAsync(int accountId);
        Task DeleteDeliveryAsync(long deliveryId);

        // Stats
        Task<DeliveryStatsDto> GetStatsAsync();
        Task SaveChangesAsync();
    }

    public class DeliveryStatsDto
    {
        public int TotalDeliveries { get; set; }
        public int UnreadDeliveries { get; set; }
        public int ReadDeliveries { get; set; }
        public int TodayDeliveries { get; set; }
        public Dictionary<string, int> DeliveriesByTemplate { get; set; } = new();
    }
}
