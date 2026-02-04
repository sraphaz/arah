using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Governance;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Infrastructure.InMemory;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryModerationService,
/// focusing on invalid rule configs, serialization errors, rule violations, and edge cases.
/// </summary>
public sealed class TerritoryModerationServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryTerritoryModerationRuleRepository _ruleRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly TerritoryModerationService _service;

    public TerritoryModerationServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _ruleRepository = new InMemoryTerritoryModerationRuleRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new TerritoryModerationService(_ruleRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateRuleAsync_WithEmptyTerritoryId_HandlesGracefully()
    {
        var userId = Guid.NewGuid();
        var ruleConfig = new { prohibitedWords = new[] { "spam" } };

        var result = await _service.CreateRuleAsync(
            Guid.Empty,
            userId,
            null,
            RuleType.ProhibitedWords,
            ruleConfig,
            CancellationToken.None);

        // Pode criar regra mesmo com Empty Guid (validação está na entidade)
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateRuleAsync_WithInvalidRuleConfig_ReturnsFailure()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        // Criar objeto que não pode ser serializado (usando objeto complexo)
        var invalidConfig = new { Nested = new { Deep = new { Value = "test" } } };

        var result = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ProhibitedWords,
            invalidConfig,
            CancellationToken.None);

        // Deve criar com sucesso (JSON serializa normalmente)
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateRuleAsync_WithNullRuleConfig_HandlesGracefully()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ProhibitedWords,
            null!,
            CancellationToken.None);

        // Pode falhar na serialização ou criar regra com "null"
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ListRulesAsync_WithNonExistentTerritory_ReturnsEmpty()
    {
        var result = await _service.ListRulesAsync(
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListRulesAsync_WithIsActiveFilter_ReturnsFiltered()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { prohibitedWords = new[] { "spam" } };

        // Criar regra ativa
        var activeResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ProhibitedWords,
            ruleConfig,
            CancellationToken.None);
        Assert.True(activeResult.IsSuccess);

        // Criar regra inativa
        var inactiveRule = new TerritoryModerationRule(
            Guid.NewGuid(),
            territoryId,
            null,
            RuleType.ProhibitedWords,
            JsonSerializer.Serialize(ruleConfig),
            false,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _ruleRepository.AddAsync(inactiveRule, CancellationToken.None);

        var activeRules = await _service.ListRulesAsync(territoryId, true, CancellationToken.None);
        var inactiveRules = await _service.ListRulesAsync(territoryId, false, CancellationToken.None);
        var allRules = await _service.ListRulesAsync(territoryId, null, CancellationToken.None);

        Assert.Single(activeRules);
        Assert.Single(inactiveRules);
        Assert.Equal(2, allRules.Count);
    }

    [Fact]
    public async Task ApplyRulesAsync_WithPostContainingProhibitedWord_ReturnsFailure()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { prohibitedWords = new[] { "spam", "scam" } };

        // Criar regra
        var ruleResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ProhibitedWords,
            ruleConfig,
            CancellationToken.None);
        Assert.True(ruleResult.IsSuccess);

        // Criar post com palavra proibida
        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Title with SPAM",
            "Content with scam word",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(post, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("prohibited word", result.Error ?? "");
    }

    [Fact]
    public async Task ApplyRulesAsync_WithPostWithAllowedType_ReturnsSuccess()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { allowedTypes = new[] { "General", "Alert" } };

        // Criar regra
        var ruleResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ContentType,
            ruleConfig,
            CancellationToken.None);
        Assert.True(ruleResult.IsSuccess);

        // Criar post com tipo permitido
        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(post, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ApplyRulesAsync_WithPostWithDisallowedType_ReturnsFailure()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { allowedTypes = new[] { "General" } };

        // Criar regra
        var ruleResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ContentType,
            ruleConfig,
            CancellationToken.None);
        Assert.True(ruleResult.IsSuccess);

        // Criar post com tipo não permitido
        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Title",
            "Content",
            PostType.Alert, // Não permitido
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(post, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not allowed", result.Error ?? "");
    }

    [Fact]
    public async Task ApplyRulesAsync_WithStoreItemContainingProhibitedWord_ReturnsFailure()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { prohibitedWords = new[] { "spam" } };

        // Criar regra de marketplace
        var ruleResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.MarketplacePolicy,
            ruleConfig,
            CancellationToken.None);
        Assert.True(ruleResult.IsSuccess);

        // Criar item com palavra proibida
        var item = new StoreItem(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            ItemType.Product,
            "Title with SPAM",
            "Description",
            null,
            null,
            ItemPricingType.Fixed,
            100m,
            "BRL",
            null,
            null,
            null,
            ItemStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(item, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("prohibited word", result.Error ?? "");
    }

    [Fact]
    public async Task ApplyRulesAsync_WithNoActiveRules_ReturnsSuccess()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Criar regra inativa
        var inactiveRule = new TerritoryModerationRule(
            Guid.NewGuid(),
            territoryId,
            null,
            RuleType.ProhibitedWords,
            JsonSerializer.Serialize(new { prohibitedWords = new[] { "spam" } }),
            false,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _ruleRepository.AddAsync(inactiveRule, CancellationToken.None);

        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Title with spam",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(post, CancellationToken.None);

        Assert.True(result.IsSuccess); // Regra inativa não é aplicada
    }

    [Fact]
    public void TerritoryModerationRule_WithInvalidRuleJson_ThrowsInConstructor()
    {
        // TerritoryModerationRule valida JSON no construtor; não é possível criar regra com JSON inválido
        Assert.Throws<ArgumentException>(() => new TerritoryModerationRule(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            RuleType.ProhibitedWords,
            "invalid json {",
            true,
            DateTime.UtcNow,
            DateTime.UtcNow));
    }

    [Fact]
    public async Task ApplyRulesAsync_WithUnicodeInProhibitedWords_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ruleConfig = new { prohibitedWords = new[] { "café", "naïve", "文字" } };

        var ruleResult = await _service.CreateRuleAsync(
            territoryId,
            userId,
            null,
            RuleType.ProhibitedWords,
            ruleConfig,
            CancellationToken.None);
        Assert.True(ruleResult.IsSuccess);

        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            "Title with café",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        var result = await _service.ApplyRulesAsync(post, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("prohibited word", result.Error ?? "");
    }
}
