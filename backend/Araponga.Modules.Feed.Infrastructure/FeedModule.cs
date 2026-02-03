using Araponga.Application;
using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Shared;
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

        // Registrar FeedDbContext
        services.AddDbContext<FeedDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        // Participante do UoW composto para que CommitAsync persista alterações do Feed junto com os demais contextos
        services.AddScoped<IUnitOfWorkParticipant>(sp => new DbContextUnitOfWorkParticipant(sp.GetRequiredService<FeedDbContext>()));

        // Registrar repositórios de Feed
        services.AddScoped<IFeedRepository, PostgresFeedRepository>();
    }
}
