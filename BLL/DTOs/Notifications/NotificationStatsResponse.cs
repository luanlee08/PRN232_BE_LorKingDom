namespace BLL.DTOs.Notifications
{
    /// <summary>
    /// Statistics about delivered notifications
    /// </summary>
    public class DeliveryStatsResponse
    {
        public int TotalDeliveries { get; set; }
        public int UnreadDeliveries { get; set; }
        public int ReadDeliveries { get; set; }
        public int TodayDeliveries { get; set; }
        public Dictionary<string, int> DeliveriesByTemplate { get; set; } = new();
    }
}
