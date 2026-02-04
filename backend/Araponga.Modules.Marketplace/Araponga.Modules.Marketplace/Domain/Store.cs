namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Representa uma loja/comércio no território para operação econômica de um morador.
/// Store não é um TerritoryAsset - representa operação econômica, não recurso do território.
/// Stores são criadas apenas por moradores validados (RESIDENT, VALIDATED) e podem ser suspensas
/// se o membership for revogado, mas os dados não são deletados.
/// </summary>
public sealed class Store
{
    /// <summary>
    /// Inicializa uma nova instância de Store.
    /// </summary>
    /// <param name="id">Identificador único da loja</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="ownerUserId">Identificador do usuário dono da loja</param>
    /// <param name="displayName">Nome de exibição da loja</param>
    /// <param name="description">Descrição da loja</param>
    /// <param name="status">Status da loja (Active, Paused, Archived)</param>
    /// <param name="paymentsEnabled">Indica se a loja aceita pagamentos</param>
    /// <param name="contactVisibility">Visibilidade das informações de contato</param>
    /// <param name="phone">Telefone de contato</param>
    /// <param name="whatsapp">WhatsApp de contato</param>
    /// <param name="email">E-mail de contato</param>
    /// <param name="instagram">Instagram da loja</param>
    /// <param name="website">Website da loja</param>
    /// <param name="preferredContactMethod">Método de contato preferido</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
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

    /// <summary>
    /// Identificador único da loja.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde a loja está localizada.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador do usuário dono da loja.
    /// </summary>
    public Guid OwnerUserId { get; }

    /// <summary>
    /// Nome de exibição da loja.
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Descrição da loja.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Status da loja (Active, Paused, Archived).
    /// </summary>
    public StoreStatus Status { get; private set; }

    /// <summary>
    /// Indica se a loja aceita pagamentos através da plataforma.
    /// </summary>
    public bool PaymentsEnabled { get; private set; }

    /// <summary>
    /// Visibilidade das informações de contato (Public, ResidentsOnly, Private).
    /// </summary>
    public StoreContactVisibility ContactVisibility { get; private set; }

    /// <summary>
    /// Telefone de contato da loja.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// WhatsApp de contato da loja.
    /// </summary>
    public string? Whatsapp { get; private set; }

    /// <summary>
    /// E-mail de contato da loja.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Instagram da loja.
    /// </summary>
    public string? Instagram { get; private set; }

    /// <summary>
    /// Website da loja.
    /// </summary>
    public string? Website { get; private set; }

    /// <summary>
    /// Método de contato preferido da loja.
    /// </summary>
    public string? PreferredContactMethod { get; private set; }

    /// <summary>
    /// Data de criação da loja.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização da loja.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza os detalhes da loja.
    /// </summary>
    /// <param name="displayName">Novo nome de exibição</param>
    /// <param name="description">Nova descrição</param>
    /// <param name="contactVisibility">Nova visibilidade de contato</param>
    /// <param name="phone">Novo telefone</param>
    /// <param name="whatsapp">Novo WhatsApp</param>
    /// <param name="email">Novo e-mail</param>
    /// <param name="instagram">Novo Instagram</param>
    /// <param name="website">Novo website</param>
    /// <param name="preferredContactMethod">Novo método de contato preferido</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
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

    /// <summary>
    /// Atualiza o status da loja.
    /// </summary>
    /// <param name="status">Novo status</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void SetStatus(StoreStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Define se a loja aceita pagamentos através da plataforma.
    /// </summary>
    /// <param name="enabled">True para habilitar pagamentos, false para desabilitar</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void SetPaymentsEnabled(bool enabled, DateTime updatedAtUtc)
    {
        PaymentsEnabled = enabled;
        UpdatedAtUtc = updatedAtUtc;
    }
}
