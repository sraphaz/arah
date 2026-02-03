using Araponga.Modules.Alerts.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Alerts.Infrastructure.Postgres;

public sealed class AlertsDbContext : DbContext
{
    public AlertsDbContext(DbContextOptions<AlertsDbContext> options)
        : base(options)
    {
    }

    public DbSet<HealthAlertRecord> HealthAlerts => Set<HealthAlertRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}
