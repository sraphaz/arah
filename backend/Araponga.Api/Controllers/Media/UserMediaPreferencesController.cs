using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para gerenciar preferências de mídia do usuário.
/// </summary>
[ApiController]
[Route("api/v1/user/media-preferences")]
[Produces("application/json")]
[Tags("User Preferences")]
public sealed class UserMediaPreferencesController : ControllerBase
{
    private readonly UserMediaPreferencesService _preferencesService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserMediaPreferencesController(
        UserMediaPreferencesService preferencesService,
        CurrentUserAccessor currentUserAccessor)
    {
        _preferencesService = preferencesService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém preferências de mídia do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserMediaPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserMediaPreferencesResponse>> Get(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var preferences = await _preferencesService.GetPreferencesAsync(userContext.User.Id, cancellationToken);
        var response = MapToResponse(preferences);
        return Ok(response);
    }

    /// <summary>
    /// Atualiza preferências de mídia do usuário autenticado.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(UserMediaPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserMediaPreferencesResponse>> Update(
        [FromBody] UpdateUserMediaPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var existingPreferences = await _preferencesService.GetPreferencesAsync(userContext.User.Id, cancellationToken);
        var updatedPreferences = UpdatePreferencesFromRequest(existingPreferences, request);

        var savedPreferences = await _preferencesService.UpdatePreferencesAsync(
            userContext.User.Id,
            updatedPreferences,
            cancellationToken);

        var response = MapToResponse(savedPreferences);
        return Ok(response);
    }

    private static UserMediaPreferencesResponse MapToResponse(Domain.Users.UserMediaPreferences preferences)
    {
        return new UserMediaPreferencesResponse(
            preferences.UserId,
            preferences.ShowImages,
            preferences.ShowVideos,
            preferences.ShowAudio,
            preferences.AutoPlayVideos,
            preferences.AutoPlayAudio,
            preferences.UpdatedAtUtc);
    }

    private static Domain.Users.UserMediaPreferences UpdatePreferencesFromRequest(
        Domain.Users.UserMediaPreferences existing,
        UpdateUserMediaPreferencesRequest request)
    {
        return new Domain.Users.UserMediaPreferences
        {
            UserId = existing.UserId,
            ShowImages = request.ShowImages ?? existing.ShowImages,
            ShowVideos = request.ShowVideos ?? existing.ShowVideos,
            ShowAudio = request.ShowAudio ?? existing.ShowAudio,
            AutoPlayVideos = request.AutoPlayVideos ?? existing.AutoPlayVideos,
            AutoPlayAudio = request.AutoPlayAudio ?? existing.AutoPlayAudio,
            UpdatedAtUtc = existing.UpdatedAtUtc
        };
    }
}
