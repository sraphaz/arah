namespace Araponga.Bff.Journeys;

/// <summary>
/// Registro de todas as jornadas expostas pelo BFF (proxy para a API principal).
/// Base path: /api/v2/journeys/
/// </summary>
public static class BffJourneyRegistry
{
    /// <summary>Prefixo base das jornadas na API.</summary>
    public const string BasePath = "/api/v2/journeys/";

    /// <summary>Jornada de onboarding e primeiro acesso.</summary>
    public const string Onboarding = "onboarding";

    /// <summary>Jornada do feed (posts, interações).</summary>
    public const string Feed = "feed";

    /// <summary>Jornada de eventos (lista, criar, participar).</summary>
    public const string Events = "events";

    /// <summary>Jornada do marketplace (busca, carrinho, checkout).</summary>
    public const string Marketplace = "marketplace";

    /// <summary>Todos os endpoints de cada jornada (GET e POST) para documentação.</summary>
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<JourneyEndpoint>> AllEndpoints =
        new Dictionary<string, IReadOnlyList<JourneyEndpoint>>(StringComparer.OrdinalIgnoreCase)
        {
            [Onboarding] = new List<JourneyEndpoint>
            {
                new("suggested-territories", "GET", "Territórios sugeridos por localização (latitude, longitude, radiusKm)."),
                new("complete", "POST", "Completa o onboarding: seleciona território e retorna contexto inicial. Requer X-Session-Id.")
            },
            [Feed] = new List<JourneyEndpoint>
            {
                new("territory-feed", "GET", "Feed do território (posts, contadores). Query: territoryId, pageNumber, pageSize, filterByInterests, mapEntityId, assetId."),
                new("create-post", "POST", "Cria um post no território. Query: territoryId. Body: título, conteúdo, tipo, visibilidade, mediaIds."),
                new("interact", "POST", "Interage com um post (like, comment, share). Body: postId, territoryId, action, commentContent (opcional).")
            },
            [Events] = new List<JourneyEndpoint>
            {
                new("territory-events", "GET", "Eventos do território. Query: territoryId, from, to, status, pageNumber, pageSize."),
                new("create-event", "POST", "Cria um evento no território. Body: territoryId, título, descrição, datas, local, etc."),
                new("participate", "POST", "Marca interesse ou confirma participação. Body: eventId, status.")
            },
            [Marketplace] = new List<JourneyEndpoint>
            {
                new("search", "GET", "Busca itens no marketplace. Query: territoryId, query, category, type, minPrice, maxPrice, pageNumber, pageSize."),
                new("add-to-cart", "POST", "Adiciona item ao carrinho. Body: territoryId, itemId, quantity, notes."),
                new("checkout", "POST", "Finaliza o checkout do carrinho. Body: territoryId, message.")
            }
        };

    /// <summary>Paths completos (pathAndQuery) dos endpoints GET cacheáveis por jornada.</summary>
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<JourneyEndpoint>> CacheableGetEndpoints =
        new Dictionary<string, IReadOnlyList<JourneyEndpoint>>(StringComparer.OrdinalIgnoreCase)
        {
            [Onboarding] = new List<JourneyEndpoint>
            {
                new("suggested-territories", "GET", "Territórios sugeridos por localização.")
            },
            [Feed] = new List<JourneyEndpoint>
            {
                new("territory-feed", "GET", "Feed do território (posts, contadores).")
            },
            [Events] = new List<JourneyEndpoint>
            {
                new("territory-events", "GET", "Eventos do território.")
            },
            [Marketplace] = new List<JourneyEndpoint>
            {
                new("search", "GET", "Busca de itens no marketplace.")
            }
        };

    /// <summary>Todos os path prefixes de jornada (para validação e TTL).</summary>
    public static IReadOnlyList<string> AllPathPrefixes { get; } = new[]
    {
        Onboarding,
        Feed,
        Events,
        Marketplace
    };

    /// <summary>Retorna pathAndQuery esperado para a API (ex: onboarding/suggested-territories).</summary>
    public static string PathAndQuery(string journeyPrefix, string subPath)
    {
        var p = journeyPrefix.TrimEnd('/');
        var s = subPath.TrimStart('/');
        return string.IsNullOrEmpty(s) ? p : $"{p}/{s}";
    }
}

/// <summary>Endpoint de jornada para documentação.</summary>
public sealed record JourneyEndpoint(string Path, string Method, string Description);
