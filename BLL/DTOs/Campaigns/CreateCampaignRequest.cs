using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Campaigns
{
    public class CreateCampaignRequest
    {
        [Required(ErrorMessage = "Tên chiến dịch không được để trống")]
        [MaxLength(255)]
        public string CampaignName { get; set; } = null!;

        /// <summary>Optional template code. If null, TitleOverride + MessageOverride are required.</summary>
        public string? TemplateCode { get; set; }

        [MaxLength(255)]
        public string? TitleOverride { get; set; }

        [MaxLength(500)]
        public string? MessageOverride { get; set; }

        /// <summary>'ADMIN' | 'SYSTEM' | 'WORKER'</summary>
        public string SourceType { get; set; } = "ADMIN";

        /// <summary>'ALL' | 'GROUP' | 'CUSTOM' | 'SINGLE'</summary>
        [Required]
        public string TargetType { get; set; } = null!;

        /// <summary>
        /// List of target values:
        ///   TargetType=ALL   → empty (ignored)
        ///   TargetType=GROUP → group codes e.g. ["VIP","NEW"]
        ///   TargetType=CUSTOM / SINGLE → account ID strings e.g. ["12","45"]
        /// </summary>
        public List<string> TargetValues { get; set; } = [];

        public DateTime? ScheduledAt { get; set; }

        [MaxLength(100)]
        public string? EventKey { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>'product' | 'voucher' | 'url' | 'none'</summary>
        [MaxLength(20)]
        public string? ActionType { get; set; }

        [MaxLength(500)]
        public string? ActionTarget { get; set; }
    }
}
