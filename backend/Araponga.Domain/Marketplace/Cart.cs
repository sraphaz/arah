namespace Araponga.Domain.Marketplace;

/// <summary>
/// Representa o carrinho de compras de um usuário em um território.
/// Cada usuário pode ter apenas um carrinho ativo por território.
/// </summary>
public sealed class Cart
{
    /// <summary>
    /// Inicializa uma nova instância de Cart.
    /// </summary>
    /// <param name="id">Identificador único do carrinho</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="userId">Identificador do usuário dono do carrinho</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public Cart(
        Guid id,
        Guid territoryId,
        Guid userId,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        Id = id;
        TerritoryId = territoryId;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único do carrinho.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde o carrinho está ativo.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador do usuário dono do carrinho.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Data de criação do carrinho.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização do carrinho.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza o timestamp de modificação do carrinho.
    /// </summary>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void Touch(DateTime updatedAtUtc)
    {
        UpdatedAtUtc = updatedAtUtc;
    }
}
