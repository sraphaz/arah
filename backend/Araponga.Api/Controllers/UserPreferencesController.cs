using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/preferences")]
[Produces("application/json")]
[Tags("User Preferences")]
public sealed class UserPreferencesController : ControllerBase
{
    private readonly UserPreferencesService _preferencesService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserPreferencesController(
        UserPreferencesService preferencesService,
        CurrentUserAccessor currentUserAccessor)
    {
        _preferencesService = preferencesService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém as preferências do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserPreferencesResponse>> GetMyPreferences(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var preferences = await _preferencesService.GetPreferencesAsync(
            userContext.User.Id,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Atualiza as preferências de privacidade do usuário autenticado.
    /// </summary>
    [HttpPut("privacy")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserPreferencesResponse>> UpdatePrivacyPreferences(
        [FromBody] UpdatePrivacyPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<ProfileVisibility>(request.ProfileVisibility, ignoreCase: true, out var profileVisibility))
        {
            return BadRequest(new { error = "Invalid profileVisibility. Must be one of: Public, ResidentsOnly, Private." });
        }

        if (!Enum.TryParse<ContactVisibility>(request.ContactVisibility, ignoreCase: true, out var contactVisibility))
        {
            return BadRequest(new { error = "Invalid contactVisibility. Must be one of: Public, ResidentsOnly, Private." });
        }

        var preferences = await _preferencesService.UpdatePrivacyPreferencesAsync(
            userContext.User.Id,
            profileVisibility,
            contactVisibility,
            request.ShareLocation,
            request.ShowMemberships,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Atualiza as preferências de notificações do usuário autenticado.
    /// </summary>
    [HttpPut("notifications")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserPreferencesResponse>> UpdateNotificationPreferences(
        [FromBody] UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var notificationPreferences = new NotificationPreferences(
            request.PostsEnabled,
            request.CommentsEnabled,
            request.EventsEnabled,
            request.AlertsEnabled,
            request.MarketplaceEnabled,
            request.ModerationEnabled,
            request.MembershipRequestsEnabled);

        var preferences = await _preferencesService.UpdateNotificationPreferencesAsync(
            userContext.User.Id,
            notificationPreferences,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Atualiza as preferências de email do usuário autenticado.
    /// </summary>
    [HttpPut("email")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<UserPreferencesResponse>> UpdateEmailPreferences(
        [FromBody] UpdateEmailPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var currentPreferences = await _preferencesService.GetPreferencesAsync(
            userContext.User.Id,
            cancellationToken);

        var receiveEmails = request.ReceiveEmails ?? currentPreferences.EmailPreferences.ReceiveEmails;
        var emailFrequency = currentPreferences.EmailPreferences.EmailFrequency;
        var emailTypes = currentPreferences.EmailPreferences.EmailTypes;

        if (request.EmailFrequency != null)
        {
            if (!Enum.TryParse<EmailFrequency>(request.EmailFrequency, ignoreCase: true, out var parsedFrequency))
            {
                return BadRequest(new { error = "Invalid emailFrequency. Must be one of: Immediate, Daily, Weekly." });
            }
            emailFrequency = parsedFrequency;
        }

        if (request.EmailTypes.HasValue)
        {
            emailTypes = (EmailTypes)request.EmailTypes.Value;
        }

        var emailPreferences = new EmailPreferences(
            receiveEmails,
            emailFrequency,
            emailTypes);

        var preferences = await _preferencesService.UpdateEmailPreferencesAsync(
            userContext.User.Id,
            emailPreferences,
            cancellationToken);

        return Ok(MapToResponse(preferences));
    }

    private static UserPreferencesResponse MapToResponse(UserPreferences preferences)
    {
        return new UserPreferencesResponse(
            preferences.UserId,
            preferences.ProfileVisibility.ToString(),
            preferences.ContactVisibility.ToString(),
            preferences.ShareLocation,
            preferences.ShowMemberships,
            new NotificationPreferencesResponse(
                preferences.NotificationPreferences.PostsEnabled,
                preferences.NotificationPreferences.CommentsEnabled,
                preferences.NotificationPreferences.EventsEnabled,
                preferences.NotificationPreferences.AlertsEnabled,
                preferences.NotificationPreferences.MarketplaceEnabled,
                preferences.NotificationPreferences.ModerationEnabled,
                preferences.NotificationPreferences.MembershipRequestsEnabled),
            new EmailPreferencesResponse(
                preferences.EmailPreferences.ReceiveEmails,
                preferences.EmailPreferences.EmailFrequency.ToString(),
                (int)preferences.EmailPreferences.EmailTypes),
            preferences.CreatedAtUtc,
            preferences.UpdatedAtUtc);
    }
}
