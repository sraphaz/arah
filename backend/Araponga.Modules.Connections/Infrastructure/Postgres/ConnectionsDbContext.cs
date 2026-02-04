using Araponga.Modules.Connections.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Connections.Infrastructure.Postgres;

public sealed class ConnectionsDbContext : DbContext
{
    public ConnectionsDbContext(DbContextOptions<ConnectionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserConnectionRecord> UserConnections => Set<UserConnectionRecord>();
    public DbSet<ConnectionPrivacySettingsRecord> ConnectionPrivacySettings => Set<ConnectionPrivacySettingsRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserConnectionRecord>(entity =>
        {
            entity.ToTable("user_connections");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Status).HasConversion<int>().IsRequired();
            entity.Property(c => c.RequestedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.RespondedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.RemovedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.RequesterUserId);
            entity.HasIndex(c => c.TargetUserId);
            entity.HasIndex(c => new { c.RequesterUserId, c.TargetUserId }).IsUnique();
            entity.HasIndex(c => c.Status);
        });

        modelBuilder.Entity<ConnectionPrivacySettingsRecord>(entity =>
        {
            entity.ToTable("connection_privacy_settings");
            entity.HasKey(s => s.UserId);
            entity.Property(s => s.WhoCanAddMe).HasConversion<int>().IsRequired();
            entity.Property(s => s.WhoCanSeeMyConnections).HasConversion<int>().IsRequired();
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.UserId).IsUnique();
        });
    }
}
