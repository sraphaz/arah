using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Ratings")]
public sealed class RatingController : ControllerBase
{
    private readonly RatingService _ratingService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly IUserRepository _userRepository;

    public RatingController(
        RatingService ratingService,
        CurrentUserAccessor currentUserAccessor,
        IUserRepository userRepository)
    {
        _ratingService = ratingService;
        _currentUserAccessor = currentUserAccessor;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Avalia uma loja.
    /// </summary>
    /// <remarks>
    /// Permite que usuários avaliem lojas com rating de 1 a 5 e comentário opcional.
    /// </remarks>
    [HttpPost("stores/{storeId:guid}/ratings")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RatingResponse>> RateStore(
        [FromRoute] Guid storeId,
        [FromBody] CreateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _ratingService.RateStoreAsync(
            storeId,
            userContext.User.Id,
            request.Rating,
            request.Comment,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to rate store." });
        }

        var rating = result.Value;
        var response = await BuildRatingResponseAsync(rating, cancellationToken);
        return CreatedAtAction(nameof(GetStoreRatings), new { storeId }, response);
    }

    /// <summary>
    /// Lista avaliações de uma loja.
    /// </summary>
    [HttpGet("stores/{storeId:guid}/ratings")]
    [ProducesResponseType(typeof(IEnumerable<RatingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RatingResponse>>> GetStoreRatings(
        [FromRoute] Guid storeId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.ListStoreRatingsAsync(storeId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        var responses = new List<RatingResponse>();
        foreach (var rating in result.Value ?? Array.Empty<StoreRating>())
        {
            var response = await BuildRatingResponseAsync(rating, cancellationToken);
            responses.Add(response);
        }

        return Ok(responses);
    }

    /// <summary>
    /// Avalia um item do marketplace.
    /// </summary>
    /// <remarks>
    /// Permite que usuários avaliem itens com rating de 1 a 5 e comentário opcional.
    /// </remarks>
    [HttpPost("items/{itemId:guid}/ratings")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RatingResponse>> RateItem(
        [FromRoute] Guid itemId,
        [FromBody] CreateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _ratingService.RateItemAsync(
            itemId,
            userContext.User.Id,
            request.Rating,
            request.Comment,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to rate item." });
        }

        var rating = result.Value;
        var response = await BuildItemRatingResponseAsync(rating, cancellationToken);
        return CreatedAtAction(nameof(GetItemRatings), new { itemId }, response);
    }

    /// <summary>
    /// Lista avaliações de um item.
    /// </summary>
    [HttpGet("items/{itemId:guid}/ratings")]
    [ProducesResponseType(typeof(IEnumerable<RatingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<RatingResponse>>> GetItemRatings(
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.ListItemRatingsAsync(itemId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        var responses = new List<RatingResponse>();
        foreach (var rating in result.Value ?? Array.Empty<StoreItemRating>())
        {
            var response = await BuildItemRatingResponseAsync(rating, cancellationToken);
            responses.Add(response);
        }

        return Ok(responses);
    }

    /// <summary>
    /// Responde a uma avaliação de loja.
    /// </summary>
    /// <remarks>
    /// Permite que o dono da loja responda a uma avaliação.
    /// </remarks>
    [HttpPost("ratings/{ratingId:guid}/response")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(RatingResponseInfo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RatingResponseInfo>> RespondToRating(
        [FromRoute] Guid ratingId,
        [FromQuery] Guid storeId,
        [FromBody] RespondToRatingRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _ratingService.RespondToRatingAsync(
            ratingId,
            storeId,
            userContext.User.Id,
            request.ResponseText,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to respond to rating." });
        }

        var response = result.Value;
        return CreatedAtAction(nameof(GetStoreRatings), new { storeId }, new RatingResponseInfo(
            response.Id,
            response.ResponseText,
            response.CreatedAtUtc));
    }

    /// <summary>
    /// Obtém a média de avaliações de uma loja.
    /// </summary>
    [HttpGet("stores/{storeId:guid}/ratings/average")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<double>> GetStoreAverageRating(
        [FromRoute] Guid storeId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetStoreAverageRatingAsync(storeId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém a média de avaliações de um item.
    /// </summary>
    [HttpGet("items/{itemId:guid}/ratings/average")]
    [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<double>> GetItemAverageRating(
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var result = await _ratingService.GetItemAverageRatingAsync(itemId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    private async Task<RatingResponse> BuildRatingResponseAsync(
        StoreRating rating,
        CancellationToken cancellationToken)
    {
        // Buscar resposta se existir
        var response = await _ratingService.GetRatingResponseAsync(rating.Id, cancellationToken);

        RatingResponseInfo? responseInfo = null;
        if (response is not null)
        {
            responseInfo = new RatingResponseInfo(
                response.Id,
                response.ResponseText,
                response.CreatedAtUtc);
        }

        return new RatingResponse(
            rating.Id,
            rating.UserId,
            rating.Rating,
            rating.Comment,
            rating.CreatedAtUtc,
            rating.UpdatedAtUtc,
            responseInfo);
    }

    private Task<RatingResponse> BuildItemRatingResponseAsync(
        StoreItemRating rating,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new RatingResponse(
            rating.Id,
            rating.UserId,
            rating.Rating,
            rating.Comment,
            rating.CreatedAtUtc,
            rating.UpdatedAtUtc,
            null));
    }
}
