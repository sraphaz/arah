namespace Araponga.Api.Contracts.Marketplace;

/// <summary>
/// Request para criar ou atualizar uma configuração de taxa da plataforma.
/// </summary>
/// <param name="TerritoryId">Identificador do território</param>
/// <param name="ItemType">Tipo de item (Product ou Service)</param>
/// <param name="FeeMode">Modo de taxa (Percentage ou Fixed)</param>
/// <param name="FeeValue">Valor da taxa</param>
/// <param name="Currency">Moeda (ex: BRL, USD)</param>
/// <param name="IsActive">Indica se a configuração está ativa</param>
public sealed record PlatformFeeRequest(
    Guid TerritoryId,
    string ItemType,
    string FeeMode,
    decimal FeeValue,
    string? Currency,
    bool IsActive);

/// <summary>
/// Response com os dados de uma configuração de taxa da plataforma.
/// </summary>
/// <param name="Id">Identificador único da configuração</param>
/// <param name="TerritoryId">Identificador do território</param>
/// <param name="ItemType">Tipo de item (PRODUCT ou SERVICE)</param>
/// <param name="FeeMode">Modo de taxa (PERCENTAGE ou FIXED)</param>
/// <param name="FeeValue">Valor da taxa</param>
/// <param name="Currency">Moeda</param>
/// <param name="IsActive">Indica se a configuração está ativa</param>
/// <param name="CreatedAtUtc">Data de criação</param>
/// <param name="UpdatedAtUtc">Data da última atualização</param>
public sealed record PlatformFeeResponse(
    Guid Id,
    Guid TerritoryId,
    string ItemType,
    string FeeMode,
    decimal FeeValue,
    string? Currency,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
