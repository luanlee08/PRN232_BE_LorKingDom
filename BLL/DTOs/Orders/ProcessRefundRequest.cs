namespace BLL.DTOs.Orders
{
    /// <summary>
    /// Request to process (approve/reject) a refund
    /// </summary>
    public class ProcessRefundRequest
    {
        public bool IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public string? Note { get; set; }
    }
}
