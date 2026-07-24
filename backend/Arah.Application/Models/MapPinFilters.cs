namespace Arah.Application.Models;

/// <summary>
/// Filtros de tipos de pin do mapa, derivados do parâmetro <c>types</c>.
/// </summary>
public readonly record struct MapPinFilters(
    bool Entities,
    bool Assets,
    bool Alerts,
    bool Posts,
    bool Media,
    bool Events)
{
    /// <summary>
    /// Interpreta a lista CSV de tipos. Vazio/ausente habilita todos os tipos.
    /// </summary>
    public static MapPinFilters Parse(string? types)
    {
        var typeSet = ParseTypes(types);
        return new MapPinFilters(
            Entities: typeSet.Contains("entity"),
            Assets: typeSet.Contains("asset"),
            Alerts: typeSet.Contains("alert"),
            Posts: typeSet.Contains("post"),
            Media: typeSet.Contains("media"),
            Events: typeSet.Contains("event"));
    }

    private static IReadOnlyCollection<string> ParseTypes(string? types)
    {
        if (string.IsNullOrWhiteSpace(types))
        {
            return new HashSet<string>(new[] { "post", "media", "entity", "alert", "asset", "event" });
        }

        return new HashSet<string>(
            types.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => value.ToLowerInvariant()));
    }
}
