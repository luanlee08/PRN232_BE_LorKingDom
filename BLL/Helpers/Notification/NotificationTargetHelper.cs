using BLL.DTOs.Notifications;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Helpers.Notification
{
    /// <summary>
    /// Helper for resolving notification target users
    /// </summary>
    public class NotificationTargetHelper
    {
        private readonly AspLorKingDomContext _context;
        private readonly ILogger<NotificationTargetHelper> _logger;

        public NotificationTargetHelper(
            AspLorKingDomContext context,
            ILogger<NotificationTargetHelper> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get target user IDs based on request target type
        /// </summary>
        public async Task<List<int>> GetTargetUserIdsAsync(SendNotificationRequest request)
        {
            return request.TargetType switch
            {
                NotificationConstants.TargetTypes.All => await GetAllActiveUsersAsync(),
                NotificationConstants.TargetTypes.Role => await GetUsersByRoleAsync(request.TargetRoleId),
                NotificationConstants.TargetTypes.User => GetSingleUser(request.TargetUserId),
                NotificationConstants.TargetTypes.Condition => await GetUsersByConditionAsync(request.ConditionJson),
                _ => new List<int>()
            };
        }

        /// <summary>
        /// Get all active users
        /// </summary>
        private async Task<List<int>> GetAllActiveUsersAsync()
        {
            return await _context.Accounts
                .Where(a => !a.IsDeleted && a.Status == "Active")
                .Select(a => a.AccountId)
                .ToListAsync();
        }

        /// <summary>
        /// Get users by role ID
        /// </summary>
        private async Task<List<int>> GetUsersByRoleAsync(int? roleId)
        {
            if (!roleId.HasValue)
            {
                _logger.LogWarning("RoleId is null when getting users by role");
                return new List<int>();
            }

            return await _context.Accounts
                .Where(a => !a.IsDeleted && a.Status == "Active" && a.RoleId == roleId.Value)
                .Select(a => a.AccountId)
                .ToListAsync();
        }

        /// <summary>
        /// Get single user
        /// </summary>
        private List<int> GetSingleUser(int? userId)
        {
            if (!userId.HasValue)
            {
                _logger.LogWarning("UserId is null when getting single user");
                return new List<int>();
            }

            return new List<int> { userId.Value };
        }

        /// <summary>
        /// Get users by complex condition (JSON)
        /// TODO: Implement complex condition handling
        /// </summary>
        private async Task<List<int>> GetUsersByConditionAsync(string? conditionJson)
        {
            _logger.LogWarning("Condition-based targeting not yet implemented. ConditionJson: {ConditionJson}", conditionJson);
            return await Task.FromResult(new List<int>());
        }
    }
}
