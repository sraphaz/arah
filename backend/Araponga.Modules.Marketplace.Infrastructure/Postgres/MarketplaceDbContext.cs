using Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerritoryStoreRecord> TerritoryStores => Set<TerritoryStoreRecord>();
    public DbSet<StoreItemRecord> StoreItems => Set<StoreItemRecord>();
    public DbSet<ItemInquiryRecord> ItemInquiries => Set<ItemInquiryRecord>();
    public DbSet<CartRecord> Carts => Set<CartRecord>();
    public DbSet<CartItemRecord> CartItems => Set<CartItemRecord>();
    public DbSet<CheckoutRecord> Checkouts => Set<CheckoutRecord>();
    public DbSet<CheckoutItemRecord> CheckoutItems => Set<CheckoutItemRecord>();
    public DbSet<StoreRatingRecord> StoreRatings => Set<StoreRatingRecord>();
    public DbSet<StoreItemRatingRecord> StoreItemRatings => Set<StoreItemRatingRecord>();
    public DbSet<StoreRatingResponseRecord> StoreRatingResponses => Set<StoreRatingResponseRecord>();
    public DbSet<PlatformFeeConfigRecord> PlatformFeeConfigs => Set<PlatformFeeConfigRecord>();
    public DbSet<TerritoryPayoutConfigRecord> TerritoryPayoutConfigs => Set<TerritoryPayoutConfigRecord>();
    public DbSet<FinancialTransactionRecord> FinancialTransactions => Set<FinancialTransactionRecord>();
    public DbSet<TransactionStatusHistoryRecord> TransactionStatusHistories => Set<TransactionStatusHistoryRecord>();
    public DbSet<SellerBalanceRecord> SellerBalances => Set<SellerBalanceRecord>();
    public DbSet<SellerTransactionRecord> SellerTransactions => Set<SellerTransactionRecord>();
    public DbSet<PlatformFinancialBalanceRecord> PlatformFinancialBalances => Set<PlatformFinancialBalanceRecord>();
    public DbSet<PlatformRevenueTransactionRecord> PlatformRevenueTransactions => Set<PlatformRevenueTransactionRecord>();
    public DbSet<PlatformExpenseTransactionRecord> PlatformExpenseTransactions => Set<PlatformExpenseTransactionRecord>();
    public DbSet<ReconciliationRecordRecord> ReconciliationRecords => Set<ReconciliationRecordRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTerritoryStore(modelBuilder);
        ConfigureStoreItem(modelBuilder);
        ConfigureItemInquiry(modelBuilder);
        ConfigureCart(modelBuilder);
        ConfigureCartItem(modelBuilder);
        ConfigureCheckout(modelBuilder);
        ConfigureCheckoutItem(modelBuilder);
        ConfigurePlatformFeeConfig(modelBuilder);
        ConfigureTerritoryPayoutConfig(modelBuilder);
        ConfigureStoreRating(modelBuilder);
        ConfigureStoreItemRating(modelBuilder);
        ConfigureStoreRatingResponse(modelBuilder);
        ConfigureFinancialTransaction(modelBuilder);
        ConfigureTransactionStatusHistory(modelBuilder);
        ConfigureSellerBalance(modelBuilder);
        ConfigureSellerTransaction(modelBuilder);
        ConfigurePlatformFinancialBalance(modelBuilder);
        ConfigurePlatformRevenueTransaction(modelBuilder);
        ConfigurePlatformExpenseTransaction(modelBuilder);
        ConfigureReconciliationRecord(modelBuilder);
    }

    private static void ConfigureTerritoryStore(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryStoreRecord>(entity =>
        {
            entity.ToTable("territory_stores");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Description).HasMaxLength(1000);
            entity.Property(s => s.Status).HasConversion<int>();
            entity.Property(s => s.ContactVisibility).HasConversion<int>();
            entity.Property(s => s.Phone).HasMaxLength(80);
            entity.Property(s => s.Whatsapp).HasMaxLength(80);
            entity.Property(s => s.Email).HasMaxLength(320);
            entity.Property(s => s.Instagram).HasMaxLength(120);
            entity.Property(s => s.Website).HasMaxLength(200);
            entity.Property(s => s.PreferredContactMethod).HasMaxLength(80);
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.TerritoryId);
            entity.HasIndex(s => s.OwnerUserId);
            entity.HasIndex(s => new { s.TerritoryId, s.OwnerUserId }).IsUnique();
        });
    }

    private static void ConfigureStoreItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreItemRecord>(entity =>
        {
            entity.ToTable("store_items");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Type).HasConversion<int>();
            entity.Property(l => l.Title).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Description).HasMaxLength(4000);
            entity.Property(l => l.Category).HasMaxLength(120);
            entity.Property(l => l.Tags).HasMaxLength(4000);
            entity.Property(l => l.PricingType).HasConversion<int>();
            entity.Property(l => l.PriceAmount).HasColumnType("numeric(18,2)");
            entity.Property(l => l.Currency).HasMaxLength(10);
            entity.Property(l => l.Unit).HasMaxLength(120);
            entity.Property(l => l.Latitude).HasColumnType("double precision");
            entity.Property(l => l.Longitude).HasColumnType("double precision");
            entity.Property(l => l.Status).HasConversion<int>();
            entity.Property(l => l.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(l => l.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(l => l.TerritoryId);
            entity.HasIndex(l => l.StoreId);
            entity.HasIndex(l => new { l.TerritoryId, l.Type, l.Status });
        });
    }

    private static void ConfigureItemInquiry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemInquiryRecord>(entity =>
        {
            entity.ToTable("item_inquiries");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Status).HasConversion<int>();
            entity.Property(i => i.Message).HasMaxLength(2000);
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.TerritoryId);
            entity.HasIndex(i => i.ItemId);
            entity.HasIndex(i => i.StoreId);
            entity.HasIndex(i => i.FromUserId);
        });
    }

    private static void ConfigureCart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartRecord>(entity =>
        {
            entity.ToTable("carts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => new { c.TerritoryId, c.UserId }).IsUnique();
        });
    }

    private static void ConfigureCartItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItemRecord>(entity =>
        {
            entity.ToTable("cart_items");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Notes).HasMaxLength(1000);
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(i => i.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.CartId);
            entity.HasIndex(i => i.ItemId);
            entity.HasIndex(i => new { i.CartId, i.ItemId }).IsUnique();
        });
    }

    private static void ConfigureCheckout(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckoutRecord>(entity =>
        {
            entity.ToTable("checkouts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Status).HasConversion<int>();
            entity.Property(c => c.Currency).HasMaxLength(10).IsRequired();
            entity.Property(c => c.ItemsSubtotalAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.PlatformFeeAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.TotalAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => c.BuyerUserId);
            entity.HasIndex(c => c.StoreId);
        });
    }

    private static void ConfigureCheckoutItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CheckoutItemRecord>(entity =>
        {
            entity.ToTable("checkout_items");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ItemType).HasConversion<int>();
            entity.Property(i => i.TitleSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineSubtotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.PlatformFeeLine).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineTotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.CheckoutId);
            entity.HasIndex(i => i.ItemId);
        });
    }

    private static void ConfigurePlatformFeeConfig(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlatformFeeConfigRecord>(entity =>
        {
            entity.ToTable("platform_fee_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ItemType).HasConversion<int>();
            entity.Property(c => c.FeeMode).HasConversion<int>();
            entity.Property(c => c.FeeValue).HasColumnType("numeric(18,4)");
            entity.Property(c => c.Currency).HasMaxLength(10);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.ItemType, c.IsActive }).IsUnique();
        });
    }

    private static void ConfigureTerritoryPayoutConfig(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryPayoutConfigRecord>(entity =>
        {
            entity.ToTable("territory_payout_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Currency).HasMaxLength(10).IsRequired();
            entity.Property(c => c.Frequency).HasConversion<int>().IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.IsActive });
        });
    }

    private static void ConfigureStoreRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreRatingRecord>(entity =>
        {
            entity.ToTable("store_ratings");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(2000);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.StoreId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.StoreId, r.UserId }).IsUnique();
        });
    }

    private static void ConfigureStoreItemRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreItemRatingRecord>(entity =>
        {
            entity.ToTable("store_item_ratings");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(2000);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.StoreItemId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.StoreItemId, r.UserId }).IsUnique();
        });
    }

    private static void ConfigureStoreRatingResponse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoreRatingResponseRecord>(entity =>
        {
            entity.ToTable("store_rating_responses");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.ResponseText).HasMaxLength(2000).IsRequired();
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.RatingId).IsUnique();
            entity.HasIndex(r => r.StoreId);
        });
    }

    private static void ConfigureFinancialTransaction(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureTransactionStatusHistory(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureSellerBalance(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureSellerTransaction(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigurePlatformFinancialBalance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlatformFinancialBalanceRecord>(entity =>
        {
            entity.ToTable("platform_financial_balances");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Currency).HasMaxLength(10).IsRequired();
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.TerritoryId).IsUnique();
        });
    }

    private static void ConfigurePlatformRevenueTransaction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlatformRevenueTransactionRecord>(entity =>
        {
            entity.ToTable("platform_revenue_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.CheckoutId);
        });
    }

    private static void ConfigurePlatformExpenseTransaction(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureReconciliationRecord(ModelBuilder modelBuilder)
    {
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
    }
}
