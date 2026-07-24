using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureSocial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryJoinRequestRecord>(entity =>
        {
            entity.ToTable("territory_join_requests");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).HasConversion<string>();
            entity.Property(r => r.Status).HasMaxLength(32);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.ExpiresAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.DecidedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.TerritoryId);
            entity.HasIndex(r => r.RequesterUserId);
            entity.HasIndex(r => new { r.TerritoryId, r.RequesterUserId })
                .HasFilter("\"Status\" = 'Pending'");
        });

        modelBuilder.Entity<TerritoryJoinRequestRecipientRecord>(entity =>
        {
            entity.ToTable("territory_join_request_recipients");
            entity.HasKey(r => new { r.JoinRequestId, r.RecipientUserId });
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.RespondedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.JoinRequestId);
            entity.HasIndex(r => r.RecipientUserId);
        });
    }
}
