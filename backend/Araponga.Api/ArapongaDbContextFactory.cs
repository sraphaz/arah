using Araponga.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Araponga.Api;

/// <summary>
/// Design-time factory used by EF tools to create migrations.
/// </summary>
public sealed class ArapongaDbContextFactory : IDesignTimeDbContextFactory<ArapongaDbContext>
{
    public ArapongaDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ARAPONGA_POSTGRES_CONNECTION") ??
            "Host=localhost;Database=araponga;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ArapongaDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ArapongaDbContext(optionsBuilder.Options);
    }
}

