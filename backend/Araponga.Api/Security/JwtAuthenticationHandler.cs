using Araponga.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Araponga.Api.Security;

/// <summary>
/// Valida o Bearer JWT (user ou system/client_credentials) e define HttpContext.User.
/// Permite que [Authorize] e políticas (ex.: SystemAdmin) funcionem no pipeline.
/// </summary>
public sealed class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string BearerPrefix = "Bearer ";
    private readonly ITokenService _tokenService;

    public JwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITokenService tokenService)
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiHeaders.Authorization, out var authorization) ||
            string.IsNullOrWhiteSpace(authorization))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authValue = authorization.ToString();
        if (!authValue.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authValue[BearerPrefix.Length..].Trim();
        var principal = _tokenService.GetPrincipal(token);
        if (principal is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired token."));
        }

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
