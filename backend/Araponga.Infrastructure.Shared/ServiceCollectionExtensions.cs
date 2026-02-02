using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Shared.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Infrastructure.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        // Registrar SharedDbContext
        services.AddDbContext<SharedDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        // Registrar SharedDbContext como IUnitOfWork também
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SharedDbContext>());

        return services;
    }

    public static IServiceCollection AddSharedCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar repositórios compartilhados que usam SharedDbContext
        // TODO: Mover repositórios compartilhados para cá quando forem criados
        // services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
        // services.AddScoped<IUserRepository, PostgresUserRepository>();
        // etc.

        return services;
    }
}
