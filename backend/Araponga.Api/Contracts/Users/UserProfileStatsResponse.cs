namespace Araponga.Api.Contracts.Users;

/// <summary>
/// Resposta com estatísticas de contribuição territorial de um usuário.
/// </summary>
public sealed record UserProfileStatsResponse(
    Guid UserId,
    int PostsCreated,
    int EventsCreated,
    int EventsParticipated,
    int TerritoriesMember,
    int EntitiesConfirmed);
