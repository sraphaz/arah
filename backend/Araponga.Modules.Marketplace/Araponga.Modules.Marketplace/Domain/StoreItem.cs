namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Representa um produto ou serviço oferecido por um morador através de uma Store.
/// StoreItem não é um TerritoryAsset e não pode vender ou transferir TerritoryAssets.
/// Items podem referenciar TerritoryAssets contextualmente (ex.: "Serviço de guia na trilha X"),
/// mas isso não implica propriedade ou venda do asset.
/// </summary>
public sealed class StoreItem
{
    /// <summary>
    /// Inicializa uma nova instância de StoreItem.
    /// </summary>
    /// <param name="id">Identificador único do item</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="storeId">Identificador da loja</param>
    /// <param name="type">Tipo do item (Product ou Service)</param>
    /// <param name="title">Título do item</param>
    /// <param name="description">Descrição do item</param>
    /// <param name="category">Categoria do item</param>
    /// <param name="tags">Tags do item (separadas por vírgula)</param>
    /// <param name="pricingType">Tipo de preço (Fixed, Negotiable, Free)</param>
    /// <param name="priceAmount">Valor do preço</param>
    /// <param name="currency">Moeda (ex: BRL, USD)</param>
    /// <param name="unit">Unidade de medida (ex: kg, unidade, hora)</param>
    /// <param name="latitude">Latitude da localização do item</param>
    /// <param name="longitude">Longitude da localização do item</param>
    /// <param name="status">Status do item (Active, Archived)</param>
    /// <param name="createdAtUtc">Data de criação</param>
    /// <param name="updatedAtUtc">Data da última atualização</param>
    public StoreItem(
        Guid id,
        Guid territoryId,
        Guid storeId,
        ItemType type,
        string title,
        string? description,
        string? category,
        string? tags,
        ItemPricingType pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ItemStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Id = id;
        TerritoryId = territoryId;
        StoreId = storeId;
        Type = type;
        Title = title.Trim();
        Description = description;
        Category = category;
        Tags = tags;
        PricingType = pricingType;
        PriceAmount = priceAmount;
        Currency = currency;
        Unit = unit;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Identificador único do item.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do território onde o item está disponível.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Identificador da loja que oferece o item.
    /// </summary>
    public Guid StoreId { get; }

    /// <summary>
    /// Tipo do item (Product ou Service).
    /// </summary>
    public ItemType Type { get; private set; }

    /// <summary>
    /// Título do item.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Descrição detalhada do item.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Categoria do item.
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// Tags do item (separadas por vírgula).
    /// </summary>
    public string? Tags { get; private set; }

    /// <summary>
    /// Tipo de preço (Fixed, Negotiable, Free).
    /// </summary>
    public ItemPricingType PricingType { get; private set; }

    /// <summary>
    /// Valor do preço.
    /// </summary>
    public decimal? PriceAmount { get; private set; }

    /// <summary>
    /// Moeda (ex: BRL, USD).
    /// </summary>
    public string? Currency { get; private set; }

    /// <summary>
    /// Unidade de medida (ex: kg, unidade, hora).
    /// </summary>
    public string? Unit { get; private set; }

    /// <summary>
    /// Latitude da localização do item.
    /// </summary>
    public double? Latitude { get; private set; }

    /// <summary>
    /// Longitude da localização do item.
    /// </summary>
    public double? Longitude { get; private set; }

    /// <summary>
    /// Status do item (Active, Archived).
    /// </summary>
    public ItemStatus Status { get; private set; }

    /// <summary>
    /// Data de criação do item.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Data da última atualização do item.
    /// </summary>
    public DateTime UpdatedAtUtc { get; private set; }

    /// <summary>
    /// Atualiza os detalhes do item.
    /// </summary>
    /// <param name="type">Novo tipo</param>
    /// <param name="title">Novo título</param>
    /// <param name="description">Nova descrição</param>
    /// <param name="category">Nova categoria</param>
    /// <param name="tags">Novas tags</param>
    /// <param name="pricingType">Novo tipo de preço</param>
    /// <param name="priceAmount">Novo valor do preço</param>
    /// <param name="currency">Nova moeda</param>
    /// <param name="unit">Nova unidade</param>
    /// <param name="latitude">Nova latitude</param>
    /// <param name="longitude">Nova longitude</param>
    /// <param name="status">Novo status</param>
    /// <param name="updatedAtUtc">Data da atualização</param>
    public void UpdateDetails(
        ItemType type,
        string title,
        string? description,
        string? category,
        string? tags,
        ItemPricingType pricingType,
        decimal? priceAmount,
        string? currency,
        string? unit,
        double? latitude,
        double? longitude,
        ItemStatus status,
        DateTime updatedAtUtc)
    {
        Type = type;
        Title = title.Trim();
        Description = description;
        Category = category;
        Tags = tags;
        PricingType = pricingType;
        PriceAmount = priceAmount;
        Currency = currency;
        Unit = unit;
        Latitude = latitude;
        Longitude = longitude;
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }

    /// <summary>
    /// Arquiva o item (muda status para Archived).
    /// </summary>
    /// <param name="updatedAtUtc">Data da arquivação</param>
    public void Archive(DateTime updatedAtUtc)
    {
        Status = ItemStatus.Archived;
        UpdatedAtUtc = updatedAtUtc;
    }
}
