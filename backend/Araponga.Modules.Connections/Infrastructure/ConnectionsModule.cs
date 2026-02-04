using Araponga.Application.Interfaces;
using Araponga.Modules.Connections.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Connections;

public sealed class ConnectionsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<ConnectionsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new ConnectionsUnitOfWorkParticipant(sp.GetRequiredService<ConnectionsDbContext>()));
        services.AddScoped<IUserConnectionRepository, PostgresUserConnectionRepository>();
        services.AddScoped<IConnectionPrivacySettingsRepository, PostgresConnectionPrivacySettingsRepository>();
    }
}

internal sealed class ConnectionsUnitOfWorkParticipant(ConnectionsDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
