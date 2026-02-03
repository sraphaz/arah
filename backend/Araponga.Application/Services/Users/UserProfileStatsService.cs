using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para calcular estatísticas de contribuição territorial de um usuário.
/// </summary>
public sealed class UserProfileStatsService
{
    private readonly IFeedRepository? _feedRepository;
    private readonly ITerritoryEventRepository? _eventRepository;
    private readonly IEventParticipationRepository? _participationRepository;
    private readonly ITerritoryMembershipRepository? _membershipRepository;

    public UserProfileStatsService(
        IFeedRepository? feedRepository = null,
        ITerritoryEventRepository? eventRepository = null,
        IEventParticipationRepository? participationRepository = null,
        ITerritoryMembershipRepository? membershipRepository = null)
    {
        _feedRepository = feedRepository;
        _eventRepository = eventRepository;
        _participationRepository = participationRepository;
        _membershipRepository = membershipRepository;
    }

    /// <summary>
    /// Calcula as estatísticas de contribuição territorial de um usuário.
    /// </summary>
    public async Task<UserProfileStats> GetStatsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var postsCreated = _feedRepository is not null
            ? await _feedRepository.CountByAuthorAsync(userId, cancellationToken)
            : 0;

        var eventsCreated = _eventRepository is not null
            ? await _eventRepository.CountByAuthorAsync(userId, cancellationToken)
            : 0;

        var eventsParticipated = 0;
        if (_participationRepository is not null)
        {
            var participations = await _participationRepository.GetByUserIdAsync(userId, cancellationToken);
            // Contar apenas participações confirmadas ou interessadas
            eventsParticipated = participations.Count(p =>
                p.Status == Domain.Events.EventParticipationStatus.Confirmed ||
                p.Status == Domain.Events.EventParticipationStatus.Interested);
        }

        var territoriesMember = 0;
        if (_membershipRepository is not null)
        {
            var memberships = await _membershipRepository.ListByUserAsync(userId, cancellationToken);
            // Contar apenas memberships válidos (não apenas Visitor sem verificação)
            territoriesMember = memberships.Count(m =>
                m.Role == MembershipRole.Resident ||
                (m.Role == MembershipRole.Visitor && m.ResidencyVerification != ResidencyVerification.None));
        }

        // Entidades confirmadas: por enquanto retornar 0
        // Pode ser implementado no futuro se houver repositório específico
        var entitiesConfirmed = 0;

        return new UserProfileStats(
            userId,
            postsCreated,
            eventsCreated,
            eventsParticipated,
            territoriesMember,
            entitiesConfirmed);
    }
}
