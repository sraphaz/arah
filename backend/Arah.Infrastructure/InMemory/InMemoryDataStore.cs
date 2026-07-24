using System.Collections.Concurrent;
using Arah.Application.Models;
using Arah.Modules.Assets.Domain;
using Arah.Domain.Events;
using Arah.Domain.Feed;
using Arah.Domain.Financial;
using Arah.Domain.Health;
using Arah.Modules.Map.Domain;
using Arah.Modules.Marketplace.Domain;
using Arah.Domain.Media;
using Arah.Domain.Membership;
using Arah.Domain.Configuration;
using Arah.Domain.Chat;
using Arah.Domain.Connections;
using Arah.Domain.Policies;
using Arah.Domain.Social.JoinRequests;
using Arah.Domain.Subscriptions;
using Arah.Domain.Territories;
using Arah.Domain.Users;
using Arah.Domain.Email;
using Arah.Modules.Moderation.Domain.Evidence;
using Arah.Modules.Moderation.Domain.Moderation;
using Arah.Modules.Moderation.Domain.Work;

namespace Arah.Infrastructure.InMemory;

public sealed partial class InMemoryDataStore
{
    public InMemoryDataStore()
    {
        InMemorySeeder.Seed(this);
    }

    public List<Territory> Territories { get; } = new();
    public List<User> Users { get; } = new();
    public List<TerritoryMembership> Memberships { get; } = new();
    public List<CommunityPost> Posts { get; } = new();
    public List<TerritoryEvent> TerritoryEvents { get; } = new();
    public List<EventParticipation> EventParticipations { get; } = new();
    public List<MapEntity> MapEntities { get; } = new();
    public List<MapEntityRelation> MapEntityRelations { get; } = new();
    public List<PostGeoAnchor> PostGeoAnchors { get; } = new();
    public List<HealthAlert> HealthAlerts { get; } = new();
    public List<TerritoryAsset> TerritoryAssets { get; } = new();
    public List<AssetGeoAnchor> AssetGeoAnchors { get; } = new();
    public List<AssetValidation> AssetValidations { get; } = new();
    public List<PostAsset> PostAssets { get; } = new();
    public List<Store> TerritoryStores { get; } = new();
    public List<StoreItem> StoreItems { get; } = new();
    public List<ItemInquiry> ItemInquiries { get; } = new();
    public List<Cart> Carts { get; } = new();
    public List<CartItem> CartItems { get; } = new();
    public List<Checkout> Checkouts { get; } = new();
    public List<CheckoutItem> CheckoutItems { get; } = new();
    public List<StoreRating> StoreRatings { get; } = new();
    public List<StoreItemRating> StoreItemRatings { get; } = new();
    public List<StoreRatingResponse> StoreRatingResponses { get; } = new();
    public List<PlatformFeeConfig> PlatformFeeConfigs { get; } = new();
    public List<TerritoryPayoutConfig> TerritoryPayoutConfigs { get; } = new();

    // Financial
    public List<FinancialTransaction> FinancialTransactions { get; } = new();
    public List<TransactionStatusHistory> TransactionStatusHistories { get; } = new();
    public List<SellerBalance> SellerBalances { get; } = new();

    // Policies
    public List<TermsOfService> TermsOfServices { get; } = new();
    public List<TermsAcceptance> TermsAcceptances { get; } = new();
    public List<PrivacyPolicy> PrivacyPolicies { get; } = new();
    public List<PrivacyPolicyAcceptance> PrivacyPolicyAcceptances { get; } = new();
    public List<SellerTransaction> SellerTransactions { get; } = new();
    public List<PlatformFinancialBalance> PlatformFinancialBalances { get; } = new();
    public List<PlatformRevenueTransaction> PlatformRevenueTransactions { get; } = new();
    public List<PlatformExpenseTransaction> PlatformExpenseTransactions { get; } = new();
    public List<ReconciliationRecord> ReconciliationRecords { get; } = new();
    public List<FeeSplitRule> FeeSplitRules { get; } = new();

    public ConcurrentDictionary<string, Guid> ActiveTerritories { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<Guid, HashSet<string>> PostLikes { get; } = new();
    public Dictionary<Guid, List<PostComment>> PostComments { get; } = new();
    public Dictionary<Guid, HashSet<Guid>> PostShares { get; } = new();
    public List<Application.Models.AuditEntry> AuditEntries { get; } = new();
    public List<ModerationReport> ModerationReports { get; } = new();
    public List<UserBlock> UserBlocks { get; } = new();
    public List<UserConnection> UserConnections { get; } = new();
    public List<ConnectionPrivacySettings> ConnectionPrivacySettings { get; } = new();
    public List<Sanction> Sanctions { get; } = new();
    public List<OutboxMessage> OutboxMessages { get; } = new();
    public List<UserNotification> UserNotifications { get; } = new();
    public List<TerritoryJoinRequest> TerritoryJoinRequests { get; } = new();
    public List<TerritoryJoinRequestRecipient> TerritoryJoinRequestRecipients { get; } = new();
    public List<Domain.Users.UserPreferences> UserPreferences { get; } = new();
    public List<MembershipSettings> MembershipSettings { get; } = new();
    public List<MembershipCapability> MembershipCapabilities { get; } = new();
    public List<SystemPermission> SystemPermissions { get; } = new();
    public List<SystemConfig> SystemConfigs { get; } = new();
    public List<WorkItem> WorkItems { get; } = new();
    public List<DocumentEvidence> DocumentEvidences { get; } = new();

    // Chat
    public List<ChatConversation> ChatConversations { get; } = new();
    public List<ConversationParticipant> ChatParticipants { get; } = new();
    public List<ChatMessage> ChatMessages { get; } = new();
    public List<Arah.Application.Models.ChatConversationStats> ChatStats { get; } = new();

    // Media
    public List<MediaAsset> MediaAssets { get; } = new();
    public List<MediaAttachment> MediaAttachments { get; } = new();
    public List<TerritoryMediaConfig> TerritoryMediaConfigs { get; } = new();
    public List<UserMediaPreferences> UserMediaPreferences { get; } = new();
    public List<MediaStorageConfig> MediaStorageConfigs { get; } = new();

    // User Interests
    public List<UserInterest> UserInterests { get; } = new();

    // Governance
    public List<Domain.Governance.Voting> Votings { get; } = new();
    public List<Domain.Governance.Vote> Votes { get; } = new();
    public List<Domain.Governance.TerritoryModerationRule> TerritoryModerationRules { get; } = new();
    public List<Domain.Territories.TerritoryCharacterization> TerritoryCharacterizations { get; } = new();

    // Email
    public List<EmailQueueItem> EmailQueueItems { get; } = new();

    // Notifications
    public List<Domain.Notifications.NotificationConfig> NotificationConfigs { get; } = new();

    // Subscriptions
    public List<SubscriptionPlan> SubscriptionPlans { get; } = new();
    public List<Subscription> Subscriptions { get; } = new();
    public List<SubscriptionPayment> SubscriptionPayments { get; } = new();
    public List<SubscriptionCoupon> SubscriptionCoupons { get; } = new();
    public List<Coupon> Coupons { get; } = new();
    public List<SubscriptionPlanHistory> SubscriptionPlanHistories { get; } = new();
}
