using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araponga.Infrastructure.Postgres;

public sealed class ArapongaDbContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public ArapongaDbContext(DbContextOptions<ArapongaDbContext> options)
        : base(options)
    {
    }

    public DbSet<TerritoryRecord> Territories => Set<TerritoryRecord>();
    public DbSet<UserRecord> Users => Set<UserRecord>();
    public DbSet<UserPreferencesRecord> UserPreferences => Set<UserPreferencesRecord>();
    public DbSet<UserDeviceRecord> UserDevices => Set<UserDeviceRecord>();
    public DbSet<UserInterestRecord> UserInterests => Set<UserInterestRecord>();
    public DbSet<VotingRecord> Votings => Set<VotingRecord>();
    public DbSet<VoteRecord> Votes => Set<VoteRecord>();
    public DbSet<TerritoryModerationRuleRecord> TerritoryModerationRules => Set<TerritoryModerationRuleRecord>();
    public DbSet<TerritoryCharacterizationRecord> TerritoryCharacterizations => Set<TerritoryCharacterizationRecord>();
    public DbSet<NotificationConfigRecord> NotificationConfigs => Set<NotificationConfigRecord>();
    public DbSet<TerritoryMembershipRecord> TerritoryMemberships => Set<TerritoryMembershipRecord>();
    public DbSet<MembershipSettingsRecord> MembershipSettings => Set<MembershipSettingsRecord>();
    public DbSet<MembershipCapabilityRecord> MembershipCapabilities => Set<MembershipCapabilityRecord>();
    public DbSet<SystemPermissionRecord> SystemPermissions => Set<SystemPermissionRecord>();
    public DbSet<SystemConfigRecord> SystemConfigs => Set<SystemConfigRecord>();
    public DbSet<WorkItemRecord> WorkItems => Set<WorkItemRecord>();
    public DbSet<DocumentEvidenceRecord> DocumentEvidences => Set<DocumentEvidenceRecord>();
    public DbSet<TerritoryJoinRequestRecord> TerritoryJoinRequests => Set<TerritoryJoinRequestRecord>();
    public DbSet<TerritoryJoinRequestRecipientRecord> TerritoryJoinRequestRecipients => Set<TerritoryJoinRequestRecipientRecord>();
    public DbSet<CommunityPostRecord> CommunityPosts => Set<CommunityPostRecord>();
    public DbSet<PostCommentRecord> PostComments => Set<PostCommentRecord>();
    public DbSet<TerritoryEventRecord> TerritoryEvents => Set<TerritoryEventRecord>();
    public DbSet<EventParticipationRecord> EventParticipations => Set<EventParticipationRecord>();
    public DbSet<MapEntityRecord> MapEntities => Set<MapEntityRecord>();
    public DbSet<MapEntityRelationRecord> MapEntityRelations => Set<MapEntityRelationRecord>();
    public DbSet<PostGeoAnchorRecord> PostGeoAnchors => Set<PostGeoAnchorRecord>();
    public DbSet<HealthAlertRecord> HealthAlerts => Set<HealthAlertRecord>();
    public DbSet<PostLikeRecord> PostLikes => Set<PostLikeRecord>();
    public DbSet<PostShareRecord> PostShares => Set<PostShareRecord>();
    public DbSet<TerritoryAssetRecord> TerritoryAssets => Set<TerritoryAssetRecord>();
    public DbSet<AssetGeoAnchorRecord> AssetGeoAnchors => Set<AssetGeoAnchorRecord>();
    public DbSet<AssetValidationRecord> AssetValidations => Set<AssetValidationRecord>();
    public DbSet<PostAssetRecord> PostAssets => Set<PostAssetRecord>();
    public DbSet<ActiveTerritoryRecord> ActiveTerritories => Set<ActiveTerritoryRecord>();
    public DbSet<FeatureFlagRecord> FeatureFlags => Set<FeatureFlagRecord>();
    public DbSet<AuditEntryRecord> AuditEntries => Set<AuditEntryRecord>();
    public DbSet<ModerationReportRecord> ModerationReports => Set<ModerationReportRecord>();
    public DbSet<UserBlockRecord> UserBlocks => Set<UserBlockRecord>();
    public DbSet<SanctionRecord> Sanctions => Set<SanctionRecord>();
    public DbSet<OutboxMessageRecord> OutboxMessages => Set<OutboxMessageRecord>();
    public DbSet<UserNotificationRecord> UserNotifications => Set<UserNotificationRecord>();
    public DbSet<TerritoryStoreRecord> TerritoryStores => Set<TerritoryStoreRecord>();
    public DbSet<StoreItemRecord> StoreItems => Set<StoreItemRecord>();
    public DbSet<ItemInquiryRecord> ItemInquiries => Set<ItemInquiryRecord>();
    public DbSet<CartRecord> Carts => Set<CartRecord>();
    public DbSet<CartItemRecord> CartItems => Set<CartItemRecord>();
    public DbSet<CheckoutRecord> Checkouts => Set<CheckoutRecord>();
    public DbSet<CheckoutItemRecord> CheckoutItems => Set<CheckoutItemRecord>();
    public DbSet<StoreRatingRecord> StoreRatings => Set<StoreRatingRecord>();
    public DbSet<StoreItemRatingRecord> StoreItemRatings => Set<StoreItemRatingRecord>();
    public DbSet<StoreRatingResponseRecord> StoreRatingResponses => Set<StoreRatingResponseRecord>();
    public DbSet<PlatformFeeConfigRecord> PlatformFeeConfigs => Set<PlatformFeeConfigRecord>();
    public DbSet<TerritoryPayoutConfigRecord> TerritoryPayoutConfigs => Set<TerritoryPayoutConfigRecord>();

    // Financial
    public DbSet<FinancialTransactionRecord> FinancialTransactions => Set<FinancialTransactionRecord>();
    public DbSet<TransactionStatusHistoryRecord> TransactionStatusHistories => Set<TransactionStatusHistoryRecord>();
    public DbSet<SellerBalanceRecord> SellerBalances => Set<SellerBalanceRecord>();
    public DbSet<SellerTransactionRecord> SellerTransactions => Set<SellerTransactionRecord>();
    public DbSet<PlatformFinancialBalanceRecord> PlatformFinancialBalances => Set<PlatformFinancialBalanceRecord>();
    public DbSet<PlatformRevenueTransactionRecord> PlatformRevenueTransactions => Set<PlatformRevenueTransactionRecord>();
    public DbSet<PlatformExpenseTransactionRecord> PlatformExpenseTransactions => Set<PlatformExpenseTransactionRecord>();
    public DbSet<ReconciliationRecordRecord> ReconciliationRecords => Set<ReconciliationRecordRecord>();

    // Chat
    public DbSet<ChatConversationRecord> ChatConversations => Set<ChatConversationRecord>();
    public DbSet<ChatConversationParticipantRecord> ChatConversationParticipants => Set<ChatConversationParticipantRecord>();
    public DbSet<ChatMessageRecord> ChatMessages => Set<ChatMessageRecord>();
    public DbSet<ChatConversationStatsRecord> ChatConversationStats => Set<ChatConversationStatsRecord>();

    // Media
    public DbSet<MediaAssetRecord> MediaAssets => Set<MediaAssetRecord>();
    public DbSet<MediaAttachmentRecord> MediaAttachments => Set<MediaAttachmentRecord>();

    // Policies
    public DbSet<TermsOfServiceRecord> TermsOfServices => Set<TermsOfServiceRecord>();
    public DbSet<TermsAcceptanceRecord> TermsAcceptances => Set<TermsAcceptanceRecord>();
    public DbSet<PrivacyPolicyRecord> PrivacyPolicies => Set<PrivacyPolicyRecord>();
    public DbSet<PrivacyPolicyAcceptanceRecord> PrivacyPolicyAcceptances => Set<PrivacyPolicyAcceptanceRecord>();

    // Email
    public DbSet<EmailQueueItemRecord> EmailQueueItems => Set<EmailQueueItemRecord>();

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Salva mudanças primeiro
            await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Re-throw com mensagem mais clara
            throw new InvalidOperationException(
                "Concurrency conflict detected. The entity was modified by another process. Please retry the operation.",
                ex);
        }

        // Se há uma transação ativa, faz commit da transação
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

        modelBuilder.Entity<TerritoryCharacterizationRecord>(entity =>
        {
            entity.ToTable("territory_characterizations");
            entity.HasKey(c => c.TerritoryId);
            entity.Property(c => c.TagsJson).IsRequired();
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId).IsUnique();
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
            // Índice único parcial para garantir 1 Resident por User
            entity.HasIndex(m => m.UserId)
                .HasFilter("\"Role\" = 1") // MembershipRole.Resident = 1
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

        modelBuilder.Entity<CommunityPostRecord>(entity =>
        {
            entity.ToTable("community_posts");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.AuthorUserId).IsRequired();
            entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Content).HasMaxLength(4000).IsRequired();
            entity.Property(p => p.Type).HasConversion<int>();
            entity.Property(p => p.Visibility).HasConversion<int>();
            entity.Property(p => p.Status).HasConversion<int>();
            entity.Property(p => p.ReferenceType).HasMaxLength(40);
            entity.Property(p => p.TagsJson).HasColumnType("jsonb"); // JSONB para busca eficiente
            entity.Property(p => p.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.RowVersion).IsRowVersion();
            entity.HasIndex(p => p.TerritoryId);
            entity.HasIndex(p => new { p.TerritoryId, p.CreatedAtUtc });
            entity.HasIndex(p => new { p.TerritoryId, p.Status, p.CreatedAtUtc });
            entity.HasIndex(p => p.AuthorUserId);
            entity.HasIndex(p => p.MapEntityId);
            entity.HasIndex(p => new { p.ReferenceType, p.ReferenceId });
            // Índice GIN para busca eficiente em tags JSONB
            entity.HasIndex(p => p.TagsJson)
                .HasMethod("gin")
                .HasFilter("\"TagsJson\" IS NOT NULL");
        });

        modelBuilder.Entity<PostCommentRecord>(entity =>
        {
            entity.ToTable("post_comments");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Content).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.PostId);
        });

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

        modelBuilder.Entity<MapEntityRecord>(entity =>
        {
            entity.ToTable("map_entities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedByUserId).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Latitude).HasColumnType("double precision");
            entity.Property(e => e.Longitude).HasColumnType("double precision");
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Visibility).HasConversion<int>();
            entity.Property(e => e.ConfirmationCount).IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasIndex(e => e.TerritoryId);
            entity.HasIndex(e => new { e.TerritoryId, e.Status });
            entity.HasIndex(e => new { e.TerritoryId, e.Visibility });
            entity.HasIndex(e => e.CreatedByUserId);
        });

        modelBuilder.Entity<MapEntityRelationRecord>(entity =>
        {
            entity.ToTable("map_entity_relations");
            entity.HasKey(r => new { r.UserId, r.EntityId });
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.EntityId);
        });

        modelBuilder.Entity<PostGeoAnchorRecord>(entity =>
        {
            entity.ToTable("post_geo_anchors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Type).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Latitude).HasColumnType("double precision");
            entity.Property(a => a.Longitude).HasColumnType("double precision");
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.PostId);
        });

        modelBuilder.Entity<HealthAlertRecord>(entity =>
        {
            entity.ToTable("health_alerts");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Title).HasMaxLength(200).IsRequired();
            entity.Property(h => h.Description).HasMaxLength(4000).IsRequired();
            entity.Property(h => h.Status).HasConversion<int>();
            entity.Property(h => h.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(h => h.TerritoryId);
            entity.HasIndex(h => new { h.TerritoryId, h.Status });
        });

        modelBuilder.Entity<TerritoryAssetRecord>(entity =>
        {
            entity.ToTable("territory_assets");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Type).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Name).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(1000);
            entity.Property(a => a.Status).HasConversion<int>();
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.ArchivedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(a => a.ArchiveReason).HasMaxLength(500);
            entity.HasIndex(a => a.TerritoryId);
            entity.HasIndex(a => new { a.TerritoryId, a.Status });
            entity.HasIndex(a => new { a.TerritoryId, a.Type });
        });

        modelBuilder.Entity<AssetGeoAnchorRecord>(entity =>
        {
            entity.ToTable("asset_geo_anchors");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Latitude).HasColumnType("double precision");
            entity.Property(a => a.Longitude).HasColumnType("double precision");
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.AssetId);
        });

        modelBuilder.Entity<AssetValidationRecord>(entity =>
        {
            entity.ToTable("asset_validations");
            entity.HasKey(a => new { a.AssetId, a.UserId });
            entity.Property(a => a.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.UserId);
        });

        modelBuilder.Entity<PostAssetRecord>(entity =>
        {
            entity.ToTable("post_assets");
            entity.HasKey(p => new { p.PostId, p.AssetId });
            entity.HasIndex(p => p.AssetId);
        });

        modelBuilder.Entity<PostLikeRecord>(entity =>
        {
            entity.ToTable("post_likes");
            entity.HasKey(l => new { l.PostId, l.ActorId });
            entity.Property(l => l.ActorId).HasMaxLength(160).IsRequired();
            entity.Property(l => l.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(l => l.PostId);
        });

        modelBuilder.Entity<PostShareRecord>(entity =>
        {
            entity.ToTable("post_shares");
            entity.HasKey(s => new { s.PostId, s.UserId });
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.PostId);
        });

        modelBuilder.Entity<ActiveTerritoryRecord>(entity =>
        {
            entity.ToTable("active_territories");
            entity.HasKey(a => a.SessionId);
            entity.Property(a => a.SessionId).HasMaxLength(200).IsRequired();
            entity.Property(a => a.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(a => a.TerritoryId);
        });

        modelBuilder.Entity<FeatureFlagRecord>(entity =>
        {
            entity.ToTable("feature_flags");
            entity.HasKey(f => new { f.TerritoryId, f.Flag });
            entity.Property(f => f.Flag).HasConversion<int>();
            entity.HasIndex(f => f.TerritoryId);
        });

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

        modelBuilder.Entity<UserBlockRecord>(entity =>
        {
            entity.ToTable("user_blocks");
            entity.HasKey(b => new { b.BlockerUserId, b.BlockedUserId });
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.BlockerUserId);
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

        modelBuilder.Entity<TerritoryStoreRecord>(entity =>
        {
            entity.ToTable("territory_stores");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Description).HasMaxLength(1000);
            entity.Property(s => s.Status).HasConversion<int>();
            entity.Property(s => s.ContactVisibility).HasConversion<int>();
            entity.Property(s => s.Phone).HasMaxLength(80);
            entity.Property(s => s.Whatsapp).HasMaxLength(80);
            entity.Property(s => s.Email).HasMaxLength(320);
            entity.Property(s => s.Instagram).HasMaxLength(120);
            entity.Property(s => s.Website).HasMaxLength(200);
            entity.Property(s => s.PreferredContactMethod).HasMaxLength(80);
            entity.Property(s => s.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(s => s.TerritoryId);
            entity.HasIndex(s => s.OwnerUserId);
            entity.HasIndex(s => new { s.TerritoryId, s.OwnerUserId }).IsUnique();
        });

        modelBuilder.Entity<StoreItemRecord>(entity =>
        {
            entity.ToTable("store_items");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Type).HasConversion<int>();
            entity.Property(l => l.Title).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Description).HasMaxLength(4000);
            entity.Property(l => l.Category).HasMaxLength(120);
            entity.Property(l => l.Tags).HasMaxLength(4000);
            entity.Property(l => l.PricingType).HasConversion<int>();
            entity.Property(l => l.PriceAmount).HasColumnType("numeric(18,2)");
            entity.Property(l => l.Currency).HasMaxLength(10);
            entity.Property(l => l.Unit).HasMaxLength(120);
            entity.Property(l => l.Latitude).HasColumnType("double precision");
            entity.Property(l => l.Longitude).HasColumnType("double precision");
            entity.Property(l => l.Status).HasConversion<int>();
            entity.Property(l => l.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(l => l.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(l => l.TerritoryId);
            entity.HasIndex(l => l.StoreId);
            entity.HasIndex(l => new { l.TerritoryId, l.Type, l.Status });
        });

        modelBuilder.Entity<ItemInquiryRecord>(entity =>
        {
            entity.ToTable("item_inquiries");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Status).HasConversion<int>();
            entity.Property(i => i.Message).HasMaxLength(2000);
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.TerritoryId);
            entity.HasIndex(i => i.ItemId);
            entity.HasIndex(i => i.StoreId);
            entity.HasIndex(i => i.FromUserId);
        });

        modelBuilder.Entity<CartRecord>(entity =>
        {
            entity.ToTable("carts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => new { c.TerritoryId, c.UserId }).IsUnique();
        });

        modelBuilder.Entity<CartItemRecord>(entity =>
        {
            entity.ToTable("cart_items");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Notes).HasMaxLength(1000);
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(i => i.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.CartId);
            entity.HasIndex(i => i.ItemId);
            entity.HasIndex(i => new { i.CartId, i.ItemId }).IsUnique();
        });

        modelBuilder.Entity<CheckoutRecord>(entity =>
        {
            entity.ToTable("checkouts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Status).HasConversion<int>();
            entity.Property(c => c.Currency).HasMaxLength(10).IsRequired();
            entity.Property(c => c.ItemsSubtotalAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.PlatformFeeAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.TotalAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => c.BuyerUserId);
            entity.HasIndex(c => c.StoreId);
        });

        modelBuilder.Entity<CheckoutItemRecord>(entity =>
        {
            entity.ToTable("checkout_items");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ItemType).HasConversion<int>();
            entity.Property(i => i.TitleSnapshot).HasMaxLength(200).IsRequired();
            entity.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineSubtotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.PlatformFeeLine).HasColumnType("numeric(18,2)");
            entity.Property(i => i.LineTotal).HasColumnType("numeric(18,2)");
            entity.Property(i => i.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(i => i.CheckoutId);
            entity.HasIndex(i => i.ItemId);
        });

        modelBuilder.Entity<PlatformFeeConfigRecord>(entity =>
        {
            entity.ToTable("platform_fee_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ItemType).HasConversion<int>();
            entity.Property(c => c.FeeMode).HasConversion<int>();
            entity.Property(c => c.FeeValue).HasColumnType("numeric(18,4)");
            entity.Property(c => c.Currency).HasMaxLength(10);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.ItemType, c.IsActive }).IsUnique();
        });

        modelBuilder.Entity<TerritoryPayoutConfigRecord>(entity =>
        {
            entity.ToTable("territory_payout_configs");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Currency).HasMaxLength(10).IsRequired();
            entity.Property(c => c.Frequency).HasConversion<int>().IsRequired();
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.IsActive });
        });

        modelBuilder.Entity<StoreRatingRecord>(entity =>
        {
            entity.ToTable("store_ratings");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(2000);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.StoreId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.StoreId, r.UserId }).IsUnique();
        });

        modelBuilder.Entity<StoreItemRatingRecord>(entity =>
        {
            entity.ToTable("store_item_ratings");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(2000);
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.StoreItemId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.StoreItemId, r.UserId }).IsUnique();
        });

        modelBuilder.Entity<StoreRatingResponseRecord>(entity =>
        {
            entity.ToTable("store_rating_responses");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.ResponseText).HasMaxLength(2000).IsRequired();
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.RatingId).IsUnique();
            entity.HasIndex(r => r.StoreId);
        });

        // -----------------------
        // Financial
        // -----------------------
        modelBuilder.Entity<FinancialTransactionRecord>(entity =>
        {
            entity.ToTable("financial_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).HasConversion<int>().IsRequired();
            entity.Property(t => t.Status).HasConversion<int>().IsRequired();
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(500).IsRequired();
            entity.Property(t => t.RelatedEntityType).HasMaxLength(100);
            entity.Property(t => t.RelatedTransactionIdsJson).HasColumnType("text").IsRequired();
            entity.Property(t => t.MetadataJson).HasColumnType("text");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => new { t.TerritoryId, t.Type });
            entity.HasIndex(t => new { t.TerritoryId, t.Status });
            entity.HasIndex(t => t.RelatedEntityId);
        });

        modelBuilder.Entity<TransactionStatusHistoryRecord>(entity =>
        {
            entity.ToTable("transaction_status_histories");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.PreviousStatus).HasConversion<int>().IsRequired();
            entity.Property(h => h.NewStatus).HasConversion<int>().IsRequired();
            entity.Property(h => h.Reason).HasMaxLength(500);
            entity.Property(h => h.ChangedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne<FinancialTransactionRecord>()
                .WithMany()
                .HasForeignKey(h => h.FinancialTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(h => h.FinancialTransactionId);
            entity.HasIndex(h => h.ChangedByUserId);
        });

        modelBuilder.Entity<SellerBalanceRecord>(entity =>
        {
            entity.ToTable("seller_balances");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Currency).HasMaxLength(10).IsRequired();
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => new { b.TerritoryId, b.SellerUserId }).IsUnique();
            entity.HasIndex(b => b.TerritoryId);
            entity.HasIndex(b => b.SellerUserId);
        });

        modelBuilder.Entity<SellerTransactionRecord>(entity =>
        {
            entity.ToTable("seller_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.Status).HasConversion<int>().IsRequired();
            entity.Property(t => t.PayoutId).HasMaxLength(200);
            entity.Property(t => t.ReadyForPayoutAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.PaidAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(t => t.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.CheckoutId).IsUnique();
            entity.HasIndex(t => t.SellerUserId);
            entity.HasIndex(t => new { t.TerritoryId, t.Status });
            entity.HasIndex(t => t.PayoutId);
        });

        modelBuilder.Entity<PlatformFinancialBalanceRecord>(entity =>
        {
            entity.ToTable("platform_financial_balances");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Currency).HasMaxLength(10).IsRequired();
            entity.Property(b => b.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(b => b.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(b => b.TerritoryId).IsUnique();
        });

        modelBuilder.Entity<PlatformRevenueTransactionRecord>(entity =>
        {
            entity.ToTable("platform_revenue_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.CheckoutId);
        });

        modelBuilder.Entity<PlatformExpenseTransactionRecord>(entity =>
        {
            entity.ToTable("platform_expense_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Currency).HasMaxLength(10).IsRequired();
            entity.Property(t => t.PayoutId).HasMaxLength(200);
            entity.Property(t => t.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(t => t.TerritoryId);
            entity.HasIndex(t => t.SellerTransactionId);
            entity.HasIndex(t => t.PayoutId);
        });

        modelBuilder.Entity<ReconciliationRecordRecord>(entity =>
        {
            entity.ToTable("reconciliation_records");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Currency).HasMaxLength(10).IsRequired();
            entity.Property(r => r.Status).HasConversion<int>().IsRequired();
            entity.Property(r => r.Notes).HasMaxLength(1000);
            entity.Property(r => r.ReconciliationDate).HasColumnType("timestamp with time zone");
            entity.Property(r => r.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(r => r.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(r => r.TerritoryId);
            entity.HasIndex(r => new { r.TerritoryId, r.Status });
            entity.HasIndex(r => r.ReconciliationDate);
        });

        // -----------------------
        // Chat
        // -----------------------
        modelBuilder.Entity<ChatConversationRecord>(entity =>
        {
            entity.ToTable("chat_conversations");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Kind).HasConversion<int>().IsRequired();
            entity.Property(c => c.Status).HasConversion<int>().IsRequired();
            entity.Property(c => c.Name).HasMaxLength(120);
            entity.Property(c => c.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.ApprovedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.LockedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(c => c.DisabledAtUtc).HasColumnType("timestamp with time zone");

            entity.HasIndex(c => c.TerritoryId);
            entity.HasIndex(c => new { c.TerritoryId, c.Kind, c.Status, c.CreatedAtUtc });

            // Impede múltiplos canais do território do mesmo tipo (Public/Residents)
            entity.HasIndex(c => new { c.TerritoryId, c.Kind })
                .IsUnique()
                .HasFilter("\"TerritoryId\" IS NOT NULL AND \"Kind\" IN (1, 2)");
        });

        modelBuilder.Entity<ChatConversationParticipantRecord>(entity =>
        {
            entity.ToTable("chat_conversation_participants");
            entity.HasKey(p => new { p.ConversationId, p.UserId });
            entity.Property(p => p.Role).HasConversion<int>().IsRequired();
            entity.Property(p => p.JoinedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.LeftAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.MutedUntilUtc).HasColumnType("timestamp with time zone");
            entity.Property(p => p.LastReadAtUtc).HasColumnType("timestamp with time zone");

            entity.HasOne<ChatConversationRecord>()
                .WithMany()
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.UserId);
        });

        modelBuilder.Entity<ChatMessageRecord>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.ContentType).HasConversion<int>().IsRequired();
            entity.Property(m => m.Text).HasColumnType("text");
            entity.Property(m => m.PayloadJson).HasColumnType("text");
            entity.Property(m => m.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.EditedAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(m => m.DeletedAtUtc).HasColumnType("timestamp with time zone");

            entity.HasOne<ChatConversationRecord>()
                .WithMany()
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => m.ConversationId);
            entity.HasIndex(m => new { m.ConversationId, m.CreatedAtUtc, m.Id });
            entity.HasIndex(m => new { m.SenderUserId, m.CreatedAtUtc });
        });

        modelBuilder.Entity<ChatConversationStatsRecord>(entity =>
        {
            entity.ToTable("chat_conversation_stats");
            entity.HasKey(s => s.ConversationId);
            entity.Property(s => s.LastPreview).HasMaxLength(200);
            entity.Property(s => s.LastMessageAtUtc).HasColumnType("timestamp with time zone");
            entity.Property(s => s.MessageCount).IsRequired();

            entity.HasOne<ChatConversationRecord>()
                .WithOne()
                .HasForeignKey<ChatConversationStatsRecord>(s => s.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.LastMessageAtUtc);
        });

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
