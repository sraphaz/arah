using Araponga.Application.Interfaces;
using Araponga.Modules.Feed.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Feed;

public sealed class FeedModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<FeedDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new FeedUnitOfWorkParticipant(sp.GetRequiredService<FeedDbContext>()));
        services.AddScoped<IFeedRepository, PostgresFeedRepository>();
        services.AddScoped<IPostGeoAnchorRepository, PostgresPostGeoAnchorRepository>();
        services.AddScoped<IPostAssetRepository, PostgresPostAssetRepository>();
    }
}

internal sealed class FeedUnitOfWorkParticipant(FeedDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
