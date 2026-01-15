using Araponga.Api.Contracts.Chat;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/chat")]
[Produces("application/json")]
[Tags("Chat")]
public sealed class TerritoryChatController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ChatService _chatService;

    public TerritoryChatController(CurrentUserAccessor currentUserAccessor, ChatService chatService)
    {
        _currentUserAccessor = currentUserAccessor;
        _chatService = chatService;
    }

    [HttpGet("channels")]
    [ProducesResponseType(typeof(ListChannelsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListChannelsResponse>> ListChannels(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.ListOrCreateTerritoryChannelsAsync(
            territoryId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        var response = new ListChannelsResponse(
            territoryId,
            result.Value.Select(ToSummary).ToList());

        return Ok(response);
    }

    [HttpGet("groups")]
    [ProducesResponseType(typeof(ListGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListGroupsResponse>> ListGroups(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.ListActiveGroupsAsync(
            territoryId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new ListGroupsResponse(
            territoryId,
            result.Value.Select(ToSummary).ToList()));
    }

    [HttpPost("groups")]
    [ProducesResponseType(typeof(ConversationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConversationResponse>> CreateGroup(
        [FromRoute] Guid territoryId,
        [FromBody] CreateGroupRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.CreateGroupAsync(
            territoryId,
            userContext.User.Id,
            request.Name,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(ChatController.GetConversation),
            "Chat",
            new { conversationId = result.Value.Id },
            ToConversationResponse(result.Value));
    }

    [HttpPost("groups/{groupId:guid}/approve")]
    [ProducesResponseType(typeof(ConversationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConversationResponse>> ApproveGroup(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.ApproveGroupAsync(
            territoryId,
            groupId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(ToConversationResponse(result.Value));
    }

    [HttpPost("groups/{groupId:guid}/disable")]
    [ProducesResponseType(typeof(ConversationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConversationResponse>> DisableGroup(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.DisableGroupAsync(
            territoryId,
            groupId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(ToConversationResponse(result.Value));
    }

    [HttpPost("groups/{groupId:guid}/lock")]
    [ProducesResponseType(typeof(ConversationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConversationResponse>> LockGroup(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid groupId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.LockGroupAsync(
            territoryId,
            groupId,
            userContext.User.Id,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(ToConversationResponse(result.Value));
    }

    private static ConversationSummaryResponse ToSummary(ChatConversation c)
    {
        return new ConversationSummaryResponse(
            c.Id,
            c.TerritoryId,
            c.Kind.ToString().ToUpperInvariant(),
            c.Status.ToString().ToUpperInvariant(),
            c.Name,
            c.CreatedAtUtc);
    }

    private static ConversationResponse ToConversationResponse(ChatConversation c)
    {
        return new ConversationResponse(
            c.Id,
            c.TerritoryId,
            c.Kind.ToString().ToUpperInvariant(),
            c.Status.ToString().ToUpperInvariant(),
            c.Name,
            c.CreatedByUserId,
            c.CreatedAtUtc,
            c.ApprovedByUserId,
            c.ApprovedAtUtc,
            c.LockedByUserId,
            c.LockedAtUtc,
            c.DisabledByUserId,
            c.DisabledAtUtc);
    }
}

