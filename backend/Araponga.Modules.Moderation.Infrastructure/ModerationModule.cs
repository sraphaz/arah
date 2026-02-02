using Araponga.Application;
using Araponga.Application.Interfaces;
using Araponga.Modules.Moderation.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Moderation;

public sealed class ModerationModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<ModerationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<IReportRepository, PostgresReportRepository>();
        services.AddScoped<ISanctionRepository, PostgresSanctionRepository>();
        services.AddScoped<IWorkItemRepository, PostgresWorkItemRepository>();
        services.AddScoped<IDocumentEvidenceRepository, PostgresDocumentEvidenceRepository>();
        services.AddScoped<ITerritoryModerationRuleRepository, PostgresTerritoryModerationRuleRepository>();
    }
}
