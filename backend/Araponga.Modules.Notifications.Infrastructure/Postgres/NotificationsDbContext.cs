using Araponga.Modules.Notifications.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Notifications.Infrastructure.Postgres;

public sealed class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserNotificationRecord> UserNotifications => Set<UserNotificationRecord>();
    public DbSet<NotificationConfigRecord> NotificationConfigs => Set<NotificationConfigRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<NotificationConfigRecord>(entity =>
        {
            entity.ToTable("notification_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.NotificationTypesJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.AvailableChannelsJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.TemplatesJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.DefaultChannelsJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId).IsUnique()
                .HasFilter("\"TerritoryId\" IS NOT NULL");
            entity.HasIndex(c => new { c.TerritoryId })
                .IsUnique()
                .HasFilter("\"TerritoryId\" IS NULL"); // Índice único para config global
        });
    }
}
