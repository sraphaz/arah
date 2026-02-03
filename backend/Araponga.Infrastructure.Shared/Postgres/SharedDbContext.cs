using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Shared.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araponga.Infrastructure.Shared.Postgres;

public sealed class SharedDbContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public SharedDbContext(DbContextOptions<SharedDbContext> options)
        : base(options)
    {
    }

    // Core entities shared across all modules
    public DbSet<TerritoryRecord> Territories => Set<TerritoryRecord>();
    public DbSet<UserRecord> Users => Set<UserRecord>();
    public DbSet<UserPreferencesRecord> UserPreferences => Set<UserPreferencesRecord>();
    public DbSet<UserDeviceRecord> UserDevices => Set<UserDeviceRecord>();
    public DbSet<UserInterestRecord> UserInterests => Set<UserInterestRecord>();
    public DbSet<TerritoryMembershipRecord> TerritoryMemberships => Set<TerritoryMembershipRecord>();
    public DbSet<MembershipSettingsRecord> MembershipSettings => Set<MembershipSettingsRecord>();
    public DbSet<MembershipCapabilityRecord> MembershipCapabilities => Set<MembershipCapabilityRecord>();
    public DbSet<SystemPermissionRecord> SystemPermissions => Set<SystemPermissionRecord>();
    public DbSet<SystemConfigRecord> SystemConfigs => Set<SystemConfigRecord>();
    public DbSet<WorkItemRecord> WorkItems => Set<WorkItemRecord>();
    public DbSet<DocumentEvidenceRecord> DocumentEvidences => Set<DocumentEvidenceRecord>();
    public DbSet<TerritoryJoinRequestRecord> TerritoryJoinRequests => Set<TerritoryJoinRequestRecord>();
    public DbSet<TerritoryJoinRequestRecipientRecord> TerritoryJoinRequestRecipients => Set<TerritoryJoinRequestRecipientRecord>();
    public DbSet<VotingRecord> Votings => Set<VotingRecord>();
    public DbSet<VoteRecord> Votes => Set<VoteRecord>();
    public DbSet<TerritoryModerationRuleRecord> TerritoryModerationRules => Set<TerritoryModerationRuleRecord>();
    public DbSet<TerritoryCharacterizationRecord> TerritoryCharacterizations => Set<TerritoryCharacterizationRecord>();
    public DbSet<NotificationConfigRecord> NotificationConfigs => Set<NotificationConfigRecord>();
    public DbSet<OutboxMessageRecord> OutboxMessages => Set<OutboxMessageRecord>();
    public DbSet<UserNotificationRecord> UserNotifications => Set<UserNotificationRecord>();
    public DbSet<ActiveTerritoryRecord> ActiveTerritories => Set<ActiveTerritoryRecord>();
    public DbSet<FeatureFlagRecord> FeatureFlags => Set<FeatureFlagRecord>();
    public DbSet<AuditEntryRecord> AuditEntries => Set<AuditEntryRecord>();
    public DbSet<TermsOfServiceRecord> TermsOfServices => Set<TermsOfServiceRecord>();
    public DbSet<TermsAcceptanceRecord> TermsAcceptances => Set<TermsAcceptanceRecord>();
    public DbSet<PrivacyPolicyRecord> PrivacyPolicies => Set<PrivacyPolicyRecord>();
    public DbSet<PrivacyPolicyAcceptanceRecord> PrivacyPolicyAcceptances => Set<PrivacyPolicyAcceptanceRecord>();
    public DbSet<EmailQueueItemRecord> EmailQueueItems => Set<EmailQueueItemRecord>();

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                "Concurrency conflict detected. The entity was modified by another process. Please retry the operation.",
                ex);
        }

        if (_currentTransaction is not null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public Task<bool> HasActiveTransactionAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_currentTransaction is not null);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure shared entities
        ConfigureTerritory(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureUserPreferences(modelBuilder);
        ConfigureUserInterest(modelBuilder);
        ConfigureVoting(modelBuilder);
        ConfigureTerritoryModerationRule(modelBuilder);
        ConfigureTerritoryCharacterization(modelBuilder);
        ConfigureNotificationConfig(modelBuilder);
        ConfigureTerritoryMembership(modelBuilder);
        ConfigureMembershipSettings(modelBuilder);
        ConfigureMembershipCapability(modelBuilder);
        ConfigureSystemPermission(modelBuilder);
        ConfigureSystemConfig(modelBuilder);
        ConfigureWorkItem(modelBuilder);
        ConfigureDocumentEvidence(modelBuilder);
        ConfigureTerritoryJoinRequest(modelBuilder);
        ConfigureTerritoryJoinRequestRecipient(modelBuilder);
        ConfigureOutboxMessage(modelBuilder);
        ConfigureUserNotification(modelBuilder);
        ConfigureActiveTerritory(modelBuilder);
        ConfigureFeatureFlag(modelBuilder);
        ConfigureAuditEntry(modelBuilder);
        ConfigureTermsOfService(modelBuilder);
        ConfigureTermsAcceptance(modelBuilder);
        ConfigurePrivacyPolicy(modelBuilder);
        ConfigurePrivacyPolicyAcceptance(modelBuilder);
        ConfigureEmailQueueItem(modelBuilder);
    }

    private static void ConfigureTerritory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryRecord>(entity =>
        {
            entity.ToTable("territories");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(1000);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.City).HasMaxLength(120).IsRequired();
            entity.Property(t => t.State).HasMaxLength(60).IsRequired();
            entity.Property(t => t.Latitude).HasColumnType("double precision");
            entity.Property(t => t.Longitude).HasColumnType("double precision");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.Name);
            entity.HasIndex(t => t.City);
            entity.HasIndex(t => t.State);
            entity.HasIndex(t => new { t.City, t.State });
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(320).IsRequired();
            entity.Property(u => u.AuthProvider).HasMaxLength(80).IsRequired();
            entity.Property(u => u.ExternalId).HasMaxLength(160).IsRequired();
            entity.Property(u => u.TwoFactorSecret).HasMaxLength(500);
            entity.Property(u => u.TwoFactorRecoveryCodesHash).HasMaxLength(500);
            entity.Property(u => u.TwoFactorVerifiedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(u => u.IdentityVerificationStatus).HasConversion<int>().IsRequired();
            entity.Property(u => u.IdentityVerifiedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(u => u.AvatarMediaAssetId);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => new { u.AuthProvider, u.ExternalId }).IsUnique();
        });
    }

    private static void ConfigureUserPreferences(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureUserInterest(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureVoting(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureTerritoryModerationRule(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureTerritoryCharacterization(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerritoryCharacterizationRecord>(entity =>
        {
            entity.ToTable("territory_characterizations");
            entity.HasKey(c => c.TerritoryId);
            entity.Property(c => c.TagsJson).IsRequired();
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId).IsUnique();
        });
    }

    private static void ConfigureNotificationConfig(ModelBuilder modelBuilder)
    {
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
                .HasFilter("\"TerritoryId\" IS NULL");
        });
    }

    private static void ConfigureTerritoryMembership(ModelBuilder modelBuilder)
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
            entity.Property(m => m.RowVersion).IsRowVersion();
            entity.HasIndex(m => m.UserId);
            entity.HasIndex(m => m.TerritoryId);
            entity.HasIndex(m => new { m.UserId, m.TerritoryId }).IsUnique();
            entity.HasIndex(m => m.UserId)
                .HasFilter("\"Role\" = 1")
                .IsUnique();
        });
    }

    private static void ConfigureMembershipSettings(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureMembershipCapability(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureSystemPermission(ModelBuilder modelBuilder)
    {
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

    private static void ConfigureSystemConfig(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemConfigRecord>(entity =>
        {
            entity.ToTable("system_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Key).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Value).HasColumnType("text").IsRequired();
            entity.Property(c => c.Category).HasConversion<int>();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.Key).IsUnique();
            entity.HasIndex(c => c.Category);
        });
    }

    private static void ConfigureWorkItem(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureDocumentEvidence(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureTerritoryJoinRequest(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureTerritoryJoinRequestRecipient(ModelBuilder modelBuilder)
    {
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

    private static void ConfigureOutboxMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessageRecord>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).HasMaxLength(200).IsRequired();
            entity.Property(o => o.PayloadJson).HasColumnType("jsonb");
            entity.Property(o => o.OccurredAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.ProcessAfterUtc).HasColumnType("timestamp with time zone");
            entity.Property(o => o.Attempts).HasDefaultValue(0);
            entity.Property(o => o.LastError).HasColumnType("text");
            entity.HasIndex(o => new { o.ProcessedAtUtc, o.ProcessAfterUtc });
            entity.HasIndex(o => new { o.Type, o.ProcessedAtUtc });
        });
    }

    private static void ConfigureUserNotification(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureActiveTerritory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveTerritoryRecord>(entity =>
        {
            entity.ToTable("active_territories");
            entity.HasKey(a => a.SessionId);
            entity.Property(a => a.SessionId).HasMaxLength(200).IsRequired();
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
        });
    }

    private static void ConfigureFeatureFlag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeatureFlagRecord>(entity =>
        {
            entity.ToTable("feature_flags");
            entity.HasKey(f => new { f.TerritoryId, f.Flag });
            entity.Property(f => f.Flag).HasConversion<int>();
            entity.HasIndex(f => f.TerritoryId);
        });
    }

    private static void ConfigureAuditEntry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEntryRecord>(entity =>
        {
            entity.ToTable("audit_entries");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).HasMaxLength(200).IsRequired();
            entity.Property(a => a.TimestampUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
            entity.HasIndex(a => a.ActorUserId);
            entity.HasIndex(a => a.TimestampUtc);
        });
    }

    private static void ConfigureTermsOfService(ModelBuilder modelBuilder)
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
    }

    private static void ConfigureTermsAcceptance(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigurePrivacyPolicy(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigurePrivacyPolicyAcceptance(ModelBuilder modelBuilder)
    {
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

    private static void ConfigureEmailQueueItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailQueueItemRecord>(entity =>
        {
            entity.ToTable("email_queue_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.To).HasMaxLength(320).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Body).HasColumnType("text").IsRequired();
            entity.Property(e => e.Status).HasConversion<int>().IsRequired();
            entity.Property(e => e.Priority).HasConversion<int>().IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.ProcessedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.SentAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.FailedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.Attempts).HasDefaultValue(0);
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.Status, e.Priority, e.CreatedAtUtc });
        });
    }
}
