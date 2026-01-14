namespace Araponga.Api.Contracts.Marketplace;

/// <summary>
/// Request para criar um item (produto ou serviço) no marketplace.
/// </summary>
/// <param name="TerritoryId">Identificador do território</param>
/// <param name="StoreId">Identificador da loja</param>
/// <param name="Type">Tipo do item (Product ou Service)</param>
/// <param name="Title">Título do item</param>
/// <param name="Description">Descrição do item</param>
/// <param name="Category">Categoria do item</param>
/// <param name="Tags">Tags do item (separadas por vírgula)</param>
/// <param name="PricingType">Tipo de preço (Fixed, Negotiable, Free)</param>
/// <param name="PriceAmount">Valor do preço</param>
/// <param name="Currency">Moeda (ex: BRL, USD)</param>
/// <param name="Unit">Unidade de medida (ex: kg, unidade, hora)</param>
/// <param name="Latitude">Latitude da localização do item</param>
/// <param name="Longitude">Longitude da localização do item</param>
/// <param name="Status">Status inicial do item (opcional, padrão: Active)</param>
public sealed record CreateItemRequest(
    Guid TerritoryId,
    Guid StoreId,
    string Type,
    string Title,
    string? Description,
    string? Category,
    string? Tags,
    string PricingType,
    decimal? PriceAmount,
    string? Currency,
    string? Unit,
    double? Latitude,
    double? Longitude,
    string? Status);

/// <summary>
/// Request para atualizar um item existente.
/// </summary>
/// <param name="Type">Tipo do item (Product ou Service)</param>
/// <param name="Title">Título do item</param>
/// <param name="Description">Descrição do item</param>
/// <param name="Category">Categoria do item</param>
/// <param name="Tags">Tags do item (separadas por vírgula)</param>
/// <param name="PricingType">Tipo de preço (Fixed, Negotiable, Free)</param>
/// <param name="PriceAmount">Valor do preço</param>
/// <param name="Currency">Moeda (ex: BRL, USD)</param>
/// <param name="Unit">Unidade de medida (ex: kg, unidade, hora)</param>
/// <param name="Latitude">Latitude da localização do item</param>
/// <param name="Longitude">Longitude da localização do item</param>
/// <param name="Status">Status do item</param>
public sealed record UpdateItemRequest(
    string? Type,
    string? Title,
    string? Description,
    string? Category,
    string? Tags,
    string? PricingType,
    decimal? PriceAmount,
    string? Currency,
    string? Unit,
    double? Latitude,
    double? Longitude,
    string? Status);

/// <summary>
/// Response com os dados de um item do marketplace.
/// </summary>
/// <param name="Id">Identificador único do item</param>
/// <param name="TerritoryId">Identificador do território</param>
/// <param name="StoreId">Identificador da loja</param>
/// <param name="Type">Tipo do item (PRODUCT ou SERVICE)</param>
/// <param name="Title">Título do item</param>
/// <param name="Description">Descrição do item</param>
/// <param name="Category">Categoria do item</param>
/// <param name="Tags">Tags do item</param>
/// <param name="PricingType">Tipo de preço (FIXED, NEGOTIABLE, FREE)</param>
/// <param name="PriceAmount">Valor do preço</param>
/// <param name="Currency">Moeda</param>
/// <param name="Unit">Unidade de medida</param>
/// <param name="Latitude">Latitude da localização</param>
/// <param name="Longitude">Longitude da localização</param>
/// <param name="Status">Status do item (ACTIVE, ARCHIVED)</param>
/// <param name="CreatedAtUtc">Data de criação</param>
/// <param name="UpdatedAtUtc">Data da última atualização</param>
public sealed record ItemResponse(
    Guid Id,
    Guid TerritoryId,
    Guid StoreId,
    string Type,
    string Title,
    string? Description,
    string? Category,
    string? Tags,
    string PricingType,
    decimal? PriceAmount,
    string? Currency,
    string? Unit,
    double? Latitude,
    double? Longitude,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
