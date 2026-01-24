namespace Araponga.Application.Models;

/// <summary>
/// Estatísticas de contribuição territorial de um usuário.
/// </summary>
public sealed record UserProfileStats(
    Guid UserId,
    int PostsCreated,
    int EventsCreated,
    int EventsParticipated,
    int TerritoriesMember,
    int EntitiesConfirmed);
