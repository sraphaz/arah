using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
[Tags("User Profile")]
public sealed class UserPublicProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;
    private readonly UserProfileStatsService? _statsService;
    private readonly IUserInterestRepository _interestRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly MediaService? _mediaService;

    public UserPublicProfileController(
        UserProfileService profileService,
        IUserInterestRepository interestRepository,
        CurrentUserAccessor currentUserAccessor,
        UserProfileStatsService? statsService = null,
        MediaService? mediaService = null)
    {
        _profileService = profileService;
        _statsService = statsService;
        _interestRepository = interestRepository;
        _currentUserAccessor = currentUserAccessor;
        _mediaService = mediaService;
    }

    /// <summary>
    /// Obtém o perfil público de um usuário.
    /// Respeita as preferências de privacidade do usuário.
    /// </summary>
    /// <param name="id">ID do usuário cujo perfil será visualizado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Perfil público do usuário, respeitando privacidade.</returns>
    [HttpGet("{id}/profile")]
    [ProducesResponseType(typeof(UserProfilePublicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfilePublicResponse>> GetUserProfile(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Obter viewer (pode ser null se não autenticado)
        Guid? viewerUserId = null;
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Valid && userContext.User is not null)
        {
            viewerUserId = userContext.User.Id;
        }

        // Obter perfil respeitando privacidade
        var user = await _profileService.GetProfileAsync(id, viewerUserId, cancellationToken);

        // Obter interesses do usuário
        var interests = await _interestRepository.ListByUserIdAsync(id, cancellationToken);
        var interestTags = interests.Select(i => i.InterestTag).ToList();

        // Mapear para resposta pública (sem informações de contato)
        var avatarUrl = await GetAvatarUrlAsync(user, cancellationToken);

        var response = new UserProfilePublicResponse(
            user.Id,
            user.DisplayName,
            user.CreatedAtUtc,
            interestTags,
            avatarUrl,
            user.Bio);

        return Ok(response);
    }

    /// <summary>
    /// Obtém as estatísticas de contribuição territorial de um usuário.
    /// Respeita as preferências de privacidade do usuário.
    /// </summary>
    /// <param name="id">ID do usuário cujas estatísticas serão visualizadas.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Estatísticas de contribuição do usuário.</returns>
    [HttpGet("{id}/profile/stats")]
    [ProducesResponseType(typeof(UserProfileStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileStatsResponse>> GetUserProfileStats(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Verificar se o usuário existe e se o viewer tem permissão
        // (usar a mesma lógica de privacidade do perfil)
        Guid? viewerUserId = null;
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Valid && userContext.User is not null)
        {
            viewerUserId = userContext.User.Id;
        }

        // Verificar acesso ao perfil (mesma lógica de privacidade)
        await _profileService.GetProfileAsync(id, viewerUserId, cancellationToken);

        // Se chegou aqui, tem permissão. Calcular estatísticas
        if (_statsService is null)
        {
            return Ok(new UserProfileStatsResponse(id, 0, 0, 0, 0, 0));
        }

        var stats = await _statsService.GetStatsAsync(id, cancellationToken);

        var response = new UserProfileStatsResponse(
            stats.UserId,
            stats.PostsCreated,
            stats.EventsCreated,
            stats.EventsParticipated,
            stats.TerritoriesMember,
            stats.EntitiesConfirmed);

        return Ok(response);
    }

    private async Task<string?> GetAvatarUrlAsync(
        Domain.Users.User user,
        CancellationToken cancellationToken)
    {
        if (!user.AvatarMediaAssetId.HasValue || _mediaService is null)
        {
            return null;
        }

        var urlResult = await _mediaService.GetMediaUrlAsync(user.AvatarMediaAssetId.Value, null, cancellationToken);
        return urlResult.IsSuccess ? urlResult.Value : null;
    }
}
