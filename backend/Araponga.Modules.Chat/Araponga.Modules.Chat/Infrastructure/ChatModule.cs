using Araponga.Application.Interfaces;
using Araponga.Modules.Chat.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Chat;

public sealed class ChatModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<ChatDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWorkParticipant>(sp => new ChatUnitOfWorkParticipant(sp.GetRequiredService<ChatDbContext>()));
        services.AddScoped<IChatConversationRepository, PostgresChatConversationRepository>();
        services.AddScoped<IChatConversationParticipantRepository, PostgresChatConversationParticipantRepository>();
        services.AddScoped<IChatMessageRepository, PostgresChatMessageRepository>();
        services.AddScoped<IChatConversationStatsRepository, PostgresChatConversationStatsRepository>();
    }
}

internal sealed class ChatUnitOfWorkParticipant(ChatDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
