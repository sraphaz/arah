using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
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

        services.AddScoped<IUnitOfWorkParticipant>(sp => new MarketplaceUnitOfWorkParticipant(sp.GetRequiredService<MarketplaceDbContext>()));
        services.AddScoped<IStoreRepository, PostgresStoreRepository>();
        services.AddScoped<IStoreItemRepository, PostgresStoreItemRepository>();
        services.AddScoped<IInquiryRepository, PostgresInquiryRepository>();
        services.AddScoped<IStoreRatingRepository, PostgresStoreRatingRepository>();
        services.AddScoped<IStoreItemRatingRepository, PostgresStoreItemRatingRepository>();
        services.AddScoped<IStoreRatingResponseRepository, PostgresStoreRatingResponseRepository>();
        services.AddScoped<ICartRepository, PostgresCartRepository>();
        services.AddScoped<ICartItemRepository, PostgresCartItemRepository>();
        services.AddScoped<ICheckoutRepository, PostgresCheckoutRepository>();
        services.AddScoped<ICheckoutItemRepository, PostgresCheckoutItemRepository>();
        services.AddScoped<IPlatformFeeConfigRepository, PostgresPlatformFeeConfigRepository>();
        services.AddScoped<ITerritoryPayoutConfigRepository, PostgresTerritoryPayoutConfigRepository>();
    }
}

internal sealed class MarketplaceUnitOfWorkParticipant(MarketplaceDbContext context) : IUnitOfWorkParticipant
{
    public Task CommitAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
