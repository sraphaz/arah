using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

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

    public async Task<UserContext> GetAsync(HttpRequest request, CancellationToken cancellationToken)
    {
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
        var userId = _tokenService.ParseToken(token);
        if (userId is null)
        {
            return new UserContext(null, TokenStatus.Invalid);
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        return user is null
            ? new UserContext(null, TokenStatus.Invalid)
            : new UserContext(user, TokenStatus.Valid);
    }
}
