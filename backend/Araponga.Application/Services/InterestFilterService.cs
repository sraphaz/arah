using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para filtrar feed por interesses do usuário.
/// </summary>
public sealed class InterestFilterService
{
    private readonly IUserInterestRepository _interestRepository;

    public InterestFilterService(IUserInterestRepository interestRepository)
    {
        _interestRepository = interestRepository;
    }

    /// <summary>
    /// Filtra posts do feed baseado nos interesses do usuário.
    /// Posts que têm tags/categorias correspondentes aos interesses são mantidos.
    /// </summary>
    public async Task<IReadOnlyList<CommunityPost>> FilterFeedByInterestsAsync(
        IReadOnlyList<CommunityPost> posts,
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        if (posts.Count == 0)
        {
            return posts;
        }

        var userInterests = await _interestRepository.ListByUserIdAsync(userId, cancellationToken);
        if (userInterests.Count == 0)
        {
            // Se usuário não tem interesses, retorna feed completo
            return posts;
        }

        var interestTags = userInterests.Select(i => i.InterestTag.ToLowerInvariant()).ToHashSet();

        // Filtrar posts que têm tags correspondentes aos interesses
        // Por enquanto, vamos filtrar por palavras-chave no título e conteúdo
        // (futuramente pode ser expandido para usar tags/categorias explícitas nos posts)
        var filtered = posts
            .Where(post =>
            {
                var titleLower = post.Title?.ToLowerInvariant() ?? "";
                var contentLower = post.Content?.ToLowerInvariant() ?? "";

                // Verificar se algum interesse aparece no título ou conteúdo
                return interestTags.Any(tag =>
                    titleLower.Contains(tag) || contentLower.Contains(tag));
            })
            .ToList();

        return filtered;
    }
}
