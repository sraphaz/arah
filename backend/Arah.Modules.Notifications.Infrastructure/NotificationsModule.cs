using Arah.Application.Interfaces;
using Arah.Application.Interfaces.Notifications;
using Arah.Modules.Notifications.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arah.Modules.Notifications;

public sealed class NotificationsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new NotificationsUnitOfWorkParticipant(sp.GetRequiredService<NotificationsDbContext>()));
        services.AddScoped<INotificationInboxRepository, PostgresNotificationInboxRepository>();
        services.AddScoped<INotificationConfigRepository, PostgresNotificationConfigRepository>();
    }
}

internal sealed class NotificationsUnitOfWorkParticipant(NotificationsDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
