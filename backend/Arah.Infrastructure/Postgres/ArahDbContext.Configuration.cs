using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemConfigRecord>(entity =>
        {
            entity.ToTable("system_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Key).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Value).HasColumnType("text").IsRequired();
            entity.Property(c => c.Category).HasConversion<int>();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.Key).IsUnique();
            entity.HasIndex(c => c.Category);
        });

        modelBuilder.Entity<FeatureFlagRecord>(entity =>
        {
            entity.ToTable("feature_flags");
            entity.HasKey(f => new { f.TerritoryId, f.Flag });
            entity.Property(f => f.Flag).HasConversion<int>();
            entity.HasIndex(f => f.TerritoryId);
        });
    }
}
