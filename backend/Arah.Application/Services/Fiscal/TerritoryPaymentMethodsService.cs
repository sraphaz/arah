using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Domain.Fiscal;

namespace Arah.Application.Services.Fiscal;

public sealed class TerritoryPaymentMethodsService
{
    private readonly ITerritoryPaymentMethodsConfigRepository _repository;
    private readonly ITerritoryFiscalPackBindingRepository _bindingRepository;
    private readonly IFiscalPackCatalog _catalog;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryPaymentMethodsService(
        ITerritoryPaymentMethodsConfigRepository repository,
        ITerritoryFiscalPackBindingRepository bindingRepository,
        IFiscalPackCatalog catalog,
        ITerritoryRepository territoryRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(bindingRepository);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(territoryRepository);
        ArgumentNullException.ThrowIfNull(auditLogger);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _repository = repository;
        _bindingRepository = bindingRepository;
        _catalog = catalog;
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

        if (!string.IsNullOrWhiteSpace(pspProvider) && pspProvider.Trim().Length > 64)
        {
            return OperationResult<TerritoryPaymentMethodsView>.Failure(
                "PspProvider must be at most 64 characters.");
        }

        var methods = new List<TerritoryPaymentMethodKind>();
        foreach (var name in methodNames)
        {
            if (!Enum.TryParse<TerritoryPaymentMethodKind>(name, ignoreCase: true, out var kind)
                || !Enum.IsDefined(kind))
            {
                return OperationResult<TerritoryPaymentMethodsView>.Failure(
                    $"Invalid payment method '{name}'. Allowed: Pix, Card, Boleto.");
            }

            methods.Add(kind);
        }

        var pixRequired = await ActivePackRequiresPixAsync(territoryId, cancellationToken);
        if (pixRequired && !methods.Contains(TerritoryPaymentMethodKind.Pix))
        {
            return OperationResult<TerritoryPaymentMethodsView>.Failure(
                "Cannot remove PIX while an active fiscal pack requires the pix capability.");
        }

        var now = DateTimeOffset.UtcNow;
        var existing = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        TerritoryPaymentMethodsConfig config;
        try
        {
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
        }
        catch (ArgumentException ex)
        {
            return OperationResult<TerritoryPaymentMethodsView>.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return OperationResult<TerritoryPaymentMethodsView>.Failure(ex.Message);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<TerritoryPaymentMethodsView>.Success(TerritoryPaymentMethodsView.From(config));
    }

    private async Task<bool> ActivePackRequiresPixAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var binding = await _bindingRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        if (binding is null || !binding.IsActive)
        {
            return false;
        }

        var pack = _catalog.GetById(binding.PackId);
        return pack is not null
               && pack.Capabilities.Contains("pix", StringComparer.OrdinalIgnoreCase);
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
