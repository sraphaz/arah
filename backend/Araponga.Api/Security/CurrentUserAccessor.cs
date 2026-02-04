using System.Security.Claims;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace Araponga.Api.Security;

public sealed class CurrentUserAccessor
{
    private const string BearerPrefix = "Bearer ";
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public CurrentUserAccessor(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Obtém o contexto do usuário atual. Preferência: HttpContext.User (definido pelo JwtAuthenticationHandler);
    /// fallback: parse do Bearer token no request (ex.: testes ou contextos sem pipeline de auth).
    /// </summary>
    public async Task<UserContext> GetAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        var httpContext = request.HttpContext;
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContext.User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
                return user is null
                    ? new UserContext(null, TokenStatus.Invalid)
                    : new UserContext(user, TokenStatus.Valid);
            }
        }

        if (!request.Headers.TryGetValue(ApiHeaders.Authorization, out var authorization) ||
            string.IsNullOrWhiteSpace(authorization))
        {
            return new UserContext(null, TokenStatus.Missing);
        }

        var authValue = authorization.ToString();
        if (!authValue.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return new UserContext(null, TokenStatus.Invalid);
        }

        var token = authValue[BearerPrefix.Length..].Trim();
        var userIdFromToken = _tokenService.ParseToken(token);
        if (userIdFromToken is null)
        {
            return new UserContext(null, TokenStatus.Invalid);
        }

        var userFromRepo = await _userRepository.GetByIdAsync(userIdFromToken.Value, cancellationToken);
        return userFromRepo is null
            ? new UserContext(null, TokenStatus.Invalid)
            : new UserContext(userFromRepo, TokenStatus.Valid);
    }
}
