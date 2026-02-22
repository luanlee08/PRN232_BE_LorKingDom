namespace BLL.DTOs.Orders
{
    public class CancelOrderRequest
    {
        public int? CancelledBy { get; set; } // AccountId of who cancelled
        public string? Reason { get; set; }
    }
}