namespace Araponga.Domain.Users;

/// <summary>
/// Tipos de permissões globais do sistema (não territoriais).
/// </summary>
public enum SystemPermissionType
{
    SystemAdmin = 1,              // Acesso total ao sistema (inclui bypass de geo por padrão)
    SupportAgent = 2,             // Suporte e observabilidade
    TerritoryAdmin = 3,           // Pode criar/gerenciar territórios
    RemoteAccessToTerritory = 4   // Pode acessar territórios sem exigência de convergência geolocalização
}
