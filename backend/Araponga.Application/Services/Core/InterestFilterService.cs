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
        // Primeiro verifica tags explícitas, depois título/conteúdo como fallback
        var filtered = posts
            .Where(post =>
            {
                // Verificar tags explícitas primeiro (mais preciso)
                if (post.Tags.Count > 0)
                {
                    var postTags = post.Tags.Select(t => t.ToLowerInvariant()).ToHashSet();
                    if (interestTags.Overlaps(postTags))
                    {
                        return true;
                    }
                }

                // Fallback: verificar se algum interesse aparece no título ou conteúdo
                var titleLower = post.Title?.ToLowerInvariant() ?? "";
                var contentLower = post.Content?.ToLowerInvariant() ?? "";

                return interestTags.Any(tag =>
                    titleLower.Contains(tag) || contentLower.Contains(tag));
            })
            .ToList();

        return filtered;
    }
}
