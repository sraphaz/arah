using Araponga.Application;
using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Marketplace;

public sealed class MarketplaceModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres") 
            ?? throw new InvalidOperationException("Postgres connection string is required");

        // Registrar MarketplaceDbContext
        services.AddDbContext<MarketplaceDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        // TODO: Registrar repositórios de Marketplace quando forem criados
        // services.AddScoped<IStoreRepository, PostgresStoreRepository>();
        // services.AddScoped<IStoreItemRepository, PostgresStoreItemRepository>();
        // etc.
    }
}
