using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class NotificationHelper
    {
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
