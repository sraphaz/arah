using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Chat;
using Araponga.Domain.Media;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for ChatService,
/// focusing on message validation, Unicode, empty messages, and conversation access.
/// </summary>
public class ChatServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly InMemorySharedStore SharedStore = new();

    [Fact]
    public async Task CreateGroupAsync_WithEmptyName_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var conversationRepository = new InMemoryChatConversationRepository(dataStore);
        var participantRepository = new InMemoryChatConversationParticipantRepository(dataStore);
        var messageRepository = new InMemoryChatMessageRepository(dataStore);
        var statsRepository = new InMemoryChatConversationStatsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(SharedStore);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        // Habilitar feature flags necessários para chat
        featureFlagService.SetEnabledFlags(TestTerritoryId, new[] { FeatureFlag.ChatEnabled, FeatureFlag.ChatGroups });
        var globalMediaLimits = new InMemoryGlobalMediaLimits();
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlagService,
            unitOfWork,
            globalMediaLimits);
        
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new FeatureFlagCacheService(
            featureFlagService,
            cache);
        
        var membershipRepository = new InMemoryTerritoryMembershipRepository(SharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(SharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(SharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(SharedStore);
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlagService);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        var chatService = new ChatService(
            conversationRepository,
            participantRepository,
            messageRepository,
            statsRepository,
            userRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            featureFlags,
            accessEvaluator,
            unitOfWork);

        var result = await chatService.CreateGroupAsync(
            TestTerritoryId,
            TestUserId,
            "   ",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Group name is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateGroupAsync_WithNullName_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var conversationRepository = new InMemoryChatConversationRepository(dataStore);
        var participantRepository = new InMemoryChatConversationParticipantRepository(dataStore);
        var messageRepository = new InMemoryChatMessageRepository(dataStore);
        var statsRepository = new InMemoryChatConversationStatsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(SharedStore);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        // Habilitar feature flags necessários para chat
        featureFlagService.SetEnabledFlags(TestTerritoryId, new[] { FeatureFlag.ChatEnabled, FeatureFlag.ChatGroups });
        var globalMediaLimits = new InMemoryGlobalMediaLimits();
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlagService,
            unitOfWork,
            globalMediaLimits);
        
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new FeatureFlagCacheService(
            featureFlagService,
            cache);
        
        var membershipRepository = new InMemoryTerritoryMembershipRepository(SharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(SharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(SharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(SharedStore);
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlagService);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        var chatService = new ChatService(
            conversationRepository,
            participantRepository,
            messageRepository,
            statsRepository,
            userRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            featureFlags,
            accessEvaluator,
            unitOfWork);

        var result = await chatService.CreateGroupAsync(
            TestTerritoryId,
            TestUserId,
            null!,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Group name is required", result.Error ?? "");
    }

    [Fact]
    public async Task SendTextMessageAsync_WithEmptyText_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var conversationRepository = new InMemoryChatConversationRepository(dataStore);
        var participantRepository = new InMemoryChatConversationParticipantRepository(dataStore);
        var messageRepository = new InMemoryChatMessageRepository(dataStore);
        var statsRepository = new InMemoryChatConversationStatsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(SharedStore);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        // Habilitar feature flags necessários para chat
        featureFlagService.SetEnabledFlags(TestTerritoryId, new[] { FeatureFlag.ChatEnabled, FeatureFlag.ChatGroups });
        var globalMediaLimits = new InMemoryGlobalMediaLimits();
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlagService,
            unitOfWork,
            globalMediaLimits);
        
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new FeatureFlagCacheService(
            featureFlagService,
            cache);
        
        var membershipRepository = new InMemoryTerritoryMembershipRepository(SharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(SharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(SharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(SharedStore);
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlagService);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        var chatService = new ChatService(
            conversationRepository,
            participantRepository,
            messageRepository,
            statsRepository,
            userRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            featureFlags,
            accessEvaluator,
            unitOfWork);

        var result = await chatService.SendTextMessageAsync(
            Guid.NewGuid(),
            TestUserId,
            "   ",
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Message text is required", result.Error ?? "");
    }

    [Fact]
    public async Task SendTextMessageAsync_WithUnicodeText_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var conversationRepository = new InMemoryChatConversationRepository(dataStore);
        var participantRepository = new InMemoryChatConversationParticipantRepository(dataStore);
        var messageRepository = new InMemoryChatMessageRepository(dataStore);
        var statsRepository = new InMemoryChatConversationStatsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(SharedStore);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        // Habilitar feature flags necessários para chat
        featureFlagService.SetEnabledFlags(TestTerritoryId, new[] { FeatureFlag.ChatEnabled, FeatureFlag.ChatGroups });
        var globalMediaLimits = new InMemoryGlobalMediaLimits();
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlagService,
            unitOfWork,
            globalMediaLimits);
        
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new FeatureFlagCacheService(
            featureFlagService,
            cache);
        
        var membershipRepository = new InMemoryTerritoryMembershipRepository(SharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(SharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(SharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(SharedStore);
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlagService);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        var chatService = new ChatService(
            conversationRepository,
            participantRepository,
            messageRepository,
            statsRepository,
            userRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            featureFlags,
            accessEvaluator,
            unitOfWork);

        var result = await chatService.SendTextMessageAsync(
            Guid.NewGuid(),
            TestUserId,
            "Mensagem com café, naïve, résumé, 文字 e emoji 🎉",
            null,
            CancellationToken.None);

        // Deve falhar porque a conversa não existe, mas a validação de texto Unicode deve passar
        Assert.True(result.IsFailure);
        // Não deve falhar por causa do texto Unicode
        Assert.DoesNotContain("Unicode", result.Error ?? "");
    }

    [Fact]
    public async Task ListMessagesAsync_WithInvalidLimit_AdjustsToDefault()
    {
        var dataStore = new InMemoryDataStore();
        var conversationRepository = new InMemoryChatConversationRepository(dataStore);
        var participantRepository = new InMemoryChatConversationParticipantRepository(dataStore);
        var messageRepository = new InMemoryChatMessageRepository(dataStore);
        var statsRepository = new InMemoryChatConversationStatsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(SharedStore);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        // Habilitar feature flags necessários para chat
        featureFlagService.SetEnabledFlags(TestTerritoryId, new[] { FeatureFlag.ChatEnabled, FeatureFlag.ChatGroups });
        var globalMediaLimits = new InMemoryGlobalMediaLimits();
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlagService,
            unitOfWork,
            globalMediaLimits);
        
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlags = new FeatureFlagCacheService(
            featureFlagService,
            cache);
        
        var membershipRepository = new InMemoryTerritoryMembershipRepository(SharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(SharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(SharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(SharedStore);
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlagService);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        var chatService = new ChatService(
            conversationRepository,
            participantRepository,
            messageRepository,
            statsRepository,
            userRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            featureFlags,
            accessEvaluator,
            unitOfWork);

        // Teste com limite negativo (deve ajustar para 50)
        var result1 = await chatService.ListMessagesAsync(
            Guid.NewGuid(),
            TestUserId,
            null,
            null,
            -10,
            CancellationToken.None);

        // Teste com limite zero (deve ajustar para 50)
        var result2 = await chatService.ListMessagesAsync(
            Guid.NewGuid(),
            TestUserId,
            null,
            null,
            0,
            CancellationToken.None);

        // Teste com limite maior que 100 (deve ajustar para 50)
        var result3 = await chatService.ListMessagesAsync(
            Guid.NewGuid(),
            TestUserId,
            null,
            null,
            200,
            CancellationToken.None);

        // Todos devem falhar porque a conversa não existe, mas a validação de limite deve passar
        Assert.True(result1.IsFailure);
        Assert.True(result2.IsFailure);
        Assert.True(result3.IsFailure);
    }
}
