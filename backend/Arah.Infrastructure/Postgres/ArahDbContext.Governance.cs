using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext
{
    private static void ConfigureGovernance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VotingRecord>(entity =>
        {
            entity.ToTable("votings");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Type).HasConversion<int>().IsRequired();
            entity.Property(v => v.Visibility).HasConversion<int>().IsRequired();
            entity.Property(v => v.Status).HasConversion<int>().IsRequired();
            entity.Property(v => v.Title).HasMaxLength(200).IsRequired();
            entity.Property(v => v.Description).HasMaxLength(2000).IsRequired();
            entity.Property(v => v.OptionsJson).IsRequired();
            entity.Property(v => v.StartsAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(v => v.EndsAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(v => v.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(v => v.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(v => v.TerritoryId);
            entity.HasIndex(v => v.CreatedByUserId);
            entity.HasIndex(v => v.Status);
            entity.HasIndex(v => new { v.TerritoryId, v.Status });
        });

        modelBuilder.Entity<VoteRecord>(entity =>
        {
            entity.ToTable("votes");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.SelectedOption).HasMaxLength(200).IsRequired();
            entity.Property(v => v.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(v => v.VotingId);
            entity.HasIndex(v => v.UserId);
            entity.HasIndex(v => new { v.VotingId, v.UserId }).IsUnique();
            entity.HasOne<VotingRecord>()
                .WithMany()
                .HasForeignKey(v => v.VotingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TerritoryModerationRuleRecord>(entity =>
        {
            entity.ToTable("territory_moderation_rules");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RuleType).HasConversion<int>().IsRequired();
            entity.Property(r => r.RuleJson).IsRequired();
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.TerritoryId);
            entity.HasIndex(r => r.CreatedByVotingId);
            entity.HasIndex(r => new { r.TerritoryId, r.IsActive });
        });
    }
}
