using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Domain.Fiscal;

namespace Arah.Application.Services.Fiscal;

public sealed class TerritoryFiscalPackService
{
    private readonly ITerritoryFiscalPackBindingRepository _bindingRepository;
    private readonly IFiscalPackCatalog _catalog;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly ITerritoryPaymentMethodsConfigRepository _paymentMethodsRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryFiscalPackService(
        ITerritoryFiscalPackBindingRepository bindingRepository,
        IFiscalPackCatalog catalog,
        ITerritoryRepository territoryRepository,
        ITerritoryPaymentMethodsConfigRepository paymentMethodsRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(bindingRepository);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(territoryRepository);
        ArgumentNullException.ThrowIfNull(paymentMethodsRepository);
        ArgumentNullException.ThrowIfNull(auditLogger);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _bindingRepository = bindingRepository;
        _catalog = catalog;
        _territoryRepository = territoryRepository;
        _paymentMethodsRepository = paymentMethodsRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Retorna binding existente ou snapshot Off (legado: comércio sem gate fiscal até 62.a).
    /// </summary>
    public async Task<Result<TerritoryFiscalPackBindingView>> GetBindingAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return Result<TerritoryFiscalPackBindingView>.Failure("Territory not found.");
        }

        var binding = await _bindingRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        if (binding is null)
        {
            return Result<TerritoryFiscalPackBindingView>.Success(
                TerritoryFiscalPackBindingView.OffDefault(territoryId));
        }

        return Result<TerritoryFiscalPackBindingView>.Success(TerritoryFiscalPackBindingView.From(binding));
    }

    public async Task<OperationResult<TerritoryFiscalPackBindingView>> UpsertBindingAsync(
        Guid territoryId,
        string packId,
        FiscalPackBindingStatus status,
        string? municipalityIbge,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(status))
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure("Status must be Off or Active.");
        }

        if (!IsValidMunicipalityIbge(municipalityIbge))
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure(
                "Municipality IBGE code must be 7 digits.");
        }

        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure("Territory not found.");
        }

        var pack = _catalog.GetById(packId);
        if (pack is null)
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure($"Unknown fiscal pack '{packId}'.");
        }

        if (status == FiscalPackBindingStatus.Active &&
            pack.Capabilities.Contains("pix", StringComparer.OrdinalIgnoreCase))
        {
            var paymentMethods = await _paymentMethodsRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);
            if (paymentMethods is null || !paymentMethods.HasMethod(TerritoryPaymentMethodKind.Pix))
            {
                return OperationResult<TerritoryFiscalPackBindingView>.Failure(
                    $"Activating {pack.Id} requires PIX enabled in territory payment methods.");
            }
        }

        var now = DateTimeOffset.UtcNow;
        var existing = await _bindingRepository.GetByTerritoryIdAsync(territoryId, cancellationToken);

        TerritoryFiscalPackBinding binding;
        try
        {
            if (existing is null)
            {
                binding = status == FiscalPackBindingStatus.Active
                    ? new TerritoryFiscalPackBinding(
                        Guid.NewGuid(),
                        territoryId,
                        pack.Id,
                        FiscalPackBindingStatus.Active,
                        actorUserId,
                        now,
                        municipalityIbge,
                        now)
                    : new TerritoryFiscalPackBinding(
                        Guid.NewGuid(),
                        territoryId,
                        pack.Id,
                        FiscalPackBindingStatus.Off,
                        null,
                        null,
                        municipalityIbge,
                        now);

                await _bindingRepository.AddAsync(binding, cancellationToken);
                await _auditLogger.LogAsync(
                    new AuditEntry("fiscal.pack.binding.created", actorUserId, territoryId, binding.Id, DateTime.UtcNow),
                    cancellationToken);
            }
            else
            {
                existing.ChangePack(pack.Id, now);
                if (status == FiscalPackBindingStatus.Active)
                {
                    existing.Activate(actorUserId, now, municipalityIbge);
                }
                else
                {
                    existing.Deactivate(now);
                }

                await _bindingRepository.UpdateAsync(existing, cancellationToken);
                binding = existing;
                await _auditLogger.LogAsync(
                    new AuditEntry("fiscal.pack.binding.updated", actorUserId, territoryId, binding.Id, DateTime.UtcNow),
                    cancellationToken);
            }
        }
        catch (ArgumentException ex)
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return OperationResult<TerritoryFiscalPackBindingView>.Failure(ex.Message);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<TerritoryFiscalPackBindingView>.Success(TerritoryFiscalPackBindingView.From(binding));
    }

    private static bool IsValidMunicipalityIbge(string? municipalityIbge)
    {
        if (string.IsNullOrWhiteSpace(municipalityIbge))
        {
            return true;
        }

        var trimmed = municipalityIbge.Trim();
        return trimmed.Length == 7 && trimmed.All(char.IsDigit);
    }
}

public sealed record TerritoryFiscalPackBindingView(
    Guid TerritoryId,
    string? PackId,
    string Status,
    Guid? ActivatedByUserId,
    DateTimeOffset? ActivatedAtUtc,
    string? MunicipalityIbge,
    bool IsLegacyDefault)
{
    public static TerritoryFiscalPackBindingView OffDefault(Guid territoryId) =>
        new(territoryId, null, FiscalPackBindingStatus.Off.ToString(), null, null, null, true);

    public static TerritoryFiscalPackBindingView From(TerritoryFiscalPackBinding binding) =>
        new(
            binding.TerritoryId,
            binding.PackId,
            binding.Status.ToString(),
            binding.ActivatedByUserId,
            binding.ActivatedAtUtc,
            binding.MunicipalityIbge,
            false);
}
