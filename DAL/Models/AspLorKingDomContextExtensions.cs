// Partial class extension — DO NOT regenerate with EF Core Power Tools.
// Adds ShippingStatusHistories DbSet and maps new columns added by
// Migrations/AddShippingStatusHistory.sql.
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class AspLorKingDomContext
{
    public virtual DbSet<ShippingStatusHistory> ShippingStatusHistories { get; set; } = null!;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // -------------------------------------------------------
        // ShippingProviderTransaction — new polling columns
        // -------------------------------------------------------
        modelBuilder.Entity<ShippingProviderTransaction>(entity =>
        {
            entity.Property(e => e.LastPolledAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            entity.Property(e => e.RetryCount)
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(e => e.LastErrorMessage)
                .HasMaxLength(500)
                .IsRequired(false);

            // Optimistic concurrency — maps to SQL ROWVERSION column
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsRequired(false); // nullable until DB column exists
        });

        // -------------------------------------------------------
        // ShippingStatusHistories — new table
        // -------------------------------------------------------
        modelBuilder.Entity<ShippingStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("ShippingStatusHistories");

            entity.Property(e => e.HistoryId).UseIdentityColumn();

            entity.Property(e => e.ShippingTxId)
                .HasColumnName("ShippingTxId")
                .IsRequired();

            entity.Property(e => e.OrderId)
                .HasColumnName("OrderId")
                .IsRequired();

            entity.Property(e => e.PreviousStatus)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.NewStatus)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Source)
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Polling");

            entity.Property(e => e.ProcessedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(h => h.ShippingTxn)
                .WithMany()
                .HasForeignKey(h => h.ShippingTxId)
                .HasConstraintName("FK_ShippingStatusHistories_ShippingTxn")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.Order)
                .WithMany()
                .HasForeignKey(h => h.OrderId)
                .HasConstraintName("FK_ShippingStatusHistories_Orders")
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.ShippingTxId)
                .HasDatabaseName("IX_ShippingStatusHistories_ShippingTxId");

            entity.HasIndex(e => new { e.OrderId, e.ProcessedAt })
                .HasDatabaseName("IX_ShippingStatusHistories_OrderId");
        });
    }
}
