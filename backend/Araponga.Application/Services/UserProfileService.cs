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
    private readonly MediaService? _mediaService;

    public UserProfileService(
        IUserRepository userRepository,
        IUserInterestRepository interestRepository,
        IUnitOfWork unitOfWork,
        IVoteRepository? voteRepository = null,
        IVotingRepository? votingRepository = null,
        MediaService? mediaService = null)
    {
        _userRepository = userRepository;
        _interestRepository = interestRepository;
        _voteRepository = voteRepository;
        _votingRepository = votingRepository;
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
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
            user.AvatarMediaAssetId,
            user.Bio,
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
            user.AvatarMediaAssetId,
            user.Bio,
            user.CreatedAtUtc);

        await _userRepository.UpdateAsync(updatedUser, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return updatedUser;
    }

    public async Task<User> UpdateAvatarAsync(
        Guid userId,
        Guid mediaAssetId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        // Validar que a mídia existe e pertence ao usuário
        if (_mediaService is not null)
        {
            var mediaAsset = await _mediaService.GetMediaAssetAsync(mediaAssetId, cancellationToken);
            if (mediaAsset is null || mediaAsset.IsDeleted)
            {
                throw new NotFoundException("MediaAsset", mediaAssetId);
            }

            if (mediaAsset.UploadedByUserId != userId)
            {
                throw new ForbiddenException("MediaAsset não pertence ao usuário.");
            }

            // Validar que é imagem (não vídeo ou áudio)
            if (mediaAsset.MediaType != Domain.Media.MediaType.Image)
            {
                throw new ValidationException("Avatar deve ser uma imagem.");
            }
        }

        user.UpdateAvatar(mediaAssetId);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return user;
    }

    public async Task<User> UpdateBioAsync(
        Guid userId,
        string? bio,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        user.UpdateBio(bio);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return user;
    }
}
