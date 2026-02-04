using System.Security.Claims;

namespace Araponga.Application.Interfaces;

public interface ITokenService
{
    string IssueToken(Guid userId);
    /// <summary>Token para workers/sistemas (client credentials). Contém claim "client_id".</summary>
    string IssueSystemToken(string clientId);
    Guid? ParseToken(string token);
    /// <summary>Retorna o principal (user com sub ou system com client_id) para definir HttpContext.User.</summary>
    ClaimsPrincipal? GetPrincipal(string token);
    /// <summary>Expiração do access token em minutos (para ExpiresIn na resposta de login/refresh).</summary>
    int GetAccessTokenExpirationMinutes();
}
