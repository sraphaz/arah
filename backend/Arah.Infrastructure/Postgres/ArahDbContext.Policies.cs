using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigurePolicies(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TermsOfServiceRecord>(entity =>
        {
            entity.ToTable("terms_of_services");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Version).HasMaxLength(50).IsRequired();
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Content).HasColumnType("text").IsRequired();
            entity.Property(t => t.EffectiveDate).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(t => t.ExpirationDate).HasColumnType("timestamp with time zone");
            entity.Property(t => t.IsActive).IsRequired();
            entity.Property(t => t.RequiredRoles).HasColumnType("jsonb");
            entity.Property(t => t.RequiredCapabilities).HasColumnType("jsonb");
            entity.Property(t => t.RequiredSystemPermissions).HasColumnType("jsonb");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(t => t.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(t => t.Version);
            entity.HasIndex(t => t.IsActive);
            entity.HasIndex(t => t.EffectiveDate);
        });

        modelBuilder.Entity<TermsAcceptanceRecord>(entity =>
        {
            entity.ToTable("terms_acceptances");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.TermsOfServiceId).IsRequired();
            entity.Property(a => a.AcceptedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(a => a.IpAddress).HasMaxLength(45);
            entity.Property(a => a.UserAgent).HasMaxLength(500);
            entity.Property(a => a.AcceptedVersion).HasMaxLength(50).IsRequired();
            entity.Property(a => a.IsRevoked).IsRequired();
            entity.Property(a => a.RevokedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.TermsOfServiceId);
            entity.HasIndex(a => new { a.UserId, a.TermsOfServiceId });
            entity.HasIndex(a => new { a.UserId, a.TermsOfServiceId, a.IsRevoked });
        });

        modelBuilder.Entity<PrivacyPolicyRecord>(entity =>
        {
            entity.ToTable("privacy_policies");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Version).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Content).HasColumnType("text").IsRequired();
            entity.Property(p => p.EffectiveDate).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.ExpirationDate).HasColumnType("timestamp with time zone");
            entity.Property(p => p.IsActive).IsRequired();
            entity.Property(p => p.RequiredRoles).HasColumnType("jsonb");
            entity.Property(p => p.RequiredCapabilities).HasColumnType("jsonb");
            entity.Property(p => p.RequiredSystemPermissions).HasColumnType("jsonb");
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(p => p.UpdatedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.HasIndex(p => p.Version);
            entity.HasIndex(p => p.IsActive);
            entity.HasIndex(p => p.EffectiveDate);
        });

        modelBuilder.Entity<PrivacyPolicyAcceptanceRecord>(entity =>
        {
            entity.ToTable("privacy_policy_acceptances");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.PrivacyPolicyId).IsRequired();
            entity.Property(a => a.AcceptedAtUtc).HasColumnType("timestamp with time zone").IsRequired();
            entity.Property(a => a.IpAddress).HasMaxLength(45);
            entity.Property(a => a.UserAgent).HasMaxLength(500);
            entity.Property(a => a.AcceptedVersion).HasMaxLength(50).IsRequired();
            entity.Property(a => a.IsRevoked).IsRequired();
            entity.Property(a => a.RevokedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.PrivacyPolicyId);
            entity.HasIndex(a => new { a.UserId, a.PrivacyPolicyId });
            entity.HasIndex(a => new { a.UserId, a.PrivacyPolicyId, a.IsRevoked });
        });
    }
}
