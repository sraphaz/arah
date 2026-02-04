using Araponga.Modules.Assets.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Assets.Infrastructure.Postgres;

public sealed class AssetsDbContext : DbContext
{
    public AssetsDbContext(DbContextOptions<AssetsDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerritoryAssetRecord> TerritoryAssets => Set<TerritoryAssetRecord>();
    public DbSet<AssetGeoAnchorRecord> AssetGeoAnchors => Set<AssetGeoAnchorRecord>();
    public DbSet<AssetValidationRecord> AssetValidations => Set<AssetValidationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}
