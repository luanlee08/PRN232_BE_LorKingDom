using System;

namespace BLL.DTOs.Campaigns
{
    public class CampaignQuery
    {
        public string? Keyword { get; set; }

        /// <summary>'Draft' | 'Scheduled' | 'Processing' | 'Completed' | 'Failed'</summary>
        public string? Status { get; set; }

        /// <summary>'ADMIN' | 'SYSTEM' | 'WORKER'</summary>
        public string? SourceType { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
