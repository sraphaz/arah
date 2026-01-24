using Araponga.Application.Exceptions;
using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class UserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserInterestRepository _interestRepository;
    private readonly IUserPreferencesRepository? _preferencesRepository;
    private readonly ITerritoryMembershipRepository? _membershipRepository;
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
        MediaService? mediaService = null,
        IUserPreferencesRepository? preferencesRepository = null,
        ITerritoryMembershipRepository? membershipRepository = null)
    {
        _userRepository = userRepository;
        _interestRepository = interestRepository;
        _preferencesRepository = preferencesRepository;
        _membershipRepository = membershipRepository;
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

    /// <summary>
    /// Obtém o perfil de um usuário para visualização por outro usuário.
    /// Respeita as preferências de privacidade do usuário.
    /// </summary>
    /// <param name="userId">ID do usuário cujo perfil será visualizado.</param>
    /// <param name="viewerUserId">ID do usuário que está visualizando (null se não autenticado).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O usuário se o viewer tem permissão, caso contrário lança ForbiddenException.</returns>
    /// <exception cref="NotFoundException">Se o usuário não for encontrado.</exception>
    /// <exception cref="ForbiddenException">Se o viewer não tem permissão para ver o perfil.</exception>
    public async Task<User> GetProfileAsync(
        Guid userId,
        Guid? viewerUserId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        // Se o viewer é o próprio usuário, sempre permitir
        if (viewerUserId.HasValue && viewerUserId.Value == userId)
        {
            return user;
        }

        // Obter preferências de privacidade do usuário
        var preferences = _preferencesRepository is not null
            ? await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken)
            : null;

        // Se não há preferências, usar padrão (Public)
        var profileVisibility = preferences?.ProfileVisibility ?? ProfileVisibility.Public;

        // Verificar permissão baseada na visibilidade
        switch (profileVisibility)
        {
            case ProfileVisibility.Public:
                // Público: qualquer um pode ver
                return user;

            case ProfileVisibility.ResidentsOnly:
                // Apenas residents: verificar se viewer é resident do mesmo território
                if (!viewerUserId.HasValue)
                {
                    throw new ForbiddenException("Perfil visível apenas para moradores.");
                }

                var canView = await CanViewResidentsOnlyProfileAsync(userId, viewerUserId.Value, cancellationToken);
                if (!canView)
                {
                    throw new ForbiddenException("Perfil visível apenas para moradores dos mesmos territórios.");
                }

                return user;

            case ProfileVisibility.Private:
                // Privado: apenas o próprio usuário
                throw new ForbiddenException("Perfil privado.");

            default:
                throw new ForbiddenException("Perfil não acessível.");
        }
    }

    /// <summary>
    /// Verifica se dois usuários compartilham territórios (são residents do mesmo território).
    /// </summary>
    private async Task<bool> CanViewResidentsOnlyProfileAsync(
        Guid profileUserId,
        Guid viewerUserId,
        CancellationToken cancellationToken)
    {
        if (_membershipRepository is null)
        {
            return false;
        }

        // Obter todos os memberships do perfil do usuário (como Resident)
        var profileMemberships = await _membershipRepository.ListByUserAsync(profileUserId, cancellationToken);
        var profileTerritoryIds = profileMemberships
            .Where(m => m.Role == MembershipRole.Resident && m.ResidencyVerification != ResidencyVerification.None)
            .Select(m => m.TerritoryId)
            .ToList();

        if (profileTerritoryIds.Count == 0)
        {
            return false;
        }

        // Verificar se o viewer é resident de algum dos mesmos territórios
        var viewerMemberships = await _membershipRepository.ListByUserAsync(viewerUserId, cancellationToken);
        var viewerTerritoryIds = viewerMemberships
            .Where(m => m.Role == MembershipRole.Resident && m.ResidencyVerification != ResidencyVerification.None)
            .Select(m => m.TerritoryId)
            .ToHashSet();

        return profileTerritoryIds.Any(territoryId => viewerTerritoryIds.Contains(territoryId));
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
