using Araponga.Modules.Moderation.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Moderation.Infrastructure.Postgres;

public sealed class ModerationDbContext : DbContext
{
    public ModerationDbContext(DbContextOptions<ModerationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ModerationReportRecord> ModerationReports => Set<ModerationReportRecord>();
    public DbSet<SanctionRecord> Sanctions => Set<SanctionRecord>();
    public DbSet<WorkItemRecord> WorkItems => Set<WorkItemRecord>();
    public DbSet<DocumentEvidenceRecord> DocumentEvidences => Set<DocumentEvidenceRecord>();
    public DbSet<TerritoryModerationRuleRecord> TerritoryModerationRules => Set<TerritoryModerationRuleRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<WorkItemRecord>(entity =>
        {
            entity.ToTable("work_items");
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Type).HasConversion<int>();
            entity.Property(w => w.Status).HasConversion<int>();
            entity.Property(w => w.RequiredSystemPermission).HasConversion<int>();
            entity.Property(w => w.RequiredCapability).HasConversion<int>();
            entity.Property(w => w.Outcome).HasConversion<int>();
            entity.Property(w => w.SubjectType).HasMaxLength(50).IsRequired();
            entity.Property(w => w.PayloadJson).HasColumnType("text");
            entity.Property(w => w.CompletionNotes).HasMaxLength(2000);
            entity.Property(w => w.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(w => w.CompletedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(w => w.Status);
            entity.HasIndex(w => w.Type);
            entity.HasIndex(w => new { w.TerritoryId, w.Status });
            entity.HasIndex(w => new { w.SubjectType, w.SubjectId });
        });

        modelBuilder.Entity<DocumentEvidenceRecord>(entity =>
        {
            entity.ToTable("document_evidences");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Kind).HasConversion<int>();
            entity.Property(d => d.StorageProvider).HasConversion<int>();
            entity.Property(d => d.StorageKey).HasMaxLength(500).IsRequired();
            entity.Property(d => d.ContentType).HasMaxLength(200).IsRequired();
            entity.Property(d => d.OriginalFileName).HasMaxLength(300);
            entity.Property(d => d.Sha256).HasMaxLength(64).IsRequired();
            entity.Property(d => d.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(d => d.UserId);
            entity.HasIndex(d => new { d.TerritoryId, d.Kind });
        });

        modelBuilder.Entity<ModerationReportRecord>(entity =>
        {
            entity.ToTable("moderation_reports");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.TerritoryId).IsRequired();
            entity.Property(r => r.TargetType).HasConversion<int>();
            entity.Property(r => r.Reason).HasMaxLength(300).IsRequired();
            entity.Property(r => r.Details).HasMaxLength(2000);
            entity.Property(r => r.Status).HasConversion<int>();
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.ReporterUserId);
            entity.HasIndex(r => new { r.TargetType, r.TargetId });
            entity.HasIndex(r => new { r.TargetType, r.TargetId, r.CreatedAtUtc });
            entity.HasIndex(r => new { r.TerritoryId, r.CreatedAtUtc });
            entity.HasIndex(r => r.CreatedAtUtc);
        });

        modelBuilder.Entity<SanctionRecord>(entity =>
        {
            entity.ToTable("sanctions");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Scope).HasConversion<int>();
            entity.Property(s => s.TargetType).HasConversion<int>();
            entity.Property(s => s.Type).HasConversion<int>();
            entity.Property(s => s.Status).HasConversion<int>();
            entity.Property(s => s.Reason).HasMaxLength(400).IsRequired();
            entity.Property(s => s.StartAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.EndAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.TargetId);
            entity.HasIndex(s => s.TerritoryId);
        });
    }
}
