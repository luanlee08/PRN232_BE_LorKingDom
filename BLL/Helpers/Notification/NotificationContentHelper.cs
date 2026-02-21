using BLL.DTOs.Notifications;
using DAL.Interface;
using Microsoft.Extensions.Logging;

namespace BLL.Helpers.Notification
{
    /// <summary>
    /// Helper for preparing notification content from templates
    /// </summary>
    public class NotificationContentHelper
    {
        private readonly ITemplateRepository _templateRepo;
        private readonly NotificationHelper _notificationHelper;
        private readonly ILogger<NotificationContentHelper> _logger;

        public NotificationContentHelper(
            ITemplateRepository templateRepo,
            NotificationHelper notificationHelper,
            ILogger<NotificationContentHelper> logger)
        {
            _templateRepo = templateRepo;
            _notificationHelper = notificationHelper;
            _logger = logger;
        }

        /// <summary>
        /// Prepare notification content from template or custom content
        /// </summary>
        /// <returns>Tuple of (templateCode, title, message, payload)</returns>
        public async Task<(string templateCode, string title, string message, string payload)> PrepareContentAsync(SendNotificationRequest request)
        {
            string templateCode;
            string title;
            string message;
            string payload = request.Payload ?? "{}";

            // If using template
            if (!string.IsNullOrWhiteSpace(request.TemplateCode))
            {
                var template = await _templateRepo.GetByCodeAsync(request.TemplateCode);

                if (template == null)
                {
                    _logger.LogWarning("Template not found: {TemplateCode}", request.TemplateCode);

                    // Fallback to custom content or throw
                    if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.Message))
                    {
                        templateCode = NotificationConstants.TemplateCodes.Custom;
                        title = request.Title;
                        message = request.Message;
                    }
                    else
                    {
                        throw new Exception($"Template '{request.TemplateCode}' not found and no fallback content provided");
                    }
                }
                else
                {
                    templateCode = template.TemplateCode;

                    // Use template, allow override from request
                    title = request.Title ?? template.TitleTemplate;
                    message = request.Message ?? template.MessageTemplate;

                    // Replace parameters if provided
                    if (request.Parameters != null && request.Parameters.Any())
                    {
                        title = _notificationHelper.ReplaceTemplateParameters(title, request.Parameters);
                        message = _notificationHelper.ReplaceTemplateParameters(message, request.Parameters);
                    }
                }
            }
            else
            {
                // Custom content
                templateCode = NotificationConstants.TemplateCodes.Custom;
                title = request.Title!;
                message = request.Message!;
            }

            return (templateCode, title, message, payload);
        }
    }
}
