namespace Arah.Bff.Journeys;

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

    /// <summary>Jornada de autenticação (login, refresh, logout, 2FA).</summary>
    public const string Auth = "auth";

    /// <summary>Jornada do usuário logado (perfil, preferências, interesses, devices).</summary>
    public const string Me = "me";

    /// <summary>Jornada de conexões (círculos/amigos: listar, solicitar, aceitar, remover).</summary>
    public const string Connections = "connections";

    /// <summary>Jornada de territórios (listar, detalhe, entrar, features).</summary>
    public const string Territories = "territories";

    /// <summary>Jornada de membership (become-resident, verify-residency, me, transfer).</summary>
    public const string Membership = "membership";

    /// <summary>Jornada de mapas (entidades, pins, camadas).</summary>
    public const string Map = "map";

    /// <summary>Jornada de assets (mídia, anexos: listar, upload, curate).</summary>
    public const string Assets = "assets";

    /// <summary>Jornada de mídia (upload, download, info).</summary>
    public const string Media = "media";

    /// <summary>Jornada de planos de assinatura (listar, detalhe).</summary>
    public const string SubscriptionPlans = "subscription-plans";

    /// <summary>Jornada de assinaturas (minha assinatura, capacidades, cancelar, reativar).</summary>
    public const string Subscriptions = "subscriptions";

    /// <summary>Jornada de notificações (listar, paginado, marcar lida).</summary>
    public const string Notifications = "notifications";

    /// <summary>Jornada marketplace v1 (carrinho, lojas, itens além da journey marketplace).</summary>
    public const string MarketplaceV1 = "marketplace-v1";

    /// <summary>Jornada de moderação (work-items, cases, evidências por território).</summary>
    public const string Moderation = "moderation";

    /// <summary>Jornada de chat (conversas, mensagens, participantes).</summary>
    public const string Chat = "chat";

    /// <summary>Jornada de alertas.</summary>
    public const string Alerts = "alerts";

    /// <summary>Jornada admin (seed, config, cache-metrics, etc.).</summary>
    public const string Admin = "admin";

    /// <summary>Mapeamento jornada → path base na API (sem barra inicial). Jornadas v2: api/v2/journeys/&lt;nome&gt;; v1: api/v1/...</summary>
    public static readonly IReadOnlyDictionary<string, string> JourneyToApiPathBase =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [Onboarding] = "api/v2/journeys/onboarding",
            [Feed] = "api/v2/journeys/feed",
            [Events] = "api/v2/journeys/events",
            [Marketplace] = "api/v2/journeys/marketplace",
            [Auth] = "api/v1/auth",
            [Me] = "api/v1/users/me",
            [Connections] = "api/v1/connections",
            [Territories] = "api/v1/territories",
            [Membership] = "api/v1/memberships",
            [Map] = "api/v1/map",
            [Assets] = "api/v1/assets",
            [Media] = "api/v1/media",
            [SubscriptionPlans] = "api/v1/subscription-plans",
            [Subscriptions] = "api/v1/subscriptions",
            [Notifications] = "api/v1/notifications",
            [MarketplaceV1] = "api/v1",
            [Moderation] = "api/v1/territories",
            [Chat] = "api/v1/chat",
            [Alerts] = "api/v1/alerts",
            [Admin] = "api/v1/admin"
        };

    /// <summary>Retorna o path base na API para a jornada (ex: api/v1/auth). Se a jornada for desconhecida, usa api/v2/journeys/&lt;nome&gt;.</summary>
    public static string GetApiPathBase(string journeyName)
    {
        if (string.IsNullOrWhiteSpace(journeyName))
            return "api/v2/journeys";
        if (JourneyToApiPathBase.TryGetValue(journeyName.Trim(), out var basePath))
            return basePath;
        return $"api/v2/journeys/{journeyName.Trim()}";
    }

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
            },
            [Auth] = new List<JourneyEndpoint>
            {
                new("check-email", "POST", "Verifica se o e-mail já está cadastrado. Body: { \"email\": \"...\" }."),
                new("signup", "POST", "Cadastro com e-mail e senha. Body: email, displayName, password."),
                new("social", "POST", "Login/cadastro social (Google, Apple, etc.). Body: authProvider, externalId, displayName, email, foreignDocument."),
                new("login", "POST", "Login (email/senha ou provedor). Body: credenciais."),
                new("refresh", "POST", "Refresh do token. Body: refreshToken."),
                new("logout", "POST", "Logout. Requer Authorization."),
                new("2fa", "POST", "2FA (enviar/validar). Body conforme API.")
            },
            [Me] = new List<JourneyEndpoint>
            {
                new("", "GET", "Atividade/resumo do usuário logado. Requer Authorization."),
                new("profile", "GET", "Perfil do usuário. Requer Authorization."),
                new("profile", "PUT", "Atualiza perfil. Body: nome, avatar, etc."),
                new("preferences", "GET", "Preferências do usuário."),
                new("preferences", "PUT", "Atualiza preferências."),
                new("interests", "GET", "Interesses do usuário."),
                new("interests", "PUT", "Atualiza interesses."),
                new("devices", "GET", "Dispositivos registrados."),
                new("devices", "POST", "Registra dispositivo.")
            },
            [Connections] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista conexões do usuário. Query: territoryId, status."),
                new("pending", "GET", "Lista solicitações pendentes."),
                new("privacy", "GET", "Configurações de privacidade de conexões."),
                new("privacy", "PUT", "Atualiza configurações de privacidade."),
                new("users/search", "GET", "Busca usuários para conectar. Query: query, territoryId."),
                new("suggestions", "GET", "Sugestões de conexão."),
                new("request", "POST", "Envia solicitação de conexão. Body: targetUserId. Query: territoryId."),
                new("{connectionId}/accept", "POST", "Aceita solicitação de conexão."),
                new("{connectionId}/reject", "POST", "Rejeita solicitação."),
                new("{connectionId}", "DELETE", "Remove conexão.")
            },
            [Territories] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista territórios disponíveis."),
                new("paged", "GET", "Lista territórios (paginado). Query: pageNumber, pageSize."),
                new("{id}", "GET", "Detalhe do território por Id."),
                new("{id}/features", "GET", "Features do território."),
                new("{id}/enter", "POST", "Entra no território como visitante.")
            },
            [Membership] = new List<JourneyEndpoint>
            {
                new("me", "GET", "Minhas memberships. Requer Authorization."),
                new("{territoryId}/me", "GET", "Membership do usuário no território."),
                new("{territoryId}/become-resident", "POST", "Solicita residência no território."),
                new("{territoryId}/verify-residency/geo", "POST", "Verifica residência por geo."),
                new("{territoryId}/verify-residency/document", "POST", "Verifica residência por documento."),
                new("transfer-residency", "POST", "Transfere residência. Body conforme API.")
            },
            [Map] = new List<JourneyEndpoint>
            {
                new("entities", "GET", "Entidades do mapa. Query: territoryId, categoryId, bounds, etc."),
                new("entities/paged", "GET", "Entidades paginadas. Query: territoryId, pageNumber, pageSize."),
                new("pins", "GET", "Pins do mapa. Query: territoryId, bounds."),
                new("pins/paged", "GET", "Pins paginados."),
                new("entities", "POST", "Cria entidade no mapa. Body conforme API."),
                new("entities/{entityId}/confirmations", "POST", "Confirma entidade."),
                new("entities/{entityId}/relations", "POST", "Cria relação entre entidades.")
            },
            [Assets] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista assets. Query: territoryId, status, type."),
                new("paged", "GET", "Lista assets paginada. Query: pageNumber, pageSize."),
                new("{assetId}", "GET", "Detalhe do asset por Id."),
                new("", "POST", "Upload de asset. Multipart."),
                new("{assetId}/archive", "POST", "Arquivar asset."),
                new("{assetId}/validate", "POST", "Validar asset."),
                new("{assetId}/curate", "POST", "Curar asset.")
            },
            [Media] = new List<JourneyEndpoint>
            {
                new("upload", "POST", "Upload de mídia. Multipart/form-data."),
                new("{id}", "GET", "Download de mídia por Id."),
                new("{id}/info", "GET", "Info da mídia por Id."),
                new("{id}", "DELETE", "Remove mídia.")
            },
            [SubscriptionPlans] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista planos de assinatura."),
                new("{id}", "GET", "Detalhe do plano por Id.")
            },
            [Subscriptions] = new List<JourneyEndpoint>
            {
                new("me", "GET", "Minha assinatura atual. Requer Authorization."),
                new("me/capabilities", "GET", "Capacidades da assinatura."),
                new("me/limits", "GET", "Limites da assinatura."),
                new("me/check-capability", "POST", "Verifica capacidade. Body conforme API."),
                new("", "POST", "Cria assinatura. Body conforme API."),
                new("{id}", "GET", "Detalhe da assinatura por Id."),
                new("{id}/cancel", "POST", "Cancela assinatura."),
                new("{id}/reactivate", "POST", "Reativa assinatura.")
            },
            [Notifications] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista notificações do usuário. Requer Authorization."),
                new("paged", "GET", "Lista notificações paginada. Query: pageNumber, pageSize."),
                new("{id}/read", "POST", "Marca notificação como lida.")
            },
            [MarketplaceV1] = new List<JourneyEndpoint>
            {
                new("cart", "GET", "Carrinho atual. Requer Authorization."),
                new("cart", "POST", "Cria/atualiza carrinho. Body: items, etc."),
                new("cart/items", "POST", "Adiciona item ao carrinho."),
                new("cart/items/{id}", "DELETE", "Remove item do carrinho."),
                new("cart/checkout", "POST", "Checkout do carrinho."),
                new("stores", "GET", "Lista lojas. Query: territoryId."),
                new("stores/me", "GET", "Minhas lojas."),
                new("stores", "POST", "Cria loja. Body conforme API."),
                new("items", "GET", "Lista itens. Query: territoryId, storeId."),
                new("items/paged", "GET", "Lista itens paginada."),
                new("items/{id}", "GET", "Detalhe do item por Id.")
            },
            [Moderation] = new List<JourneyEndpoint>
            {
                new("{territoryId}/work-items", "GET", "Work items de moderação do território."),
                new("{territoryId}/moderation/cases", "GET", "Casos de moderação do território."),
                new("{territoryId}/evidences", "GET", "Evidências do território.")
            },
            [Chat] = new List<JourneyEndpoint>
            {
                new("conversations/{conversationId}", "GET", "Detalhe da conversa."),
                new("conversations/{conversationId}/messages", "GET", "Mensagens da conversa. Query: pageNumber, pageSize."),
                new("conversations/{conversationId}/messages", "POST", "Envia mensagem. Body: content."),
                new("conversations/{conversationId}/participants", "GET", "Participantes da conversa."),
                new("conversations/{conversationId}/participants", "POST", "Adiciona participante."),
                new("conversations/{conversationId}/read", "POST", "Marca como lida.")
            },
            [Alerts] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista alertas. Query: territoryId, status."),
                new("paged", "GET", "Lista alertas paginada. Query: pageNumber, pageSize."),
                new("", "POST", "Cria alerta. Body conforme API.")
            },
            [Admin] = new List<JourneyEndpoint>
            {
                new("seed", "POST", "Executa seed (admin). Requer role admin."),
                new("subscription-plans", "GET", "Planos de assinatura (admin)."),
                new("system-configs", "GET", "Configurações de sistema."),
                new("cache-metrics", "GET", "Métricas de cache."),
                new("work-items", "GET", "Work items globais (admin)."),
                new("verifications", "GET", "Verificações pendentes (admin).")
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
            },
            [Me] = new List<JourneyEndpoint>
            {
                new("", "GET", "Atividade/resumo do usuário."),
                new("profile", "GET", "Perfil do usuário."),
                new("preferences", "GET", "Preferências."),
                new("interests", "GET", "Interesses."),
                new("devices", "GET", "Dispositivos.")
            },
            [Connections] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista conexões."),
                new("pending", "GET", "Solicitações pendentes."),
                new("privacy", "GET", "Configurações de privacidade."),
                new("users/search", "GET", "Busca usuários."),
                new("suggestions", "GET", "Sugestões de conexão.")
            },
            [Territories] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista territórios."),
                new("paged", "GET", "Lista territórios paginada."),
                new("{id}", "GET", "Detalhe do território."),
                new("{id}/features", "GET", "Features do território.")
            },
            [Membership] = new List<JourneyEndpoint>
            {
                new("me", "GET", "Minhas memberships."),
                new("{territoryId}/me", "GET", "Membership no território.")
            },
            [Map] = new List<JourneyEndpoint>
            {
                new("entities", "GET", "Entidades do mapa."),
                new("entities/paged", "GET", "Entidades paginadas."),
                new("pins", "GET", "Pins do mapa."),
                new("pins/paged", "GET", "Pins paginados.")
            },
            [Assets] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista assets."),
                new("paged", "GET", "Lista assets paginada."),
                new("{assetId}", "GET", "Detalhe do asset.")
            },
            [Media] = new List<JourneyEndpoint>
            {
                new("{id}", "GET", "Download de mídia."),
                new("{id}/info", "GET", "Info da mídia.")
            },
            [SubscriptionPlans] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista planos."),
                new("{id}", "GET", "Detalhe do plano.")
            },
            [Subscriptions] = new List<JourneyEndpoint>
            {
                new("me", "GET", "Minha assinatura."),
                new("me/capabilities", "GET", "Capacidades."),
                new("me/limits", "GET", "Limites."),
                new("{id}", "GET", "Detalhe da assinatura.")
            },
            [Notifications] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista notificações."),
                new("paged", "GET", "Notificações paginada.")
            },
            [MarketplaceV1] = new List<JourneyEndpoint>
            {
                new("cart", "GET", "Carrinho."),
                new("stores", "GET", "Lojas."),
                new("stores/me", "GET", "Minhas lojas."),
                new("items", "GET", "Lista itens."),
                new("items/paged", "GET", "Itens paginados."),
                new("items/{id}", "GET", "Detalhe do item.")
            },
            [Moderation] = new List<JourneyEndpoint>
            {
                new("{territoryId}/work-items", "GET", "Work items do território."),
                new("{territoryId}/moderation/cases", "GET", "Casos de moderação."),
                new("{territoryId}/evidences", "GET", "Evidências.")
            },
            [Chat] = new List<JourneyEndpoint>
            {
                new("conversations/{conversationId}", "GET", "Conversa."),
                new("conversations/{conversationId}/messages", "GET", "Mensagens.")
            },
            [Alerts] = new List<JourneyEndpoint>
            {
                new("", "GET", "Lista alertas."),
                new("paged", "GET", "Alertas paginados.")
            },
            [Admin] = new List<JourneyEndpoint>
            {
                new("subscription-plans", "GET", "Planos (admin)."),
                new("system-configs", "GET", "System configs."),
                new("cache-metrics", "GET", "Cache metrics.")
            }
        };

    /// <summary>Todos os path prefixes de jornada (para validação e TTL).</summary>
    public static IReadOnlyList<string> AllPathPrefixes { get; } = new[]
    {
        Onboarding,
        Feed,
        Events,
        Marketplace,
        Auth,
        Me,
        Connections,
        Territories,
        Membership,
        Map,
        Assets,
        Media,
        SubscriptionPlans,
        Subscriptions,
        Notifications,
        MarketplaceV1,
        Moderation,
        Chat,
        Alerts,
        Admin
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
