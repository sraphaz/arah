namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Representa uma inquiry (consulta) sobre um item do marketplace.
/// Permite que usuários façam perguntas ou solicitem informações sobre produtos ou serviços.
/// </summary>
public sealed class ItemInquiry
{
    /// <summary>
    /// Inicializa uma nova instância de ItemInquiry.
    /// </summary>
    /// <param name="id">Identificador único da inquiry</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="itemId">Identificador do item (produto ou serviço)</param>
    /// <param name="storeId">Identificador da loja</param>
    /// <param name="fromUserId">Identificador do usuário que criou a inquiry</param>
    /// <param name="message">Mensagem da inquiry</param>
    /// <param name="status">Status da inquiry</param>
    /// <param name="batchId">Identificador do lote (para agrupar inquiries relacionadas)</param>
    /// <param name="createdAtUtc">Data de criação</param>
    public ItemInquiry(
        Guid id,
        Guid territoryId,
        Guid itemId,
        Guid storeId,
        Guid fromUserId,
        string? message,
        InquiryStatus status,
        Guid? batchId,
        DateTime createdAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (itemId == Guid.Empty)
        {
            throw new ArgumentException("Item ID is required.", nameof(itemId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        if (fromUserId == Guid.Empty)
        {
            throw new ArgumentException("From user ID is required.", nameof(fromUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        ItemId = itemId;
        StoreId = storeId;
        FromUserId = fromUserId;
        Message = message;
        Status = status;
        BatchId = batchId;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Identificador único da inquiry.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde o item está localizado.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador do item (produto ou serviço) sobre o qual a inquiry foi feita.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Identificador da loja que oferece o item.
    /// </summary>
    public Guid StoreId { get; }

    /// <summary>
    /// Identificador do usuário que criou a inquiry.
    /// </summary>
    public Guid FromUserId { get; }

    /// <summary>
    /// Mensagem da inquiry.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Status da inquiry (Open, Closed).
    /// </summary>
    public InquiryStatus Status { get; private set; }

    /// <summary>
    /// Identificador do lote (para agrupar inquiries relacionadas, ex: checkout em lote).
    /// </summary>
    public Guid? BatchId { get; }

    /// <summary>
    /// Data de criação da inquiry.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Fecha a inquiry.
    /// </summary>
    public void Close()
    {
        Status = InquiryStatus.Closed;
    }
}
