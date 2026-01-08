using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresUserRepository : IUserRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresUserRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByProviderAsync(string provider, string externalId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Provider == provider && user.ExternalId == externalId,
                cancellationToken);
        return record?.ToDomain();
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Add(user.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
