namespace Araponga.Domain.Membership;

/// <summary>
/// Representa configurações e opt-ins do membro dentro de um território.
/// Relacionamento 1:1 com TerritoryMembership.
/// Criado automaticamente ao criar um Membership.
/// </summary>
public sealed class MembershipSettings
{
    /// <summary>
    /// Inicializa uma nova instância de MembershipSettings.
    /// </summary>
    /// <param name="membershipId">Identificador do membership associado</param>
    /// <param name="marketplaceOptIn">Indica se o membro optou por participar do marketplace</param>
    /// <param name="createdAtUtc">Data de criação das configurações</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public MembershipSettings(
        Guid membershipId,
        bool marketplaceOptIn,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (membershipId == Guid.Empty)
        {
            throw new ArgumentException("Membership ID is required.", nameof(membershipId));
        }

        MembershipId = membershipId;
        MarketplaceOptIn = marketplaceOptIn;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador do membership associado (chave primária).
    /// </summary>
    public Guid MembershipId { get; }

    /// <summary>
    /// Indica se o membro optou por participar do marketplace do território.
    /// </summary>
    public bool MarketplaceOptIn { get; private set; }

    /// <summary>
    /// Data de criação das configurações.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização das configurações.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza a opção de participação no marketplace.
    /// </summary>
    /// <param name="optIn">Nova opção de participação</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void UpdateMarketplaceOptIn(bool optIn, DateTime updatedAtUtc)
    {
        MarketplaceOptIn = optIn;
        UpdatedAtUtc = updatedAtUtc;
    }
}
