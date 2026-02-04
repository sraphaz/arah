using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar configurações de payout por território.
/// </summary>
public sealed class TerritoryPayoutConfigService
{
    private readonly ITerritoryPayoutConfigRepository _configRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryPayoutConfigService(
        ITerritoryPayoutConfigRepository configRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _configRepository = configRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtém a configuração ativa de payout para um território.
    /// </summary>
    public Task<TerritoryPayoutConfig?> GetActiveConfigAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        return _configRepository.GetActiveAsync(territoryId, cancellationToken);
    }

    /// <summary>
    /// Cria ou atualiza a configuração de payout para um território.
    /// </summary>
    public async Task<OperationResult<TerritoryPayoutConfig>> UpsertConfigAsync(
        Guid territoryId,
        int retentionPeriodDays,
        long minimumPayoutAmountInCents,
        long? maximumPayoutAmountInCents,
        PayoutFrequency frequency,
        bool autoPayoutEnabled,
        bool requiresApproval,
        string currency,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Validações
        if (retentionPeriodDays < 0)
        {
            return OperationResult<TerritoryPayoutConfig>.Failure("RetentionPeriodDays must be non-negative.");
        }

        if (minimumPayoutAmountInCents < 0)
        {
            return OperationResult<TerritoryPayoutConfig>.Failure("MinimumPayoutAmountInCents must be non-negative.");
        }

        if (maximumPayoutAmountInCents.HasValue && maximumPayoutAmountInCents.Value < minimumPayoutAmountInCents)
        {
            return OperationResult<TerritoryPayoutConfig>.Failure("MaximumPayoutAmountInCents must be greater than or equal to MinimumPayoutAmountInCents.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            return OperationResult<TerritoryPayoutConfig>.Failure("Currency must be a 3-character code (e.g., BRL, USD).");
        }

        // Buscar configuração existente ativa
        var existing = await _configRepository.GetActiveAsync(territoryId, cancellationToken);

        TerritoryPayoutConfig config;
        if (existing is null)
        {
            // Criar nova configuração
            config = new TerritoryPayoutConfig(
                Guid.NewGuid(),
                territoryId,
                retentionPeriodDays,
                minimumPayoutAmountInCents,
                maximumPayoutAmountInCents,
                frequency,
                autoPayoutEnabled,
                requiresApproval,
                currency.ToUpperInvariant());

            await _configRepository.AddAsync(config, cancellationToken);

            await _auditLogger.LogAsync(
                new Models.AuditEntry(
                    "payout.config.created",
                    userId,
                    territoryId,
                    config.Id,
                    DateTime.UtcNow),
                cancellationToken);
        }
        else
        {
            // Desativar configuração antiga
            existing.Deactivate();
            await _configRepository.UpdateAsync(existing, cancellationToken);

            // Criar nova configuração ativa
            config = new TerritoryPayoutConfig(
                Guid.NewGuid(),
                territoryId,
                retentionPeriodDays,
                minimumPayoutAmountInCents,
                maximumPayoutAmountInCents,
                frequency,
                autoPayoutEnabled,
                requiresApproval,
                currency.ToUpperInvariant());

            await _configRepository.AddAsync(config, cancellationToken);

            await _auditLogger.LogAsync(
                new Models.AuditEntry(
                    "payout.config.updated",
                    userId,
                    territoryId,
                    config.Id,
                    DateTime.UtcNow),
                cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<TerritoryPayoutConfig>.Success(config);
    }
}
