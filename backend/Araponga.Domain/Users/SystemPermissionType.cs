namespace Araponga.Domain.Users;

/// <summary>
/// Tipos de permissões globais do sistema (não territoriais).
/// </summary>
public enum SystemPermissionType
{
    SystemAdmin = 1,      // Acesso total ao sistema
    SupportAgent = 2,     // Suporte e observabilidade
    TerritoryAdmin = 3    // Pode criar/gerenciar territórios
}
