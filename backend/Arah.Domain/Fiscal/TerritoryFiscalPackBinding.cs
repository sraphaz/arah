namespace Arah.Domain.Fiscal;

/// <summary>
/// Binding de pacote fiscal escopado por território — Territory permanece geográfico/neutro.
/// </summary>
public sealed class TerritoryFiscalPackBinding
{
    public TerritoryFiscalPackBinding(
        Guid id,
        Guid territoryId,
        string packId,
        FiscalPackBindingStatus status,
        Guid? activatedByUserId,
        DateTimeOffset? activatedAtUtc,
        string? municipalityIbge,
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

        if (string.IsNullOrWhiteSpace(packId))
        {
            throw new ArgumentException("Pack id is required.", nameof(packId));
        }

        if (status == FiscalPackBindingStatus.Active && activatedByUserId is null)
        {
            throw new ArgumentException("ActivatedByUserId is required when status is Active.", nameof(activatedByUserId));
        }

        if (status == FiscalPackBindingStatus.Active && activatedAtUtc is null)
        {
            throw new ArgumentException("ActivatedAtUtc is required when status is Active.", nameof(activatedAtUtc));
        }

        Id = id;
        TerritoryId = territoryId;
        PackId = packId.Trim().ToLowerInvariant();
        Status = status;
        ActivatedByUserId = activatedByUserId;
        ActivatedAtUtc = activatedAtUtc;
        MunicipalityIbge = NormalizeMunicipalityIbge(municipalityIbge);
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string PackId { get; private set; }
    public FiscalPackBindingStatus Status { get; private set; }
    public Guid? ActivatedByUserId { get; private set; }
    public DateTimeOffset? ActivatedAtUtc { get; private set; }
    public string? MunicipalityIbge { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public bool IsActive => Status == FiscalPackBindingStatus.Active;

    public void Activate(Guid activatedByUserId, DateTimeOffset activatedAtUtc, string? municipalityIbge)
    {
        if (activatedByUserId == Guid.Empty)
        {
            throw new ArgumentException("ActivatedByUserId is required.", nameof(activatedByUserId));
        }

        Status = FiscalPackBindingStatus.Active;
        ActivatedByUserId = activatedByUserId;
        ActivatedAtUtc = activatedAtUtc;
        MunicipalityIbge = NormalizeMunicipalityIbge(municipalityIbge);
        UpdatedAtUtc = activatedAtUtc;
    }

    public void Deactivate(DateTimeOffset deactivatedAtUtc)
    {
        Status = FiscalPackBindingStatus.Off;
        UpdatedAtUtc = deactivatedAtUtc;
    }

    public void ChangePack(string packId, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(packId))
        {
            throw new ArgumentException("Pack id is required.", nameof(packId));
        }

        PackId = packId.Trim().ToLowerInvariant();
        UpdatedAtUtc = updatedAtUtc;
    }

    private static string? NormalizeMunicipalityIbge(string? municipalityIbge)
    {
        if (string.IsNullOrWhiteSpace(municipalityIbge))
        {
            return null;
        }

        var trimmed = municipalityIbge.Trim();
        if (trimmed.Length != 7 || !trimmed.All(char.IsDigit))
        {
            throw new ArgumentException("Municipality IBGE code must be 7 digits.", nameof(municipalityIbge));
        }

        return trimmed;
    }
}
