using Arah.Application.Interfaces;
using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arah.Modules.Assets;

public sealed class AssetsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<AssetsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new AssetsUnitOfWorkParticipant(sp.GetRequiredService<AssetsDbContext>()));
        services.AddScoped<ITerritoryAssetRepository, PostgresAssetRepository>();
        services.AddScoped<IAssetGeoAnchorRepository, PostgresAssetGeoAnchorRepository>();
        services.AddScoped<IAssetValidationRepository, PostgresAssetValidationRepository>();
    }
}

internal sealed class AssetsUnitOfWorkParticipant(AssetsDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
