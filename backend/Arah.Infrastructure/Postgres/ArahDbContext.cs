using Arah.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Arah.Infrastructure.Postgres;

public sealed partial class ArahDbContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;

    public ArahDbContext(DbContextOptions<ArahDbContext> options)
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
    public DbSet<HealthAlertRecord> HealthAlerts => Set<HealthAlertRecord>();
    public DbSet<PostLikeRecord> PostLikes => Set<PostLikeRecord>();
    public DbSet<PostShareRecord> PostShares => Set<PostShareRecord>();
    public DbSet<TerritoryAssetRecord> TerritoryAssets => Set<TerritoryAssetRecord>();
    public DbSet<AssetGeoAnchorRecord> AssetGeoAnchors => Set<AssetGeoAnchorRecord>();
    public DbSet<AssetValidationRecord> AssetValidations => Set<AssetValidationRecord>();
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
    public DbSet<FeeSplitRuleRecord> FeeSplitRules => Set<FeeSplitRuleRecord>();

    // Chat
    public DbSet<ChatConversationRecord> ChatConversations => Set<ChatConversationRecord>();
    public DbSet<ChatConversationParticipantRecord> ChatConversationParticipants => Set<ChatConversationParticipantRecord>();
    public DbSet<ChatMessageRecord> ChatMessages => Set<ChatMessageRecord>();
    public DbSet<ChatConversationStatsRecord> ChatConversationStats => Set<ChatConversationStatsRecord>();

    // Media
    public DbSet<MediaAssetRecord> MediaAssets => Set<MediaAssetRecord>();
    public DbSet<MediaAttachmentRecord> MediaAttachments => Set<MediaAttachmentRecord>();
    public DbSet<TerritoryMediaConfigRecord> TerritoryMediaConfigs => Set<TerritoryMediaConfigRecord>();
    public DbSet<UserMediaPreferencesRecord> UserMediaPreferences => Set<UserMediaPreferencesRecord>();
    public DbSet<MediaStorageConfigRecord> MediaStorageConfigs => Set<MediaStorageConfigRecord>();

    // Policies
    public DbSet<TermsOfServiceRecord> TermsOfServices => Set<TermsOfServiceRecord>();
    public DbSet<TermsAcceptanceRecord> TermsAcceptances => Set<TermsAcceptanceRecord>();
    public DbSet<PrivacyPolicyRecord> PrivacyPolicies => Set<PrivacyPolicyRecord>();
    public DbSet<PrivacyPolicyAcceptanceRecord> PrivacyPolicyAcceptances => Set<PrivacyPolicyAcceptanceRecord>();

    // Email
    public DbSet<EmailQueueItemRecord> EmailQueueItems => Set<EmailQueueItemRecord>();

    // Connections (Círculo de Amigos)
    public DbSet<UserConnectionRecord> UserConnections => Set<UserConnectionRecord>();
    public DbSet<ConnectionPrivacySettingsRecord> ConnectionPrivacySettings => Set<ConnectionPrivacySettingsRecord>();

    // Subscriptions
    public DbSet<SubscriptionPlanRecord> SubscriptionPlans => Set<SubscriptionPlanRecord>();
    public DbSet<SubscriptionRecord> Subscriptions => Set<SubscriptionRecord>();
    public DbSet<SubscriptionPaymentRecord> SubscriptionPayments => Set<SubscriptionPaymentRecord>();
    public DbSet<CouponRecord> Coupons => Set<CouponRecord>();
    public DbSet<SubscriptionCouponRecord> SubscriptionCoupons => Set<SubscriptionCouponRecord>();
    public DbSet<SubscriptionPlanHistoryRecord> SubscriptionPlanHistories => Set<SubscriptionPlanHistoryRecord>();

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
        ConfigureTerritories(modelBuilder);
        ConfigureUsers(modelBuilder);
        ConfigureGovernance(modelBuilder);
        ConfigureNotifications(modelBuilder);
        ConfigureMembership(modelBuilder);
        ConfigureConfiguration(modelBuilder);
        ConfigureModeration(modelBuilder);
        ConfigureSocial(modelBuilder);
        ConfigureFeed(modelBuilder);
        ConfigureEvents(modelBuilder);
        ConfigureMap(modelBuilder);
        ConfigureHealth(modelBuilder);
        ConfigureAssets(modelBuilder);
        ConfigureConnections(modelBuilder);
        ConfigureMarketplace(modelBuilder);
        ConfigureFinancial(modelBuilder);
        ConfigureChat(modelBuilder);
        ConfigureMedia(modelBuilder);
        ConfigurePolicies(modelBuilder);
        ConfigureOutbox(modelBuilder);
        ConfigureSubscriptions(modelBuilder);
    }
}
