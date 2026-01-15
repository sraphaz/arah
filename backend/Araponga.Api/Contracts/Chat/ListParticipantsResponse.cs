namespace Araponga.Api.Contracts.Chat;

public sealed record ListParticipantsResponse(
    Guid ConversationId,
    IReadOnlyList<ParticipantResponse> Participants);

