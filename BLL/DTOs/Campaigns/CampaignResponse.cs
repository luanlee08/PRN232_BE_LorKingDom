using System;
using System.Collections.Generic;

namespace BLL.DTOs.Campaigns
{
    /// <summary>Campaign summary row (used in list + single-item responses)</summary>
    public class CampaignResponse
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; } = null!;
        public string? TemplateCode { get; set; }
        public string? TitleOverride { get; set; }
        public string? MessageOverride { get; set; }
        public string SourceType { get; set; } = null!;
        public string TargetType { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime? ScheduledAt { get; set; }
        public string? EventKey { get; set; }
        public string? ImageUrl { get; set; }
        public string? ActionType { get; set; }
        public string? ActionTarget { get; set; }
        public int CreatedByAccountId { get; set; }
        public string? CreatedByAccountName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ── Analytics (computed from Deliveries + DeliveryActions) ──
        public int TotalRecipients { get; set; }
        public int TotalSent { get; set; }
        public int TotalRead { get; set; }
        public int TotalClicked { get; set; }
        public double CtrPercent => TotalSent > 0 ? Math.Round((double)TotalClicked / TotalSent * 100, 1) : 0;

        public List<string> TargetValues { get; set; } = [];
    }

    /// <summary>Extended response for the detail / analytics page</summary>
    public class CampaignDetailResponse : CampaignResponse
    {
        public List<RecipientRow> Recipients { get; set; } = [];
        public List<TimelinePoint> ClickTimeline { get; set; } = [];
    }

    public class RecipientRow
    {
        public long DeliveryId { get; set; }
        public int AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public string DeliveryStatus { get; set; } = null!;   // 'Unread' | 'Read'
        public DateTime DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? ClickedAt { get; set; }
    }

    public class TimelinePoint
    {
        public string Date { get; set; } = null!;   // "yyyy-MM-dd"
        public int Clicks { get; set; }
        public int Reads { get; set; }
    }

    /// <summary>Request body for recording a click/read action from the client</summary>
    public class RecordActionRequest
    {
        public long DeliveryId { get; set; }

        /// <summary>'Read' | 'Click'</summary>
        public string ActionType { get; set; } = null!;

        public string? ActionTarget { get; set; }
    }
}
