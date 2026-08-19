namespace Arah.Domain.Fiscal;

/// <summary>
/// Configuração de meios de pagamento habilitados por território.
/// </summary>
public sealed class TerritoryPaymentMethodsConfig
{
    public TerritoryPaymentMethodsConfig(
        Guid id,
        Guid territoryId,
        IReadOnlyList<TerritoryPaymentMethodKind> methods,
        string? pspProvider,
        DateTimeOffset updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        ArgumentNullException.ThrowIfNull(methods);

        if (!string.IsNullOrWhiteSpace(pspProvider) && pspProvider.Trim().Length > 64)
        {
            throw new ArgumentException("PspProvider must be at most 64 characters.", nameof(pspProvider));
        }

        Id = id;
        TerritoryId = territoryId;
        Methods = methods.Distinct().OrderBy(m => (int)m).ToArray();
        PspProvider = string.IsNullOrWhiteSpace(pspProvider) ? null : pspProvider.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public IReadOnlyList<TerritoryPaymentMethodKind> Methods { get; private set; }
    public string? PspProvider { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public bool HasMethod(TerritoryPaymentMethodKind kind) => Methods.Contains(kind);

    public void ReplaceMethods(
        IReadOnlyList<TerritoryPaymentMethodKind> methods,
        string? pspProvider,
        DateTimeOffset updatedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(methods);

        if (!string.IsNullOrWhiteSpace(pspProvider) && pspProvider.Trim().Length > 64)
        {
            throw new ArgumentException("PspProvider must be at most 64 characters.", nameof(pspProvider));
        }

        Methods = methods.Distinct().OrderBy(m => (int)m).ToArray();
        PspProvider = string.IsNullOrWhiteSpace(pspProvider) ? null : pspProvider.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }
}
