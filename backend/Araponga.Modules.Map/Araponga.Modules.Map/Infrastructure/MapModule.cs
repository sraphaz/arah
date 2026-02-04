using Araponga.Application.Interfaces;
using Araponga.Modules.Map.Application.Interfaces;
using Araponga.Modules.Map.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Map;

public sealed class MapModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<MapDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new MapUnitOfWorkParticipant(sp.GetRequiredService<MapDbContext>()));
        services.AddScoped<IMapRepository, PostgresMapRepository>();
        services.AddScoped<IMapEntityRelationRepository, PostgresMapEntityRelationRepository>();
    }
}

internal sealed class MapUnitOfWorkParticipant(MapDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
