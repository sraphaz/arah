using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Domain.Fiscal;

namespace Arah.Application.Services.Fiscal;

public sealed class TerritoryPaymentMethodsService
{
    private readonly ITerritoryPaymentMethodsConfigRepository _repository;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryPaymentMethodsService(
        ITerritoryPaymentMethodsConfigRepository repository,
        ITerritoryRepository territoryRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _territoryRepository = territoryRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Sem config: lista vazia (checkout não oferece meios territoriais até o implementador configurar).
    /// </summary>
    public async Task<Result<TerritoryPaymentMethodsView>> GetAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return Result<TerritoryPaymentMethodsView>.Failure("Territory not found.");
        }

        var config = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        if (config is null)
        {
            return Result<TerritoryPaymentMethodsView>.Success(
                new TerritoryPaymentMethodsView(territoryId, Array.Empty<string>(), null, null, true));
        }

        return Result<TerritoryPaymentMethodsView>.Success(TerritoryPaymentMethodsView.From(config));
    }

    public async Task<OperationResult<TerritoryPaymentMethodsView>> UpsertAsync(
        Guid territoryId,
        IReadOnlyList<string> methodNames,
        string? pspProvider,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return OperationResult<TerritoryPaymentMethodsView>.Failure("Territory not found.");
        }

        ArgumentNullException.ThrowIfNull(methodNames);

        var methods = new List<TerritoryPaymentMethodKind>();
        foreach (var name in methodNames)
        {
            if (!Enum.TryParse<TerritoryPaymentMethodKind>(name, ignoreCase: true, out var kind))
            {
                return OperationResult<TerritoryPaymentMethodsView>.Failure(
                    $"Invalid payment method '{name}'. Allowed: Pix, Card, Boleto.");
            }

            methods.Add(kind);
        }

        var now = DateTimeOffset.UtcNow;
        var existing = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        TerritoryPaymentMethodsConfig config;
        if (existing is null)
        {
            config = new TerritoryPaymentMethodsConfig(Guid.NewGuid(), territoryId, methods, pspProvider, now);
            await _repository.AddAsync(config, cancellationToken);
            await _auditLogger.LogAsync(
                new AuditEntry("fiscal.payment-methods.created", actorUserId, territoryId, config.Id, DateTime.UtcNow),
                cancellationToken);
        }
        else
        {
            existing.ReplaceMethods(methods, pspProvider, now);
            await _repository.UpdateAsync(existing, cancellationToken);
            config = existing;
            await _auditLogger.LogAsync(
                new AuditEntry("fiscal.payment-methods.updated", actorUserId, territoryId, config.Id, DateTime.UtcNow),
                cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<TerritoryPaymentMethodsView>.Success(TerritoryPaymentMethodsView.From(config));
    }
}

public sealed record TerritoryPaymentMethodsView(
    Guid TerritoryId,
    IReadOnlyList<string> Methods,
    string? PspProvider,
    DateTimeOffset? UpdatedAtUtc,
    bool IsDefaultEmpty)
{
    public static TerritoryPaymentMethodsView From(TerritoryPaymentMethodsConfig config) =>
        new(
            config.TerritoryId,
            config.Methods.Select(m => m.ToString()).ToArray(),
            config.PspProvider,
            config.UpdatedAtUtc,
            false);
}
