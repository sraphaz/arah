using Araponga.Application.Exceptions;
using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class UserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserInterestRepository _interestRepository;
    private readonly IVoteRepository? _voteRepository;
    private readonly IVotingRepository? _votingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserProfileService(
        IUserRepository userRepository,
        IUserInterestRepository interestRepository,
        IUnitOfWork unitOfWork,
        IVoteRepository? voteRepository = null,
        IVotingRepository? votingRepository = null)
    {
        _userRepository = userRepository;
        _interestRepository = interestRepository;
        _voteRepository = voteRepository;
        _votingRepository = votingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        return user;
    }

    public async Task<User> UpdateDisplayNameAsync(
        Guid userId,
        string displayName,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        // Criar novo User com displayName atualizado
        var updatedUser = new User(
            user.Id,
            displayName,
            user.Email,
            user.Cpf,
            user.ForeignDocument,
            user.PhoneNumber,
            user.Address,
            user.AuthProvider,
            user.ExternalId,
            user.TwoFactorEnabled,
            user.TwoFactorSecret,
            user.TwoFactorRecoveryCodesHash,
            user.TwoFactorVerifiedAtUtc,
            user.IdentityVerificationStatus,
            user.IdentityVerifiedAtUtc,
            user.CreatedAtUtc);

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return updatedUser;
    }

    public async Task<User> UpdateContactInfoAsync(
        Guid userId,
        string? email,
        string? phoneNumber,
        string? address,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        var updatedUser = new User(
            user.Id,
            user.DisplayName,
            email,
            user.Cpf,
            user.ForeignDocument,
            phoneNumber,
            address,
            user.AuthProvider,
            user.ExternalId,
            user.TwoFactorEnabled,
            user.TwoFactorSecret,
            user.TwoFactorRecoveryCodesHash,
            user.TwoFactorVerifiedAtUtc,
            user.IdentityVerificationStatus,
            user.IdentityVerifiedAtUtc,
            user.CreatedAtUtc);

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return updatedUser;
    }
}
