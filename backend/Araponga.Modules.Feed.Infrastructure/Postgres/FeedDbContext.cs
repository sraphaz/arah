using Araponga.Modules.Feed.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Feed.Infrastructure.Postgres;

public sealed class FeedDbContext : DbContext
{
    public FeedDbContext(DbContextOptions<FeedDbContext> options)
        : base(options)
    {
    }

    public DbSet<CommunityPostRecord> CommunityPosts => Set<CommunityPostRecord>();
    public DbSet<PostCommentRecord> PostComments => Set<PostCommentRecord>();
    public DbSet<PostLikeRecord> PostLikes => Set<PostLikeRecord>();
    public DbSet<PostShareRecord> PostShares => Set<PostShareRecord>();
    public DbSet<PostGeoAnchorRecord> PostGeoAnchors => Set<PostGeoAnchorRecord>();
    public DbSet<PostAssetRecord> PostAssets => Set<PostAssetRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(p => p.TagsJson).HasColumnType("jsonb");
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.RowVersion).IsRowVersion();
            entity.HasIndex(p => p.TerritoryId);
            entity.HasIndex(p => new { p.TerritoryId, p.CreatedAtUtc });
            entity.HasIndex(p => new { p.TerritoryId, p.Status, p.CreatedAtUtc });
            entity.HasIndex(p => p.AuthorUserId);
            entity.HasIndex(p => p.MapEntityId);
            entity.HasIndex(p => new { p.ReferenceType, p.ReferenceId });
            entity.HasIndex(p => p.TagsJson)
                .HasMethod("gin")
                .HasFilter("\"TagsJson\" IS NOT NULL");
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

        modelBuilder.Entity<PostAssetRecord>(entity =>
        {
            entity.ToTable("post_assets");
            entity.HasKey(p => new { p.PostId, p.AssetId });
            entity.HasIndex(p => p.AssetId);
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
    }
}
