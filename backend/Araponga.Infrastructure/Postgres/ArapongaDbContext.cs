using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class ArapongaDbContext : DbContext, IUnitOfWork
{
    public ArapongaDbContext(DbContextOptions<ArapongaDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerritoryRecord> Territories => Set<TerritoryRecord>();
    public DbSet<UserRecord> Users => Set<UserRecord>();
    public DbSet<TerritoryMembershipRecord> TerritoryMemberships => Set<TerritoryMembershipRecord>();
    public DbSet<UserTerritoryRecord> UserTerritories => Set<UserTerritoryRecord>();
    public DbSet<CommunityPostRecord> CommunityPosts => Set<CommunityPostRecord>();
    public DbSet<PostCommentRecord> PostComments => Set<PostCommentRecord>();
    public DbSet<MapEntityRecord> MapEntities => Set<MapEntityRecord>();
    public DbSet<MapEntityRelationRecord> MapEntityRelations => Set<MapEntityRelationRecord>();
    public DbSet<PostGeoAnchorRecord> PostGeoAnchors => Set<PostGeoAnchorRecord>();
    public DbSet<HealthAlertRecord> HealthAlerts => Set<HealthAlertRecord>();
    public DbSet<PostLikeRecord> PostLikes => Set<PostLikeRecord>();
    public DbSet<PostShareRecord> PostShares => Set<PostShareRecord>();
    public DbSet<ActiveTerritoryRecord> ActiveTerritories => Set<ActiveTerritoryRecord>();
    public DbSet<FeatureFlagRecord> FeatureFlags => Set<FeatureFlagRecord>();
    public DbSet<AuditEntryRecord> AuditEntries => Set<AuditEntryRecord>();
    public DbSet<ModerationReportRecord> ModerationReports => Set<ModerationReportRecord>();
    public DbSet<UserBlockRecord> UserBlocks => Set<UserBlockRecord>();
    public DbSet<SanctionRecord> Sanctions => Set<SanctionRecord>();

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryRecord>(entity =>
        {
            entity.ToTable("territories");
            entity.HasKey(t => t.Id);
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
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(p => p.TerritoryId);
            entity.HasIndex(p => new { p.TerritoryId, p.CreatedAtUtc });
            entity.HasIndex(p => p.AuthorUserId);
            entity.HasIndex(p => p.MapEntityId);
        });

        modelBuilder.Entity<PostCommentRecord>(entity =>
        {
            entity.ToTable("post_comments");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.PostId);
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

        modelBuilder.Entity<ActiveTerritoryRecord>(entity =>
        {
            entity.ToTable("active_territories");
            entity.HasKey(a => a.SessionId);
            entity.Property(a => a.SessionId).HasMaxLength(200).IsRequired();
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
        });

        modelBuilder.Entity<FeatureFlagRecord>(entity =>
        {
            entity.ToTable("feature_flags");
            entity.HasKey(f => new { f.TerritoryId, f.Flag });
            entity.Property(f => f.Flag).HasConversion<int>();
            entity.HasIndex(f => f.TerritoryId);
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

        modelBuilder.Entity<UserBlockRecord>(entity =>
        {
            entity.ToTable("user_blocks");
            entity.HasKey(b => new { b.BlockerUserId, b.BlockedUserId });
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.BlockerUserId);
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
    }
}
