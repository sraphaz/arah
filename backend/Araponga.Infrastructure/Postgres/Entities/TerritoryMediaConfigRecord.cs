namespace Araponga.Infrastructure.Postgres.Entities;

/// <summary>
/// Registro Postgres para configuração de mídia por território.
/// Sub-objetos (Posts, Events, Marketplace, Chat) armazenados em jsonb.
/// </summary>
public sealed class TerritoryMediaConfigRecord
{
    public Guid TerritoryId { get; set; }
    public string PostsJson { get; set; } = "{}";
    public string EventsJson { get; set; } = "{}";
    public string MarketplaceJson { get; set; } = "{}";
    public string ChatJson { get; set; } = "{}";
    public DateTime UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
