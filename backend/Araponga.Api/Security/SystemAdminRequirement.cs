using Microsoft.AspNetCore.Authorization;

namespace Araponga.Api.Security;

/// <summary>
/// Requirement para a política "SystemAdmin": o usuário autenticado deve ter permissão de sistema SystemAdmin.
/// </summary>
public sealed class SystemAdminRequirement : IAuthorizationRequirement
{
}
