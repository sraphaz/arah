namespace Araponga.Application.Interfaces;

public interface ITokenService
{
    string IssueToken(Guid userId);
    Guid? ParseToken(string token);
}
