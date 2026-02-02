using Araponga.Application;
using Araponga.Application.Interfaces;
using Araponga.Modules.Subscriptions.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Modules.Subscriptions;

public sealed class SubscriptionsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Postgres connection string is required");

        services.AddDbContext<SubscriptionsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            }));

        services.AddScoped<ISubscriptionPlanRepository, PostgresSubscriptionPlanRepository>();
        services.AddScoped<ISubscriptionRepository, PostgresSubscriptionRepository>();
        services.AddScoped<ISubscriptionPaymentRepository, PostgresSubscriptionPaymentRepository>();
        services.AddScoped<ICouponRepository, PostgresCouponRepository>();
        services.AddScoped<ISubscriptionCouponRepository, PostgresSubscriptionCouponRepository>();
        services.AddScoped<ISubscriptionPlanHistoryRepository, PostgresSubscriptionPlanHistoryRepository>();
    }
}
