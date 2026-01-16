using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Araponga.Api.Security;

/// <summary>
/// Handler de autenticação JWT customizado.
/// Este handler é usado apenas para fornecer um esquema padrão para ForbidResult.
/// A validação real do JWT é feita via middleware customizado.
/// </summary>
public sealed class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // A autenticação real é feita via middleware customizado.
        // Este handler existe apenas para fornecer um esquema padrão para ForbidResult.
        // Retornamos NoResult para que o middleware customizado possa processar.
        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
