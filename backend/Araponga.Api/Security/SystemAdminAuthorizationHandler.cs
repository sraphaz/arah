using System.Security.Claims;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Araponga.Api.Security;

/// <summary>
/// Avalia a política SystemAdmin: verifica se o usuário autenticado possui SystemPermissionType.SystemAdmin.
/// </summary>
public sealed class SystemAdminAuthorizationHandler : AuthorizationHandler<SystemAdminRequirement>
{
    private readonly AccessEvaluator _accessEvaluator;

    public SystemAdminAuthorizationHandler(AccessEvaluator accessEvaluator)
    {
        _accessEvaluator = accessEvaluator;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemAdminRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        if (await _accessEvaluator.IsSystemAdminAsync(userId, CancellationToken.None))
        {
            context.Succeed(requirement);
        }
    }
}
