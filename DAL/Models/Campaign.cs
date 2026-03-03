#nullable enable
using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Campaign
{
    public int CampaignId { get; set; }

    /// <summary>Human-readable campaign label set by admin</summary>
    public string CampaignName { get; set; } = null!;

    /// <summary>Optional template reference. Null when fully overriding content.</summary>
    public string? TemplateCode { get; set; }

    /// <summary>Admin title override (replaces template's TitleTemplate)</summary>
    public string? TitleOverride { get; set; }

    /// <summary>Admin message override (replaces template's MessageTemplate)</summary>
    public string? MessageOverride { get; set; }

    /// <summary>'ADMIN' | 'SYSTEM' | 'WORKER'</summary>
    public string SourceType { get; set; } = null!;

    /// <summary>'ALL' | 'GROUP' | 'CUSTOM' | 'SINGLE'</summary>
    public string TargetType { get; set; } = null!;

    /// <summary>'Draft' | 'Scheduled' | 'Processing' | 'Completed' | 'Failed'</summary>
    public string Status { get; set; } = null!;

    public DateTime? ScheduledAt { get; set; }

    /// <summary>Optional event key for system auto-triggers</summary>
    public string? EventKey { get; set; }

    public string? ImageUrl { get; set; }

    /// <summary>'product' | 'voucher' | 'url' | 'none'</summary>
    public string? ActionType { get; set; }

    public string? ActionTarget { get; set; }

    public int CreatedByAccountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual Account CreatedByAccount { get; set; } = null!;
    public virtual Template? TemplateCodeNavigation { get; set; }
    public virtual ICollection<CampaignTarget> CampaignTargets { get; set; } = new List<CampaignTarget>();
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
