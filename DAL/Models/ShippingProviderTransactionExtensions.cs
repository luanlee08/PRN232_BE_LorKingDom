using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

/// <summary>
/// Extension of auto-generated ShippingProviderTransaction with
/// polling/concurrency fields not yet in the EF Power Tools scaffold.
/// These columns are added via Migrations/AddShippingStatusHistory.sql.
/// </summary>
public partial class ShippingProviderTransaction
{
    /// <summary>UTC timestamp of the last time this record was polled from GHN API.</summary>
    public DateTime? LastPolledAt { get; set; }

    /// <summary>Consecutive error counter. Reset to 0 on success. Stopped at 5.</summary>
    public int RetryCount { get; set; }

    /// <summary>Last error message from GHN API or processing failure.</summary>
    public string? LastErrorMessage { get; set; }

    /// <summary>
    /// EF Core optimistic concurrency token.
    /// Maps to SQL Server ROWVERSION / TIMESTAMP column.
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
