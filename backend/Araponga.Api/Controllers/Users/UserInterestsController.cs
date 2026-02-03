using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/interests")]
[Produces("application/json")]
[Tags("User Interests")]
public sealed class UserInterestsController : ControllerBase
{
    private readonly UserInterestService _interestService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserInterestsController(
        UserInterestService interestService,
        CurrentUserAccessor currentUserAccessor)
    {
        _interestService = interestService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista todos os interesses do usuário autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de tags de interesses do usuário.</returns>
    /// <remarks>
    /// Os interesses são usados para filtrar o feed e personalizar recomendações de conteúdo.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<string>>> ListInterests(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var interests = await _interestService.ListInterestsAsync(
            userContext.User.Id,
            cancellationToken);

        var tags = interests.Select(i => i.InterestTag).ToList();
        return Ok(tags);
    }

    /// <summary>
    /// Adiciona um interesse para o usuário autenticado.
    /// </summary>
    /// <param name="request">Tag do interesse a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Tag do interesse adicionado.</returns>
    /// <remarks>
    /// O interesse será usado para filtrar o feed quando `filterByInterests=true` for usado.
    /// Tags são case-insensitive e serão normalizadas.
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<string>> AddInterest(
        [FromBody] AddInterestRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _interestService.AddInterestAsync(
            userContext.User.Id,
            request.InterestTag,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value!.InterestTag);
    }

    /// <summary>
    /// Remove um interesse do usuário autenticado.
    /// </summary>
    /// <param name="tag">Tag do interesse a ser removido.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Interesse removido com sucesso.</returns>
    [HttpDelete("{tag}")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> RemoveInterest(
        string tag,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _interestService.RemoveInterestAsync(
            userContext.User.Id,
            tag,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}
