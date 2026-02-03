using Araponga.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Shared;

/// <summary>
/// Adapta um DbContext (EF Core) para IUnitOfWorkParticipant, permitindo que seja incluído
/// em um CompositeUnitOfWork e commitado em uma única operação.
/// </summary>
public sealed class DbContextUnitOfWorkParticipant : IUnitOfWorkParticipant
{
    private readonly DbContext _context;

    public DbContextUnitOfWorkParticipant(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task CommitAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
