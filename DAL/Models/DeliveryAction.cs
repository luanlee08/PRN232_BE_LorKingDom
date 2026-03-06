#nullable enable
using System;

namespace DAL.Models;

public partial class DeliveryAction
{
    public long ActionId { get; set; }

    public long DeliveryId { get; set; }

    public int AccountId { get; set; }

    /// <summary>'Read' | 'Click'</summary>
    public string ActionType { get; set; } = null!;

    /// <summary>The URL, product ID, or voucher code that was clicked (null for 'Read' actions)</summary>
    public string? ActionTarget { get; set; }

    public DateTime OccurredAt { get; set; }

    // Navigation
    public virtual Delivery Delivery { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;
}
