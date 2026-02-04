namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Representa um item incluído em um checkout.
/// Armazena snapshot dos dados do item no momento do checkout para preservar histórico.
/// </summary>
public sealed class CheckoutItem
{
    /// <summary>
    /// Inicializa uma nova instância de CheckoutItem.
    /// </summary>
    /// <param name="id">Identificador único do item do checkout</param>
    /// <param name="checkoutId">Identificador do checkout</param>
    /// <param name="itemId">Identificador do item (produto ou serviço)</param>
    /// <param name="itemType">Tipo do item (Product ou Service)</param>
    /// <param name="titleSnapshot">Título do item no momento do checkout</param>
    /// <param name="quantity">Quantidade</param>
    /// <param name="unitPrice">Preço unitário</param>
    /// <param name="lineSubtotal">Subtotal da linha (quantidade × preço unitário)</param>
    /// <param name="platformFeeLine">Taxa da plataforma aplicada na linha</param>
    /// <param name="lineTotal">Total da linha (subtotal + taxa da plataforma)</param>
    /// <param name="createdAtUtc">Data de criação</param>
    public CheckoutItem(
        Guid id,
        Guid checkoutId,
        Guid itemId,
        ItemType itemType,
        string titleSnapshot,
        int quantity,
        decimal? unitPrice,
        decimal? lineSubtotal,
        decimal? platformFeeLine,
        decimal? lineTotal,
        DateTime createdAtUtc)
    {
        if (checkoutId == Guid.Empty)
        {
            throw new ArgumentException("Checkout ID is required.", nameof(checkoutId));
        }

        if (itemId == Guid.Empty)
        {
            throw new ArgumentException("Item ID is required.", nameof(itemId));
        }

        if (string.IsNullOrWhiteSpace(titleSnapshot))
        {
            throw new ArgumentException("Title snapshot is required.", nameof(titleSnapshot));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        }

        Id = id;
        CheckoutId = checkoutId;
        ItemId = itemId;
        ItemType = itemType;
        TitleSnapshot = titleSnapshot.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineSubtotal = lineSubtotal;
        PlatformFeeLine = platformFeeLine;
        LineTotal = lineTotal;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Identificador único do item do checkout.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do checkout ao qual o item pertence.
    /// </summary>
    public Guid CheckoutId { get; }

    /// <summary>
    /// Identificador do item (produto ou serviço).
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Tipo do item (Product ou Service).
    /// </summary>
    public ItemType ItemType { get; }

    /// <summary>
    /// Título do item no momento do checkout (snapshot para preservar histórico).
    /// </summary>
    public string TitleSnapshot { get; }

    /// <summary>
    /// Quantidade do item.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Preço unitário do item no momento do checkout.
    /// </summary>
    public decimal? UnitPrice { get; }

    /// <summary>
    /// Subtotal da linha (quantidade × preço unitário).
    /// </summary>
    public decimal? LineSubtotal { get; }

    /// <summary>
    /// Taxa da plataforma aplicada na linha.
    /// </summary>
    public decimal? PlatformFeeLine { get; }

    /// <summary>
    /// Total da linha (subtotal + taxa da plataforma).
    /// </summary>
    public decimal? LineTotal { get; }

    /// <summary>
    /// Data de criação do item no checkout.
    /// </summary>
    public DateTime CreatedAtUtc { get; }
}
