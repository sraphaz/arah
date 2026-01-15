namespace Araponga.Application.Common;

/// <summary>
/// Constantes da aplicação para evitar strings mágicas e números mágicos.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Constantes de paginação padrão.
    /// </summary>
    public static class Pagination
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int MinPageSize = 1;
    }

    /// <summary>
    /// Constantes de cache TTL (Time To Live).
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// TTL para cache de membership (10 minutos).
        /// </summary>
        public static readonly TimeSpan MembershipExpiration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// TTL para cache de system permissions (15 minutos).
        /// </summary>
        public static readonly TimeSpan SystemPermissionExpiration = TimeSpan.FromMinutes(15);

        /// <summary>
        /// TTL para cache de territórios (30 minutos).
        /// </summary>
        public static readonly TimeSpan TerritoryExpiration = TimeSpan.FromMinutes(30);

        /// <summary>
        /// TTL para cache de território individual (10 minutos).
        /// </summary>
        public static readonly TimeSpan TerritoryDetailExpiration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// TTL para cache de alertas (10 minutos).
        /// </summary>
        public static readonly TimeSpan AlertExpiration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// TTL para cache de eventos (15 minutos).
        /// </summary>
        public static readonly TimeSpan EventExpiration = TimeSpan.FromMinutes(15);

        /// <summary>
        /// TTL para cache de feature flags (5 minutos).
        /// </summary>
        public static readonly TimeSpan FeatureFlagExpiration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// TTL para cache de map entities (10 minutos).
        /// </summary>
        public static readonly TimeSpan MapEntityExpiration = TimeSpan.FromMinutes(10);
    }

    /// <summary>
    /// Constantes de geolocalização.
    /// </summary>
    public static class Geo
    {
        /// <summary>
        /// Raio máximo para verificação de geolocalização (5km).
        /// </summary>
        public const double VerificationRadiusKm = 5.0;

        /// <summary>
        /// Latitude mínima válida.
        /// </summary>
        public const double MinLatitude = -90.0;

        /// <summary>
        /// Latitude máxima válida.
        /// </summary>
        public const double MaxLatitude = 90.0;

        /// <summary>
        /// Longitude mínima válida.
        /// </summary>
        public const double MinLongitude = -180.0;

        /// <summary>
        /// Longitude máxima válida.
        /// </summary>
        public const double MaxLongitude = 180.0;
    }

    /// <summary>
    /// Constantes de validação de strings.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Tamanho máximo para nome de exibição.
        /// </summary>
        public const int MaxDisplayNameLength = 200;

        /// <summary>
        /// Tamanho máximo para descrição.
        /// </summary>
        public const int MaxDescriptionLength = 2000;

        /// <summary>
        /// Tamanho máximo para categoria.
        /// </summary>
        public const int MaxCategoryLength = 100;

        /// <summary>
        /// Tamanho máximo para telefone.
        /// </summary>
        public const int MaxPhoneLength = 20;

        /// <summary>
        /// Tamanho mínimo para JWT secret em produção.
        /// </summary>
        public const int MinJwtSecretLength = 32;
    }

    /// <summary>
    /// Constantes de rate limiting padrão.
    /// </summary>
    public static class RateLimiting
    {
        /// <summary>
        /// Limite padrão global (60 req/min).
        /// </summary>
        public const int DefaultPermitLimit = 60;

        /// <summary>
        /// Janela padrão em segundos (60 segundos = 1 minuto).
        /// </summary>
        public const int DefaultWindowSeconds = 60;

        /// <summary>
        /// Limite para endpoints de autenticação (5 req/min).
        /// </summary>
        public const int AuthPermitLimit = 5;

        /// <summary>
        /// Limite para endpoints de feed (100 req/min).
        /// </summary>
        public const int FeedPermitLimit = 100;

        /// <summary>
        /// Limite para endpoints de escrita (30 req/min).
        /// </summary>
        public const int WritePermitLimit = 30;
    }
}
