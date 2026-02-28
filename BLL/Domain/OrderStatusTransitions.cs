using BLL.DTOs.Orders;

namespace BLL.Domain
{
    /// <summary>
    /// Encodes all valid order status transitions as a state machine.
    ///
    /// WHY this belongs in Domain (not in helpers or validators):
    ///   - It represents a business rule — "Delivered orders cannot be cancelled"
    ///   - It must be enforced regardless of who calls the service (admin, customer, webhook)
    ///   - Centralising it here prevents the same rule from being copied across helpers
    ///
    /// Usage:
    ///   OrderStatusTransitions.ThrowIfInvalid(currentStatus, requestedStatus);
    /// </summary>
    public static class OrderStatusTransitions
    {
        /// <summary>
        /// Defines which statuses are reachable from a given status.
        /// Any transition not listed here is explicitly forbidden.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> _allowed =
            new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                [OrderStatusNames.Pending] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    OrderStatusNames.Processing,
                    OrderStatusNames.Cancelled
                },
                [OrderStatusNames.Processing] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    OrderStatusNames.Confirmed,
                    OrderStatusNames.Cancelled
                },
                [OrderStatusNames.Confirmed] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    OrderStatusNames.Shipped,
                    OrderStatusNames.Cancelled
                },
                [OrderStatusNames.Shipped] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    OrderStatusNames.Delivered
                },
                [OrderStatusNames.Delivered] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    OrderStatusNames.Refunded
                },
                [OrderStatusNames.Cancelled] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                [OrderStatusNames.Refunded] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };

        /// <summary>Returns true if the transition from → to is permitted.</summary>
        public static bool CanTransition(string from, string to)
        {
            return _allowed.TryGetValue(from, out var targets) && targets.Contains(to);
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> when the transition is not allowed.
        /// Call this before persisting any status change.
        /// </summary>
        public static void ThrowIfInvalid(string from, string to)
        {
            if (!CanTransition(from, to))
            {
                throw new InvalidOperationException(
                    $"Không thể chuyển trạng thái đơn hàng từ '{from}' sang '{to}'. " +
                    $"Các trạng thái hợp lệ từ '{from}': [{GetAllowedTargets(from)}].");
            }
        }

        /// <summary>Returns the set of statuses reachable from a given status.</summary>
        public static IReadOnlySet<string> GetAllowedTransitions(string from)
        {
            return _allowed.TryGetValue(from, out var targets)
                ? targets
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns true if the order in this status can be cancelled by the customer.
        /// Only Pending and Processing orders are cancellable by the customer.
        /// </summary>
        public static bool IsCustomerCancellable(string status) =>
            status.Equals(OrderStatusNames.Pending, StringComparison.OrdinalIgnoreCase) ||
            status.Equals(OrderStatusNames.Processing, StringComparison.OrdinalIgnoreCase);

        private static string GetAllowedTargets(string from)
            => _allowed.TryGetValue(from, out var t) && t.Any()
                ? string.Join(", ", t)
                : "không có (trạng thái cuối cùng)";
    }
}
