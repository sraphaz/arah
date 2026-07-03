using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureMembership(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryMembershipRecord>(entity =>
        {
            entity.ToTable("territory_memberships");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Role).HasConversion<int>();
            entity.Property(m => m.ResidencyVerification).HasConversion<int>();
            entity.Property(m => m.LastGeoVerifiedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.LastDocumentVerifiedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            // DEFAULT no banco (migração 20260207000000); EF não envia no INSERT para evitar NULL.
            entity.Property(m => m.RowVersion)
                .HasDefaultValueSql("gen_random_bytes(8)")
                .ValueGeneratedOnAdd();
            entity.HasIndex(m => m.UserId);
            entity.HasIndex(m => m.TerritoryId);
            entity.HasIndex(m => new { m.UserId, m.TerritoryId }).IsUnique();
            // Índice único parcial: 1 Resident por User (Role=2). Visitor (1) pode ter vários territórios.
            entity.HasIndex(m => m.UserId)
                .HasFilter("\"Role\" = 2") // MembershipRole.Resident = 2
                .IsUnique();
        });

        modelBuilder.Entity<MembershipSettingsRecord>(entity =>
        {
            entity.ToTable("membership_settings");
            entity.HasKey(s => s.MembershipId);
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne<TerritoryMembershipRecord>()
                .WithOne()
                .HasForeignKey<MembershipSettingsRecord>(s => s.MembershipId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(s => s.MembershipId).IsUnique();
        });

        modelBuilder.Entity<MembershipCapabilityRecord>(entity =>
        {
            entity.ToTable("membership_capabilities");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CapabilityType).HasConversion<int>();
            entity.Property(c => c.GrantedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.RevokedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.Reason).HasMaxLength(500);
            entity.HasOne<TerritoryMembershipRecord>()
                .WithMany()
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(c => c.MembershipId);
            entity.HasIndex(c => new { c.MembershipId, c.CapabilityType })
                .HasFilter("\"RevokedAtUtc\" IS NULL");
        });

        modelBuilder.Entity<SystemPermissionRecord>(entity =>
        {
            entity.ToTable("system_permissions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.PermissionType).HasConversion<int>();
            entity.Property(p => p.GrantedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.RevokedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne<UserRecord>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => new { p.UserId, p.PermissionType })
                .IsUnique()
                .HasFilter("\"RevokedAtUtc\" IS NULL");
            entity.HasIndex(p => p.PermissionType)
                .HasFilter("\"RevokedAtUtc\" IS NULL");
        });
    }
}
