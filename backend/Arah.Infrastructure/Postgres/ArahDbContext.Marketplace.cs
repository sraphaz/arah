using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureMarketplace(ModelBuilder modelBuilder)
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
}
