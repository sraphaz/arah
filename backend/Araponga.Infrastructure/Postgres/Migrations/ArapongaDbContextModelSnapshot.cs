using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Araponga.Infrastructure.Postgres.Migrations;

[DbContext(typeof(ArapongaDbContext))]
public partial class ArapongaDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

        modelBuilder.Entity<ActiveTerritoryRecord>(entity =>
        {
            entity.ToTable("active_territories");
            entity.HasKey(a => a.SessionId);
            entity.Property(a => a.SessionId).HasMaxLength(200).IsRequired();
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
        });

        modelBuilder.Entity<AuditEntryRecord>(entity =>
        {
            entity.ToTable("audit_entries");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).HasMaxLength(200).IsRequired();
            entity.Property(a => a.TimestampUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
            entity.HasIndex(a => a.ActorUserId);
            entity.HasIndex(a => a.TimestampUtc);
        });

        modelBuilder.Entity<CommunityPostRecord>(entity =>
        {
            entity.ToTable("community_posts");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.AuthorUserId).IsRequired();
            entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Content).HasMaxLength(4000).IsRequired();
            entity.Property(p => p.Type).HasConversion<int>();
            entity.Property(p => p.Visibility).HasConversion<int>();
            entity.Property(p => p.Status).HasConversion<int>();
            entity.Property(p => p.ReferenceType).HasMaxLength(40);
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(p => p.TerritoryId);
            entity.HasIndex(p => new { p.TerritoryId, p.CreatedAtUtc });
            entity.HasIndex(p => p.AuthorUserId);
            entity.HasIndex(p => p.MapEntityId);
            entity.HasIndex(p => new { p.ReferenceType, p.ReferenceId });
        });

        modelBuilder.Entity<FeatureFlagRecord>(entity =>
        {
            entity.ToTable("feature_flags");
            entity.HasKey(f => new { f.TerritoryId, f.Flag });
            entity.Property(f => f.Flag).HasConversion<int>();
            entity.HasIndex(f => f.TerritoryId);
        });

        modelBuilder.Entity<HealthAlertRecord>(entity =>
        {
            entity.ToTable("health_alerts");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Title).HasMaxLength(200).IsRequired();
            entity.Property(h => h.Description).HasMaxLength(4000).IsRequired();
            entity.Property(h => h.Status).HasConversion<int>();
            entity.Property(h => h.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(h => h.TerritoryId);
            entity.HasIndex(h => new { h.TerritoryId, h.Status });
        });

        modelBuilder.Entity<TerritoryEventRecord>(entity =>
        {
            entity.ToTable("territory_events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.StartsAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.EndsAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.Latitude).HasColumnType("double precision");
            entity.Property(e => e.Longitude).HasColumnType("double precision");
            entity.Property(e => e.LocationLabel).HasMaxLength(200);
            entity.Property(e => e.CreatedByMembership).HasConversion<string>().HasMaxLength(32);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(e => e.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(e => new { e.TerritoryId, e.StartsAtUtc });
            entity.HasIndex(e => new { e.TerritoryId, e.Status });
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
        });

        modelBuilder.Entity<EventParticipationRecord>(entity =>
        {
            entity.ToTable("event_participations");
            entity.HasKey(p => new { p.EventId, p.UserId });
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(p => new { p.EventId, p.Status });
            entity.HasIndex(p => new { p.UserId, p.Status });
        });

        modelBuilder.Entity<TerritoryAssetRecord>(entity =>
        {
            entity.ToTable("territory_assets");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Type).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Name).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(1000);
            entity.Property(a => a.Status).HasConversion<int>();
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.ArchivedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.ArchiveReason).HasMaxLength(500);
            entity.HasIndex(a => a.TerritoryId);
            entity.HasIndex(a => new { a.TerritoryId, a.Status });
            entity.HasIndex(a => new { a.TerritoryId, a.Type });
        });

        modelBuilder.Entity<AssetGeoAnchorRecord>(entity =>
        {
            entity.ToTable("asset_geo_anchors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Latitude).HasColumnType("double precision");
            entity.Property(a => a.Longitude).HasColumnType("double precision");
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.AssetId);
        });

        modelBuilder.Entity<AssetValidationRecord>(entity =>
        {
            entity.ToTable("asset_validations");
            entity.HasKey(a => new { a.AssetId, a.UserId });
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.UserId);
        });

        modelBuilder.Entity<PostAssetRecord>(entity =>
        {
            entity.ToTable("post_assets");
            entity.HasKey(p => new { p.PostId, p.AssetId });
            entity.HasIndex(p => p.AssetId);
        });

        modelBuilder.Entity<MapEntityRecord>(entity =>
        {
            entity.ToTable("map_entities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedByUserId).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Latitude).HasColumnType("double precision");
            entity.Property(e => e.Longitude).HasColumnType("double precision");
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Visibility).HasConversion<int>();
            entity.Property(e => e.ConfirmationCount).IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(e => e.TerritoryId);
            entity.HasIndex(e => new { e.TerritoryId, e.Status });
            entity.HasIndex(e => new { e.TerritoryId, e.Visibility });
            entity.HasIndex(e => e.CreatedByUserId);
        });

        modelBuilder.Entity<MapEntityRelationRecord>(entity =>
        {
            entity.ToTable("map_entity_relations");
            entity.HasKey(r => new { r.UserId, r.EntityId });
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.EntityId);
        });

        modelBuilder.Entity<ModerationReportRecord>(entity =>
        {
            entity.ToTable("moderation_reports");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.TerritoryId).IsRequired();
            entity.Property(r => r.TargetType).HasConversion<int>();
            entity.Property(r => r.Reason).HasMaxLength(300).IsRequired();
            entity.Property(r => r.Details).HasMaxLength(2000);
            entity.Property(r => r.Status).HasConversion<int>();
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.ReporterUserId);
            entity.HasIndex(r => new { r.TargetType, r.TargetId });
            entity.HasIndex(r => new { r.TerritoryId, r.CreatedAtUtc });
            entity.HasIndex(r => r.CreatedAtUtc);
        });

        modelBuilder.Entity<OutboxMessageRecord>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).HasMaxLength(200).IsRequired();
            entity.Property(o => o.PayloadJson).HasColumnType("jsonb");
            entity.Property(o => o.OccurredAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessAfterUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.Attempts).HasDefaultValue(0);
            entity.Property(o => o.LastError).HasColumnType("text");
            entity.HasIndex(o => new { o.ProcessedAtUtc, o.ProcessAfterUtc });
            entity.HasIndex(o => new { o.Type, o.ProcessedAtUtc });
        });

        modelBuilder.Entity<PostCommentRecord>(entity =>
        {
            entity.ToTable("post_comments");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.PostId);
        });

        modelBuilder.Entity<PostGeoAnchorRecord>(entity =>
        {
            entity.ToTable("post_geo_anchors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Type).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Latitude).HasColumnType("double precision");
            entity.Property(a => a.Longitude).HasColumnType("double precision");
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.PostId);
        });

        modelBuilder.Entity<PostLikeRecord>(entity =>
        {
            entity.ToTable("post_likes");
            entity.HasKey(l => new { l.PostId, l.ActorId });
            entity.Property(l => l.ActorId).HasMaxLength(160).IsRequired();
            entity.Property(l => l.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(l => l.PostId);
        });

        modelBuilder.Entity<PostShareRecord>(entity =>
        {
            entity.ToTable("post_shares");
            entity.HasKey(s => new { s.PostId, s.UserId });
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.PostId);
        });

        modelBuilder.Entity<SanctionRecord>(entity =>
        {
            entity.ToTable("sanctions");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Scope).HasConversion<int>();
            entity.Property(s => s.TargetType).HasConversion<int>();
            entity.Property(s => s.Type).HasConversion<int>();
            entity.Property(s => s.Status).HasConversion<int>();
            entity.Property(s => s.Reason).HasMaxLength(400).IsRequired();
            entity.Property(s => s.StartAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.EndAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.TargetId);
            entity.HasIndex(s => s.TerritoryId);
        });

        modelBuilder.Entity<TerritoryRecord>(entity =>
        {
            entity.ToTable("territories");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ParentTerritoryId);
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(1000);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.City).HasMaxLength(120).IsRequired();
            entity.Property(t => t.State).HasMaxLength(60).IsRequired();
            entity.Property(t => t.Latitude).HasColumnType("double precision");
            entity.Property(t => t.Longitude).HasColumnType("double precision");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.Name);
            entity.HasIndex(t => t.City);
            entity.HasIndex(t => t.State);
            entity.HasIndex(t => new { t.City, t.State });
        });

        modelBuilder.Entity<TerritoryJoinRequestRecipientRecord>(entity =>
        {
            entity.ToTable("territory_join_request_recipients");
            entity.HasKey(r => new { r.JoinRequestId, r.RecipientUserId });
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.RespondedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.JoinRequestId);
            entity.HasIndex(r => r.RecipientUserId);
        });

        modelBuilder.Entity<TerritoryJoinRequestRecord>(entity =>
        {
            entity.ToTable("territory_join_requests");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).HasConversion<string>();
            entity.Property(r => r.Status).HasMaxLength(32);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.ExpiresAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.DecidedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.TerritoryId);
            entity.HasIndex(r => r.RequesterUserId);
            entity.HasIndex(r => new { r.TerritoryId, r.RequesterUserId })
                .HasFilter("\"Status\" = 'Pending'")
                .IsUnique();
        });

        modelBuilder.Entity<TerritoryMembershipRecord>(entity =>
        {
            entity.ToTable("territory_memberships");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Role).HasConversion<int>();
            entity.Property(m => m.VerificationStatus).HasConversion<int>();
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(m => m.UserId);
            entity.HasIndex(m => m.TerritoryId);
            entity.HasIndex(m => new { m.UserId, m.TerritoryId }).IsUnique();
        });

        modelBuilder.Entity<UserBlockRecord>(entity =>
        {
            entity.ToTable("user_blocks");
            entity.HasKey(b => new { b.BlockerUserId, b.BlockedUserId });
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.BlockerUserId);
        });

        modelBuilder.Entity<UserNotificationRecord>(entity =>
        {
            entity.ToTable("user_notifications");
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
            entity.Property(n => n.Body).HasMaxLength(1000);
            entity.Property(n => n.Kind).HasMaxLength(200).IsRequired();
            entity.Property(n => n.DataJson).HasColumnType("jsonb");
            entity.Property(n => n.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(n => n.ReadAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(n => n.UserId);
            entity.HasIndex(n => n.CreatedAtUtc);
            entity.HasIndex(n => new { n.SourceOutboxId, n.UserId }).IsUnique();
        });

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

        modelBuilder.Entity<StoreListingRecord>(entity =>
        {
            entity.ToTable("store_listings");
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

        modelBuilder.Entity<ListingInquiryRecord>(entity =>
        {
            entity.ToTable("listing_inquiries");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Status).HasConversion<int>();
            entity.Property(i => i.Message).HasMaxLength(2000);
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.TerritoryId);
            entity.HasIndex(i => i.ListingId);
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
            entity.HasIndex(i => i.ListingId);
            entity.HasIndex(i => new { i.CartId, i.ListingId }).IsUnique();
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
            entity.Property(i => i.ListingType).HasConversion<int>();
            entity.Property(i => i.TitleSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineSubtotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.PlatformFeeLine).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineTotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.CheckoutId);
            entity.HasIndex(i => i.ListingId);
        });

        modelBuilder.Entity<PlatformFeeConfigRecord>(entity =>
        {
            entity.ToTable("platform_fee_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ListingType).HasConversion<int>();
            entity.Property(c => c.FeeMode).HasConversion<int>();
            entity.Property(c => c.FeeValue).HasColumnType("numeric(18,4)");
            entity.Property(c => c.Currency).HasMaxLength(10);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.ListingType, c.IsActive }).IsUnique();
        });

        modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
            entity.Property(u => u.Provider).HasMaxLength(80).IsRequired();
            entity.Property(u => u.ExternalId).HasMaxLength(160).IsRequired();
            entity.Property(u => u.Role).HasConversion<int>();
            entity.Property(u => u.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => new { u.Provider, u.ExternalId }).IsUnique();
        });

        modelBuilder.Entity<UserTerritoryRecord>(entity =>
        {
            entity.ToTable("user_territories");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Status).HasConversion<int>();
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(m => m.UserId);
            entity.HasIndex(m => m.TerritoryId);
            entity.HasIndex(m => new { m.UserId, m.TerritoryId }).IsUnique();
        });
    }
}
