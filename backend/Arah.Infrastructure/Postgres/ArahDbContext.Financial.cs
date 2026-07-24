using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureFinancial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FinancialTransactionRecord>(entity =>
        {
            entity.ToTable("financial_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).HasConversion<int>().IsRequired();
            entity.Property(t => t.Status).HasConversion<int>().IsRequired();
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(500).IsRequired();
            entity.Property(t => t.RelatedEntityType).HasMaxLength(100);
            entity.Property(t => t.RelatedTransactionIdsJson).HasColumnType("text").IsRequired();
            entity.Property(t => t.MetadataJson).HasColumnType("text");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => new { t.TerritoryId, t.Type });
            entity.HasIndex(t => new { t.TerritoryId, t.Status });
            entity.HasIndex(t => t.RelatedEntityId);
        });

        modelBuilder.Entity<TransactionStatusHistoryRecord>(entity =>
        {
            entity.ToTable("transaction_status_histories");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.PreviousStatus).HasConversion<int>().IsRequired();
            entity.Property(h => h.NewStatus).HasConversion<int>().IsRequired();
            entity.Property(h => h.Reason).HasMaxLength(500);
            entity.Property(h => h.ChangedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne<FinancialTransactionRecord>()
                .WithMany()
                .HasForeignKey(h => h.FinancialTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(h => h.FinancialTransactionId);
            entity.HasIndex(h => h.ChangedByUserId);
        });

        modelBuilder.Entity<SellerBalanceRecord>(entity =>
        {
            entity.ToTable("seller_balances");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Currency).HasMaxLength(10).IsRequired();
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => new { b.TerritoryId, b.SellerUserId }).IsUnique();
            entity.HasIndex(b => b.TerritoryId);
            entity.HasIndex(b => b.SellerUserId);
        });

        modelBuilder.Entity<SellerTransactionRecord>(entity =>
        {
            entity.ToTable("seller_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.Status).HasConversion<int>().IsRequired();
            entity.Property(t => t.PayoutId).HasMaxLength(200);
            entity.Property(t => t.ReadyForPayoutAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.PaidAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.CheckoutId).IsUnique();
            entity.HasIndex(t => t.SellerUserId);
            entity.HasIndex(t => new { t.TerritoryId, t.Status });
            entity.HasIndex(t => t.PayoutId);
        });

        modelBuilder.Entity<PlatformFinancialBalanceRecord>(entity =>
        {
            entity.ToTable("platform_financial_balances");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Currency).HasMaxLength(10).IsRequired();
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.TerritoryId).IsUnique();
        });

        modelBuilder.Entity<PlatformRevenueTransactionRecord>(entity =>
        {
            entity.ToTable("platform_revenue_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.CheckoutId);
        });

        modelBuilder.Entity<PlatformExpenseTransactionRecord>(entity =>
        {
            entity.ToTable("platform_expense_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.PayoutId).HasMaxLength(200);
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.SellerTransactionId);
            entity.HasIndex(t => t.PayoutId);
        });

        modelBuilder.Entity<ReconciliationRecordRecord>(entity =>
        {
            entity.ToTable("reconciliation_records");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Currency).HasMaxLength(10).IsRequired();
            entity.Property(r => r.Status).HasConversion<int>().IsRequired();
            entity.Property(r => r.Notes).HasMaxLength(1000);
            entity.Property(r => r.ReconciliationDate).HasColumnType("timestamp with time zone");
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.TerritoryId);
            entity.HasIndex(r => new { r.TerritoryId, r.Status });
            entity.HasIndex(r => r.ReconciliationDate);
        });

        modelBuilder.Entity<FeeSplitRuleRecord>(entity =>
        {
            entity.ToTable("fee_split_rules");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RevenueType).HasMaxLength(64).IsRequired();
            entity.Property(r => r.ImplementerSharePercent).HasPrecision(5, 2);
            entity.Property(r => r.TerritoryFundSharePercent).HasPrecision(5, 2);
            entity.Property(r => r.PlatformSharePercent).HasPrecision(5, 2);
            entity.Property(r => r.EffectiveFromUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.SupersededAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => new { r.TerritoryId, r.RevenueType });
        });
    }
}
