namespace BLL.Helpers.Notification
{
    /// <summary>
    /// Helper for replacing template parameters
    /// </summary>
    public class NotificationHelper
    {
        /// <summary>
        /// Replace template parameters with actual values
        /// Supports both {{key}} and {key} formats
        /// </summary>
        public string ReplaceTemplateParameters(string text, Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
                return text;

            foreach (var param in parameters)
            {
                // Support both {{key}} and {key} formats
                text = text.Replace($"{{{{{param.Key}}}}}", param.Value);
                text = text.Replace($"{{{param.Key}}}", param.Value);
            }

            return text;
        }
    }
}
