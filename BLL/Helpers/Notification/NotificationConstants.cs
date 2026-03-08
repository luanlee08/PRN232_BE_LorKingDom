namespace BLL.Helpers.Notification
{
    /// <summary>
    /// Constants for Notification system
    /// </summary>
    public static class NotificationConstants
    {
        /// <summary>
        /// Template codes - Only CUSTOM is hardcoded as fallback.
        /// All other template codes are loaded from the Templates table in DB.
        /// </summary>
        public static class TemplateCodes
        {
            /// <summary>
            /// Fallback template code when sending custom notifications (no template)
            /// </summary>
            public const string Custom = "CUSTOM";
        }

        /// <summary>
        /// Template codes that Admin is allowed to manually create
        /// </summary>
        public static class AdminAllowedTemplateCodes
        {
            public const string SystemAnnouncement = "SYSTEM_ANNOUNCEMENT";
            public const string Promotion = "PROMOTION";
            public const string Voucher = "VOUCHER";
            public const string Coupon = "COUPON";
            public const string ContentAnnouncement = "CONTENT_ANNOUNCEMENT";
            public const string BlogAnnouncement = "BLOG_ANNOUNCEMENT";
            public const string ProductAnnouncement = "PRODUCT_ANNOUNCEMENT";
            public const string CustomerSupport = "CUSTOMER_SUPPORT";
            public const string Welcome = "WELCOME";
            public const string Custom = "CUSTOM";

            /// <summary>
            /// Get all admin-allowed template codes
            /// </summary>
            public static readonly HashSet<string> AllowedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                SystemAnnouncement,
                Promotion,
                Voucher,
                Coupon,
                ContentAnnouncement,
                BlogAnnouncement,
                ProductAnnouncement,
                CustomerSupport,
                Welcome,
                Custom
            };
        }

        /// <summary>
        /// Template codes that are SYSTEM-ONLY (cannot be created by Admin manually)
        /// These should only be triggered by business events
        /// </summary>
        public static class SystemOnlyTemplateCodes
        {
            // Order-related notifications
            public const string OrderCreated = "ORDER_CREATED";
            public const string OrderConfirmed = "ORDER_CONFIRMED";
            public const string OrderShipped = "ORDER_SHIPPED";
            public const string OrderDelivered = "ORDER_DELIVERED";
            public const string OrderCompleted = "ORDER_COMPLETED";
            public const string OrderCancelled = "ORDER_CANCELLED";

            // Payment-related notifications
            public const string PaymentSuccess = "PAYMENT_SUCCESS";
            public const string PaymentFailed = "PAYMENT_FAILED";
            public const string PaymentPending = "PAYMENT_PENDING";
            public const string RefundSuccess = "REFUND_SUCCESS";

            // Review-related (already automated)
            public const string ReviewRejected = "REVIEW_REJECTED";

            // Inventory-related
            public const string LowStock = "LOW_STOCK";

            /// <summary>
            /// Get all system-only template codes
            /// </summary>
            public static readonly HashSet<string> RestrictedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                OrderCreated,
                OrderConfirmed,
                OrderShipped,
                OrderDelivered,
                OrderCompleted,
                OrderCancelled,
                PaymentSuccess,
                PaymentFailed,
                PaymentPending,
                RefundSuccess,
                ReviewRejected,
                LowStock
            };
        }

        /// <summary>
        /// Target types for notifications
        /// </summary>
        public static class TargetTypes
        {
            public const string All = "All";
            public const string Role = "Role";
            public const string User = "User";
            public const string Condition = "Condition";
        }

        /// <summary>
        /// Delivery status
        /// </summary>
        public static class DeliveryStatus
        {
            public const string Unread = "Unread";
            public const string Read = "Read";
        }

        /// <summary>
        /// Background job status
        /// </summary>
        public static class JobStatus
        {
            public const string Pending = "PENDING";
            public const string Success = "SUCCESS";
            public const string Failed = "FAILED";
        }
    }
}
