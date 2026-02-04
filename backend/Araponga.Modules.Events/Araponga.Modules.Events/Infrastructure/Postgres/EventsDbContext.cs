using Araponga.Modules.Events.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Events.Infrastructure.Postgres;

public sealed class EventsDbContext : DbContext
{
    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerritoryEventRecord> TerritoryEvents => Set<TerritoryEventRecord>();
    public DbSet<EventParticipationRecord> EventParticipations => Set<EventParticipationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.RowVersion).IsRowVersion();
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
    }
}
