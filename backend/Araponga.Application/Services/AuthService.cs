using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<(User user, string token)> LoginSocialAsync(
        string provider,
        string externalId,
        string displayName,
        string email,
        CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByProviderAsync(provider, externalId, cancellationToken);
        if (existing is not null)
        {
            return (existing, _tokenService.IssueToken(existing.Id));
        }

        var user = new User(
            Guid.NewGuid(),
            displayName,
            email,
            provider,
            externalId,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);

        return (user, _tokenService.IssueToken(user.Id));
    }
}
