using Araponga.Modules.Map.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Map.Infrastructure.Postgres;

public sealed class MapDbContext : DbContext
{
    public MapDbContext(DbContextOptions<MapDbContext> options)
        : base(options)
    {
    }

    public DbSet<MapEntityRecord> MapEntities => Set<MapEntityRecord>();
    public DbSet<MapEntityRelationRecord> MapEntityRelations => Set<MapEntityRelationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.RowVersion).IsRowVersion();
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
    }
}
