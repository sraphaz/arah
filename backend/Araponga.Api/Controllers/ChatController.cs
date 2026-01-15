using Araponga.Api.Contracts.Chat;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Chat;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/chat")]
[Produces("application/json")]
[Tags("Chat")]
public sealed class ChatController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ChatService _chatService;

    public ChatController(CurrentUserAccessor currentUserAccessor, ChatService chatService)
    {
        _currentUserAccessor = currentUserAccessor;
        _chatService = chatService;
    }

    [HttpGet("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(ConversationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ConversationResponse>> GetConversation(
        [FromRoute] Guid conversationId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.GetConversationAsync(conversationId, userContext.User.Id, cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return NotFound();
        }

        return Ok(ToConversationResponse(result.Value));
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(ListMessagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListMessagesResponse>> ListMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] DateTime? beforeCreatedAtUtc,
        [FromQuery] Guid? beforeMessageId,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.ListMessagesAsync(
            conversationId,
            userContext.User.Id,
            beforeCreatedAtUtc,
            beforeMessageId,
            limit,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        var items = result.Value.Select(ToMessageResponse).ToList();
        DateTime? nextAt = null;
        Guid? nextId = null;
        if (items.Count > 0)
        {
            var last = items[^1];
            nextAt = last.CreatedAtUtc;
            nextId = last.Id;
        }

        return Ok(new ListMessagesResponse(conversationId, items, nextAt, nextId));
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageResponse>> SendMessage(
        [FromRoute] Guid conversationId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.SendTextMessageAsync(
            conversationId,
            userContext.User.Id,
            request.Text,
            cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(ListMessages), new { conversationId }, ToMessageResponse(result.Value));
    }

    [HttpGet("conversations/{conversationId:guid}/participants")]
    [ProducesResponseType(typeof(ListParticipantsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListParticipantsResponse>> ListParticipants(
        [FromRoute] Guid conversationId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.ListParticipantsAsync(conversationId, userContext.User.Id, cancellationToken);
        if (result.IsFailure || result.Value is null)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(new ListParticipantsResponse(
            conversationId,
            result.Value.Select(ToParticipantResponse).ToList()));
    }

    [HttpPost("conversations/{conversationId:guid}/participants")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddParticipant(
        [FromRoute] Guid conversationId,
        [FromBody] AddParticipantRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.AddParticipantAsync(
            conversationId,
            userContext.User.Id,
            request.UserId,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("conversations/{conversationId:guid}/participants/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveParticipant(
        [FromRoute] Guid conversationId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.RemoveParticipantAsync(
            conversationId,
            userContext.User.Id,
            userId,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPost("conversations/{conversationId:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkRead(
        [FromRoute] Guid conversationId,
        [FromBody] MarkReadRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _chatService.MarkReadAsync(
            conversationId,
            userContext.User.Id,
            request.MessageId,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Forbidden.")
            {
                return Forbid();
            }

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
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

    private static MessageResponse ToMessageResponse(ChatMessage m)
    {
        return new MessageResponse(
            m.Id,
            m.ConversationId,
            m.SenderUserId,
            m.ContentType.ToString().ToUpperInvariant(),
            m.Text,
            m.CreatedAtUtc,
            m.EditedAtUtc);
    }

    private static ParticipantResponse ToParticipantResponse(ConversationParticipant p)
    {
        return new ParticipantResponse(
            p.UserId,
            p.Role.ToString().ToUpperInvariant(),
            p.JoinedAtUtc,
            p.LeftAtUtc,
            p.MutedUntilUtc,
            p.LastReadMessageId,
            p.LastReadAtUtc);
    }
}

