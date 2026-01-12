using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<(User user, string token)> LoginSocialAsync(
        string provider,
        string externalId,
        string displayName,
        string? email,
        string? cpf,
        string? foreignDocument,
        string? phoneNumber,
        string? address,
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
            cpf,
            foreignDocument,
            phoneNumber,
            address,
            provider,
            externalId,
            UserRole.Visitor,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return (user, _tokenService.IssueToken(user.Id));
    }
}
