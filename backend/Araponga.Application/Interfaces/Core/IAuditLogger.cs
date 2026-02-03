using Araponga.Application.Models;

namespace Araponga.Application.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(AuditEntry entry, CancellationToken cancellationToken);
}
