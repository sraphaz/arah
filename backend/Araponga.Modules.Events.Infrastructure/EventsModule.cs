using Araponga.Application;
using Araponga.Application.Interfaces;
using Araponga.Modules.Events.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Events;

public sealed class EventsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres") 
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<EventsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        // TODO: Registrar repositórios de Events quando forem criados
        // services.AddScoped<ITerritoryEventRepository, PostgresTerritoryEventRepository>();
        // services.AddScoped<IEventParticipationRepository, PostgresEventParticipationRepository>();
    }
}
