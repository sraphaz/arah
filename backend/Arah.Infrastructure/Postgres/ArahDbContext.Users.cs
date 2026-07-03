using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
            entity.Property(u => u.AuthProvider).HasMaxLength(80).IsRequired();
            entity.Property(u => u.ExternalId).HasMaxLength(160).IsRequired();
            // 2FA fields
            entity.Property(u => u.TwoFactorSecret).HasMaxLength(500);
            entity.Property(u => u.TwoFactorRecoveryCodesHash).HasMaxLength(500);
            entity.Property(u => u.TwoFactorVerifiedAtUtc).HasColumnType("timestamp with time zone");
            // Identity verification fields
            entity.Property(u => u.IdentityVerificationStatus).HasConversion<int>().IsRequired();
            entity.Property(u => u.IdentityVerifiedAtUtc).HasColumnType("timestamp with time zone");
            // Profile fields
            entity.Property(u => u.AvatarMediaAssetId);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.PasswordHash).HasMaxLength(500);
            entity.Property(u => u.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => new { u.AuthProvider, u.ExternalId }).IsUnique();
        });

        modelBuilder.Entity<UserPreferencesRecord>(entity =>
        {
            entity.ToTable("user_preferences");
            entity.HasKey(p => p.UserId);
            entity.Property(p => p.ProfileVisibility).HasConversion<int>().IsRequired();
            entity.Property(p => p.ContactVisibility).HasConversion<int>().IsRequired();
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne<UserRecord>()
                .WithOne()
                .HasForeignKey<UserPreferencesRecord>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(p => p.UserId).IsUnique();
        });

        modelBuilder.Entity<UserInterestRecord>(entity =>
        {
            entity.ToTable("user_interests");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.InterestTag).HasMaxLength(50).IsRequired();
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.UserId);
            entity.HasIndex(i => i.InterestTag);
            entity.HasIndex(i => new { i.UserId, i.InterestTag }).IsUnique();
            entity.HasOne<UserRecord>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserDeviceRecord>(entity =>
        {
            entity.ToTable("user_devices");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.UserId).IsRequired();
            entity.Property(d => d.DeviceToken).HasMaxLength(500).IsRequired();
            entity.Property(d => d.Platform).HasMaxLength(50).IsRequired();
            entity.Property(d => d.DeviceName).HasMaxLength(200);
            entity.Property(d => d.RegisteredAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(d => d.LastUsedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(d => d.IsActive).IsRequired();
            entity.HasIndex(d => d.UserId);
            entity.HasIndex(d => d.DeviceToken).IsUnique();
            entity.HasIndex(d => new { d.UserId, d.IsActive });
        });
    }
}
