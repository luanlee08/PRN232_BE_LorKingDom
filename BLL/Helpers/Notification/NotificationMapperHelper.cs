using BLL.DTOs.Notifications;
using DAL.Models;

namespace BLL.Helpers.Notification
{
    /// <summary>
    /// Helper for mapping Delivery entities to DTOs
    /// </summary>
    public class NotificationMapperHelper
    {
        /// <summary>
        /// Map Delivery entity to DeliveryResponse DTO
        /// </summary>
        public DeliveryResponse MapToResponse(Delivery delivery)
        {
            return new DeliveryResponse
            {
                DeliveryId = delivery.DeliveryId,
                AccountId = delivery.AccountId,
                AccountName = delivery.Account?.AccountName,
                AccountEmail = delivery.Account?.Email,
                CreatedByJobId = delivery.CreatedByJobId,
                JobName = delivery.CreatedByJob?.JobName,
                TemplateCode = delivery.TemplateCode,
                Title = delivery.Title,
                Message = delivery.Message,
                Payload = delivery.Payload,
                ImageUrl = delivery.ImageUrl,
                ActionType = delivery.ActionType,
                ActionTarget = delivery.ActionTarget,
                Status = delivery.Status,
                CreatedAt = delivery.CreatedAt
            };
        }
    }
}
