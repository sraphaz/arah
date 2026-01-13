using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/profile")]
[Produces("application/json")]
[Tags("User Profile")]
public sealed class UserProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserProfileController(
        UserProfileService profileService,
        CurrentUserAccessor currentUserAccessor)
    {
        _profileService = profileService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém o perfil do usuário autenticado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var user = await _profileService.GetProfileAsync(
            userContext.User.Id,
            cancellationToken);

        return Ok(MapToResponse(user));
    }

    /// <summary>
    /// Atualiza o nome de exibição do usuário autenticado.
    /// </summary>
    [HttpPut("display-name")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateDisplayName(
        [FromBody] UpdateDisplayNameRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest(new { error = "DisplayName is required." });
        }

        var user = await _profileService.UpdateDisplayNameAsync(
            userContext.User.Id,
            request.DisplayName,
            cancellationToken);

        return Ok(MapToResponse(user));
    }

    /// <summary>
    /// Atualiza as informações de contato do usuário autenticado.
    /// </summary>
    [HttpPut("contact")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileResponse>> UpdateContactInfo(
        [FromBody] UpdateContactInfoRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var user = await _profileService.UpdateContactInfoAsync(
            userContext.User.Id,
            request.Email,
            request.PhoneNumber,
            request.Address,
            cancellationToken);

        return Ok(MapToResponse(user));
    }

    private static UserProfileResponse MapToResponse(Domain.Users.User user)
    {
        return new UserProfileResponse(
            user.Id,
            user.DisplayName,
            user.Email,
            user.PhoneNumber,
            user.Address,
            user.CreatedAtUtc);
    }
}
