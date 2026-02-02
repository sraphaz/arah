using Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres;

public sealed class SubscriptionsDbContext : DbContext
{
    public SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<SubscriptionPlanRecord> SubscriptionPlans => Set<SubscriptionPlanRecord>();
    public DbSet<SubscriptionRecord> Subscriptions => Set<SubscriptionRecord>();
    public DbSet<SubscriptionPaymentRecord> SubscriptionPayments => Set<SubscriptionPaymentRecord>();
    public DbSet<CouponRecord> Coupons => Set<CouponRecord>();
    public DbSet<SubscriptionCouponRecord> SubscriptionCoupons => Set<SubscriptionCouponRecord>();
    public DbSet<SubscriptionPlanHistoryRecord> SubscriptionPlanHistories => Set<SubscriptionPlanHistoryRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionPlanRecord>(entity =>
        {
            entity.ToTable("subscription_plans");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.Tier).HasConversion<int>().IsRequired();
            entity.Property(p => p.Scope).HasConversion<int>().IsRequired();
            entity.Property(p => p.PricePerCycle).HasColumnType("numeric(18,2)");
            entity.Property(p => p.BillingCycle).HasConversion<int>();
            entity.Property(p => p.CapabilitiesJson).HasColumnType("jsonb").IsRequired();
            entity.Property(p => p.LimitsJson).HasColumnType("jsonb");
            entity.Property(p => p.StripePriceId).HasMaxLength(200);
            entity.Property(p => p.StripeProductId).HasMaxLength(200);
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(p => p.Scope);
            entity.HasIndex(p => p.TerritoryId);
            entity.HasIndex(p => new { p.Scope, p.TerritoryId });
            entity.HasIndex(p => new { p.Scope, p.IsDefault });
        });

        modelBuilder.Entity<SubscriptionRecord>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Status).HasConversion<int>().IsRequired();
            entity.Property(s => s.CurrentPeriodStart).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(s => s.CurrentPeriodEnd).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(s => s.TrialStart).HasColumnType("timestamp with time zone");
            entity.Property(s => s.TrialEnd).HasColumnType("timestamp with time zone");
            entity.Property(s => s.CanceledAt).HasColumnType("timestamp with time zone");
            entity.Property(s => s.StripeSubscriptionId).HasMaxLength(200);
            entity.Property(s => s.StripeCustomerId).HasMaxLength(200);
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(s => s.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.TerritoryId);
            entity.HasIndex(s => s.PlanId);
            entity.HasIndex(s => new { s.UserId, s.TerritoryId });
            entity.HasIndex(s => s.Status);
            entity.HasIndex(s => s.StripeSubscriptionId).IsUnique().HasFilter("\"StripeSubscriptionId\" IS NOT NULL");
        });

        modelBuilder.Entity<SubscriptionPaymentRecord>(entity =>
        {
            entity.ToTable("subscription_payments");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Amount).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(p => p.Currency).HasMaxLength(10).IsRequired();
            entity.Property(p => p.Status).HasConversion<int>().IsRequired();
            entity.Property(p => p.PaymentDate).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.PeriodStart).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.PeriodEnd).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.StripeInvoiceId).HasMaxLength(200);
            entity.Property(p => p.StripePaymentIntentId).HasMaxLength(200);
            entity.Property(p => p.FailureReason).HasMaxLength(500);
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(p => p.SubscriptionId);
            entity.HasIndex(p => p.Status);
            entity.HasIndex(p => p.StripeInvoiceId).IsUnique().HasFilter("\"StripeInvoiceId\" IS NOT NULL");
        });

        modelBuilder.Entity<CouponRecord>(entity =>
        {
            entity.ToTable("coupons");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Code).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.DiscountType).HasConversion<int>().IsRequired();
            entity.Property(c => c.DiscountValue).HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(c => c.ValidFrom).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(c => c.ValidUntil).HasColumnType("timestamp with time zone");
            entity.Property(c => c.StripeCouponId).HasMaxLength(200);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(c => c.Code).IsUnique();
            entity.HasIndex(c => c.IsActive);
            entity.HasIndex(c => c.StripeCouponId).IsUnique().HasFilter("\"StripeCouponId\" IS NOT NULL");
        });

        modelBuilder.Entity<SubscriptionCouponRecord>(entity =>
        {
            entity.ToTable("subscription_coupons");
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.AppliedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(sc => sc.SubscriptionId);
            entity.HasIndex(sc => sc.CouponId);
            entity.HasIndex(sc => new { sc.SubscriptionId, sc.CouponId }).IsUnique();
        });

        modelBuilder.Entity<SubscriptionPlanHistoryRecord>(entity =>
        {
            entity.ToTable("subscription_plan_histories");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.ChangeType).HasConversion<int>().IsRequired();
            entity.Property(h => h.PreviousStateJson).HasColumnType("jsonb");
            entity.Property(h => h.NewStateJson).HasColumnType("jsonb");
            entity.Property(h => h.ChangeReason).HasMaxLength(500);
            entity.Property(h => h.ChangedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(h => h.PlanId);
            entity.HasIndex(h => h.ChangedByUserId);
            entity.HasIndex(h => new { h.PlanId, h.ChangedAtUtc });
        });
    }
}
