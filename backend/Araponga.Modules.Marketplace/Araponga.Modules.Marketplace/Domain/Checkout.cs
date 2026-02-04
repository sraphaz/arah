namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Representa um checkout (finalização de compra) realizado por um usuário.
/// Contém informações sobre os itens comprados, valores e status do pagamento.
/// </summary>
public sealed class Checkout
{
    /// <summary>
    /// Inicializa uma nova instância de Checkout.
    /// </summary>
    /// <param name="id">Identificador único do checkout</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="buyerUserId">Identificador do usuário comprador</param>
    /// <param name="storeId">Identificador da loja</param>
    /// <param name="status">Status do checkout</param>
    /// <param name="currency">Moeda utilizada (ex: BRL, USD)</param>
    /// <param name="itemsSubtotalAmount">Subtotal dos itens (sem taxa da plataforma)</param>
    /// <param name="platformFeeAmount">Valor da taxa da plataforma</param>
    /// <param name="totalAmount">Valor total (subtotal + taxa da plataforma)</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public Checkout(
        Guid id,
        Guid territoryId,
        Guid buyerUserId,
        Guid storeId,
        CheckoutStatus status,
        string currency,
        decimal? itemsSubtotalAmount,
        decimal? platformFeeAmount,
        decimal? totalAmount,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (buyerUserId == Guid.Empty)
        {
            throw new ArgumentException("Buyer user ID is required.", nameof(buyerUserId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        Id = id;
        TerritoryId = territoryId;
        BuyerUserId = buyerUserId;
        StoreId = storeId;
        Status = status;
        Currency = currency;
        ItemsSubtotalAmount = itemsSubtotalAmount;
        PlatformFeeAmount = platformFeeAmount;
        TotalAmount = totalAmount;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único do checkout.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde o checkout foi realizado.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador do usuário que realizou o checkout.
    /// </summary>
    public Guid BuyerUserId { get; }

    /// <summary>
    /// Identificador da loja da qual os itens foram comprados.
    /// </summary>
    public Guid StoreId { get; }

    /// <summary>
    /// Status do checkout (Pending, Completed, Cancelled).
    /// </summary>
    public CheckoutStatus Status { get; private set; }

    /// <summary>
    /// Moeda utilizada no checkout (ex: BRL, USD).
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Subtotal dos itens (sem incluir taxa da plataforma).
    /// </summary>
    public decimal? ItemsSubtotalAmount { get; private set; }

    /// <summary>
    /// Valor da taxa da plataforma aplicada ao checkout.
    /// </summary>
    public decimal? PlatformFeeAmount { get; private set; }

    /// <summary>
    /// Valor total do checkout (subtotal + taxa da plataforma).
    /// </summary>
    public decimal? TotalAmount { get; private set; }

    /// <summary>
    /// Data de criação do checkout.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização do checkout.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Define os valores totais do checkout.
    /// </summary>
    /// <param name="itemsSubtotal">Subtotal dos itens</param>
    /// <param name="platformFee">Taxa da plataforma</param>
    /// <param name="totalAmount">Valor total</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void SetTotals(decimal itemsSubtotal, decimal platformFee, decimal totalAmount, DateTime updatedAtUtc)
    {
        ItemsSubtotalAmount = itemsSubtotal;
        PlatformFeeAmount = platformFee;
        TotalAmount = totalAmount;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Atualiza o status do checkout.
    /// </summary>
    /// <param name="status">Novo status</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void SetStatus(CheckoutStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }
}
