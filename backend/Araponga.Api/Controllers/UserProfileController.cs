using Araponga.Api.Contracts.Users;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/profile")]
[Produces("application/json")]
[Tags("User Profile")]
public sealed class UserProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;
    private readonly IUserInterestRepository _interestRepository;
    private readonly IVoteRepository? _voteRepository;
    private readonly IVotingRepository? _votingRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public UserProfileController(
        UserProfileService profileService,
        IUserInterestRepository interestRepository,
        CurrentUserAccessor currentUserAccessor,
        IVoteRepository? voteRepository = null,
        IVotingRepository? votingRepository = null)
    {
        _profileService = profileService;
        _interestRepository = interestRepository;
        _voteRepository = voteRepository;
        _votingRepository = votingRepository;
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

        var interests = await _interestRepository.ListByUserIdAsync(userContext.User.Id, cancellationToken);
        var interestTags = interests.Select(i => i.InterestTag).ToList();

        return Ok(MapToResponse(user, interestTags));
    }

    /// <summary>
    /// Atualiza o nome de exibição do usuário autenticado.
    /// </summary>
    [HttpPut("display-name")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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

        var interests = await _interestRepository.ListByUserIdAsync(userContext.User.Id, cancellationToken);
        var interestTags = interests.Select(i => i.InterestTag).ToList();

        return Ok(MapToResponse(user, interestTags));
    }

    /// <summary>
    /// Atualiza as informações de contato do usuário autenticado.
    /// </summary>
    [HttpPut("contact")]
    [EnableRateLimiting("write")]
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

        var interests = await _interestRepository.ListByUserIdAsync(userContext.User.Id, cancellationToken);
        var interestTags = interests.Select(i => i.InterestTag).ToList();

        return Ok(MapToResponse(user, interestTags));
    }

    /// <summary>
    /// Obtém o histórico de participação em governança do usuário autenticado.
    /// </summary>
    [HttpGet("governance")]
    [ProducesResponseType(typeof(UserProfileGovernanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileGovernanceResponse>> GetGovernanceHistory(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var votingHistory = new List<VotingHistoryItem>();
        var moderationContributions = 0;

        if (_voteRepository is not null && _votingRepository is not null)
        {
            var votes = await _voteRepository.ListByUserIdAsync(userContext.User.Id, cancellationToken);
            
            foreach (var vote in votes)
            {
                var voting = await _votingRepository.GetByIdAsync(vote.VotingId, cancellationToken);
                if (voting is not null)
                {
                    votingHistory.Add(new VotingHistoryItem(
                        vote.VotingId,
                        voting.Title,
                        voting.Type.ToString(),
                        vote.SelectedOption,
                        vote.CreatedAtUtc));
                }
            }
        }

        // Contar contribuições para moderação (simplificado - pode ser expandido)
        // Por enquanto, contamos apenas votações de moderação participadas
        moderationContributions = votingHistory.Count(v => v.VotingType == "ModerationRule");

        var response = new UserProfileGovernanceResponse(
            userContext.User.Id,
            votingHistory,
            moderationContributions);

        return Ok(response);
    }

    private static UserProfileResponse MapToResponse(Domain.Users.User user, IReadOnlyList<string> interests)
    {
        return new UserProfileResponse(
            user.Id,
            user.DisplayName,
            user.Email,
            user.PhoneNumber,
            user.Address,
            user.CreatedAtUtc,
            interests);
    }
}
