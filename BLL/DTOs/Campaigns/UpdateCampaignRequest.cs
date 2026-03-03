using System;
using System.Collections.Generic;

namespace BLL.DTOs.Campaigns
{
    public class UpdateCampaignRequest
    {
        public string? CampaignName { get; set; }
        public string? TemplateCode { get; set; }
        public string? TitleOverride { get; set; }
        public string? MessageOverride { get; set; }
        public string? TargetType { get; set; }
        public List<string>? TargetValues { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string? EventKey { get; set; }
        public string? ImageUrl { get; set; }
        public string? ActionType { get; set; }
        public string? ActionTarget { get; set; }
    }
}
