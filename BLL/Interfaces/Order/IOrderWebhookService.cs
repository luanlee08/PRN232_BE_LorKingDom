namespace BLL.Interfaces.Order
{
    /// <summary>
    /// Interface for payment webhook handling
    /// </summary>
    public interface IOrderWebhookService
    {
        /// <summary>
        /// Handle payment webhook from providers (VNPay, MoMo, Sepay)
        /// </summary>
        Task<WebhookResult> HandlePaymentWebhookAsync(
            string provider,
            Dictionary<string, string> webhookData);
    }

    /// <summary>
    /// Webhook processing result
    /// </summary>
    public class WebhookResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string>? ResponseData { get; set; }
    }
}
