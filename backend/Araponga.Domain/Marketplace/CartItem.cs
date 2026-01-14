namespace Araponga.Domain.Marketplace;

/// <summary>
/// Representa um item adicionado ao carrinho de compras.
/// </summary>
public sealed class CartItem
{
    /// <summary>
    /// Inicializa uma nova instância de CartItem.
    /// </summary>
    /// <param name="id">Identificador único do item do carrinho</param>
    /// <param name="cartId">Identificador do carrinho</param>
    /// <param name="itemId">Identificador do item (produto ou serviço)</param>
    /// <param name="quantity">Quantidade do item</param>
    /// <param name="notes">Notas adicionais sobre o item</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public CartItem(
        Guid id,
        Guid cartId,
        Guid itemId,
        int quantity,
        string? notes,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (cartId == Guid.Empty)
        {
            throw new ArgumentException("Cart ID is required.", nameof(cartId));
        }

        if (itemId == Guid.Empty)
        {
            throw new ArgumentException("Item ID is required.", nameof(itemId));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        }

        Id = id;
        CartId = cartId;
        ItemId = itemId;
        Quantity = quantity;
        Notes = notes;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único do item do carrinho.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do carrinho ao qual o item pertence.
    /// </summary>
    public Guid CartId { get; }

    /// <summary>
    /// Identificador do item (produto ou serviço) adicionado ao carrinho.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Quantidade do item.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Notas adicionais sobre o item.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Data de criação do item no carrinho.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização do item.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza a quantidade e notas do item.
    /// </summary>
    /// <param name="quantity">Nova quantidade</param>
    /// <param name="notes">Novas notas</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void Update(int quantity, string? notes, DateTime updatedAtUtc)
    {
        Quantity = quantity;
        Notes = notes;
        UpdatedAtUtc = updatedAtUtc;
    }
}
