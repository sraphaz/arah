using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Domain.Governance;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Modules.Moderation.Application.Interfaces;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar regras de moderação comunitária.
/// </summary>
public sealed class TerritoryModerationService
{
    private readonly ITerritoryModerationRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryModerationService(
        ITerritoryModerationRuleRepository ruleRepository,
        IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria uma nova regra de moderação.
    /// </summary>
    public async Task<Result<TerritoryModerationRule>> CreateRuleAsync(
        Guid territoryId,
        Guid userId,
        Guid? createdByVotingId,
        RuleType ruleType,
        object ruleConfig,
        CancellationToken cancellationToken)
    {
        string ruleJson;
        try
        {
            ruleJson = JsonSerializer.Serialize(ruleConfig);
        }
        catch
        {
            return Result<TerritoryModerationRule>.Failure("Failed to serialize rule configuration.");
        }

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            territoryId,
            createdByVotingId,
            ruleType,
            ruleJson,
            true,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await _ruleRepository.AddAsync(rule, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<TerritoryModerationRule>.Success(rule);
    }

    /// <summary>
    /// Lista regras de um território.
    /// </summary>
    public async Task<IReadOnlyList<TerritoryModerationRule>> ListRulesAsync(
        Guid territoryId,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        return await _ruleRepository.ListByTerritoryAsync(territoryId, isActive, cancellationToken);
    }

    /// <summary>
    /// Verifica se um post viola regras de moderação.
    /// </summary>
    public async Task<OperationResult> ApplyRulesAsync(
        CommunityPost post,
        CancellationToken cancellationToken)
    {
        var rules = await _ruleRepository.ListByTerritoryAsync(post.TerritoryId, isActive: true, cancellationToken);

        foreach (var rule in rules)
        {
            var violation = await CheckRuleViolationAsync(post, rule, cancellationToken);
            if (violation is not null)
            {
                return OperationResult.Failure(violation);
            }
        }

        return OperationResult.Success();
    }

    /// <summary>
    /// Verifica se um item do marketplace viola regras de moderação.
    /// </summary>
    public async Task<OperationResult> ApplyRulesAsync(
        StoreItem item,
        CancellationToken cancellationToken)
    {
        var rules = await _ruleRepository.ListByTerritoryAsync(item.TerritoryId, isActive: true, cancellationToken);

        foreach (var rule in rules)
        {
            if (rule.RuleType == RuleType.MarketplacePolicy)
            {
                var violation = await CheckMarketplaceRuleViolationAsync(item, rule, cancellationToken);
                if (violation is not null)
                {
                    return OperationResult.Failure(violation);
                }
            }
        }

        return OperationResult.Success();
    }

    private Task<string?> CheckRuleViolationAsync(
        CommunityPost post,
        TerritoryModerationRule rule,
        CancellationToken cancellationToken)
    {
        try
        {
            var ruleConfig = JsonSerializer.Deserialize<JsonElement>(rule.RuleJson);

            var result = rule.RuleType switch
            {
                RuleType.ContentType => CheckContentTypeRule(post, ruleConfig),
                RuleType.ProhibitedWords => CheckProhibitedWordsRule(post, ruleConfig),
                RuleType.Behavior => null, // Regras de comportamento são aplicadas em outros contextos
                _ => null
            };

            return Task.FromResult<string?>(result);
        }
        catch
        {
            return Task.FromResult<string?>(null); // Se não conseguir deserializar, ignora a regra
        }
    }

    private Task<string?> CheckMarketplaceRuleViolationAsync(
        StoreItem item,
        TerritoryModerationRule rule,
        CancellationToken cancellationToken)
    {
        try
        {
            var ruleConfig = JsonSerializer.Deserialize<JsonElement>(rule.RuleJson);

            // Verificar palavras proibidas no título e descrição
            if (ruleConfig.TryGetProperty("prohibitedWords", out var prohibitedWords))
            {
                var words = prohibitedWords.EnumerateArray().Select(w => w.GetString()?.ToLowerInvariant()).Where(w => w is not null).ToList();
                var titleLower = item.Title?.ToLowerInvariant() ?? "";
                var descriptionLower = item.Description?.ToLowerInvariant() ?? "";

                foreach (var word in words)
                {
                    if (titleLower.Contains(word!) || descriptionLower.Contains(word!))
                    {
                        return Task.FromResult<string?>($"Item contains prohibited word: {word}");
                    }
                }
            }

            return Task.FromResult<string?>(null);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    private string? CheckContentTypeRule(CommunityPost post, JsonElement ruleConfig)
    {
        if (ruleConfig.TryGetProperty("allowedTypes", out var allowedTypes))
        {
            var types = allowedTypes.EnumerateArray().Select(t => t.GetString()).ToList();
            if (!types.Contains(post.Type.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                return $"Post type {post.Type} is not allowed in this territory.";
            }
        }

        return null;
    }

    private string? CheckProhibitedWordsRule(CommunityPost post, JsonElement ruleConfig)
    {
        if (ruleConfig.TryGetProperty("prohibitedWords", out var prohibitedWords))
        {
            var words = prohibitedWords.EnumerateArray().Select(w => w.GetString()?.ToLowerInvariant()).Where(w => w is not null).ToList();
            var titleLower = post.Title?.ToLowerInvariant() ?? "";
            var contentLower = post.Content?.ToLowerInvariant() ?? "";

            foreach (var word in words)
            {
                if (titleLower.Contains(word!) || contentLower.Contains(word!))
                {
                    return $"Post contains prohibited word: {word}";
                }
            }
        }

        return null;
    }
}
