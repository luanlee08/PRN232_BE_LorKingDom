#nullable enable

namespace DAL.Models;

public partial class CampaignTarget
{
    public int CampaignTargetId { get; set; }

    public int CampaignId { get; set; }

    /// <summary>Stores: AccountId (int string), GroupCode, or "ALL"</summary>
    public string TargetValue { get; set; } = null!;

    // Navigation
    public virtual Campaign Campaign { get; set; } = null!;
}
