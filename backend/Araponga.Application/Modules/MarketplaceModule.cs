using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Marketplace - serviços de marketplace e lojas.
/// Depende de Core. Usa conectores compartilhados de pagamento.
/// </summary>
public sealed class MarketplaceModule : ModuleBase
{
    public override string Id => "Marketplace";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Marketplace services
        services.AddScoped<StoreService>();
        services.AddScoped<StoreItemService>();
        services.AddScoped<InquiryService>();
        services.AddScoped<PlatformFeeService>();
        services.AddScoped<CartService>();
        services.AddScoped<SellerPayoutService>();
        services.AddScoped<TerritoryPayoutConfigService>();
        services.AddScoped<RatingService>();
        services.AddScoped<MarketplaceSearchService>();
        services.AddScoped<UserActivityService>();

        // Nota: Gateways de pagamento (ISubscriptionGateway, IPayoutGateway) são conectores
        // compartilhados registrados no host, não neste módulo.
    }
}
