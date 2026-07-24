using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureMedia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaAssetRecord>(entity =>
        {
            entity.ToTable("media_assets");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.MediaType).HasConversion<int>().IsRequired();
            entity.Property(m => m.MimeType).HasMaxLength(100).IsRequired();
            entity.Property(m => m.StorageKey).HasMaxLength(500).IsRequired();
            entity.Property(m => m.SizeBytes).IsRequired();
            entity.Property(m => m.Checksum).HasMaxLength(64).IsRequired();
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.DeletedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(m => m.UploadedByUserId);
            entity.HasIndex(m => m.CreatedAtUtc);
            entity.HasIndex(m => m.DeletedAtUtc);
            entity.HasIndex(m => new { m.UploadedByUserId, m.DeletedAtUtc });
        });

        modelBuilder.Entity<MediaAttachmentRecord>(entity =>
        {
            entity.ToTable("media_attachments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.OwnerType).HasConversion<int>().IsRequired();
            entity.Property(a => a.DisplayOrder).IsRequired();
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => new { a.OwnerType, a.OwnerId });
            entity.HasIndex(a => a.MediaAssetId);
            entity.HasIndex(a => new { a.OwnerType, a.OwnerId, a.DisplayOrder });
        });

        modelBuilder.Entity<TerritoryMediaConfigRecord>(entity =>
        {
            entity.ToTable("territory_media_configs");
            entity.HasKey(c => c.TerritoryId);
            entity.Property(c => c.PostsJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.EventsJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.MarketplaceJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.ChatJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<UserMediaPreferencesRecord>(entity =>
        {
            entity.ToTable("user_media_preferences");
            entity.HasKey(p => p.UserId);
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<MediaStorageConfigRecord>(entity =>
        {
            entity.ToTable("media_storage_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.SettingsJson).HasColumnType("jsonb").IsRequired();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.IsActive);
        });
    }
}
