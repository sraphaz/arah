namespace Araponga.Domain.Marketplace;

/// <summary>
/// Representa uma loja/comércio no território para operação econômica de um morador.
/// Store não é um TerritoryAsset - representa operação econômica, não recurso do território.
/// Stores são criadas apenas por moradores validados (RESIDENT, VALIDATED) e podem ser suspensas
/// se o membership for revogado, mas os dados não são deletados.
/// </summary>
public sealed class Store
{
    public Store(
        Guid id,
        Guid territoryId,
        Guid ownerUserId,
        string displayName,
        string? description,
        StoreStatus status,
        bool paymentsEnabled,
        StoreContactVisibility contactVisibility,
        string? phone,
        string? whatsapp,
        string? email,
        string? instagram,
        string? website,
        string? preferredContactMethod,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("Owner user ID is required.", nameof(ownerUserId));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        Id = id;
        TerritoryId = territoryId;
        OwnerUserId = ownerUserId;
        DisplayName = displayName.Trim();
        Description = description;
        Status = status;
        PaymentsEnabled = paymentsEnabled;
        ContactVisibility = contactVisibility;
        Phone = phone;
        Whatsapp = whatsapp;
        Email = email;
        Instagram = instagram;
        Website = website;
        PreferredContactMethod = preferredContactMethod;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid OwnerUserId { get; }
    public string DisplayName { get; private set; }
    public string? Description { get; private set; }
    public StoreStatus Status { get; private set; }
    public bool PaymentsEnabled { get; private set; }
    public StoreContactVisibility ContactVisibility { get; private set; }
    public string? Phone { get; private set; }
    public string? Whatsapp { get; private set; }
    public string? Email { get; private set; }
    public string? Instagram { get; private set; }
    public string? Website { get; private set; }
    public string? PreferredContactMethod { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateDetails(
        string displayName,
        string? description,
        StoreContactVisibility contactVisibility,
        string? phone,
        string? whatsapp,
        string? email,
        string? instagram,
        string? website,
        string? preferredContactMethod,
        DateTime updatedAtUtc)
    {
        DisplayName = displayName.Trim();
        Description = description;
        ContactVisibility = contactVisibility;
        Phone = phone;
        Whatsapp = whatsapp;
        Email = email;
        Instagram = instagram;
        Website = website;
        PreferredContactMethod = preferredContactMethod;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetStatus(StoreStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetPaymentsEnabled(bool enabled, DateTime updatedAtUtc)
    {
        PaymentsEnabled = enabled;
        UpdatedAtUtc = updatedAtUtc;
    }
}
