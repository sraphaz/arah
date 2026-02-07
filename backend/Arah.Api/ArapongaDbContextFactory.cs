using Arah.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Arah.Api;

/// <summary>
/// Design-time factory used by EF tools to create migrations.
/// </summary>
public sealed class ArahDbContextFactory : IDesignTimeDbContextFactory<ArahDbContext>
{
    public ArahDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("Arah_POSTGRES_CONNECTION") ??
            "Host=localhost;Database=arah;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ArahDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ArahDbContext(optionsBuilder.Options);
    }
}

