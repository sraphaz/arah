using Araponga.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Application.Modules;

/// <summary>
/// Módulo Subscriptions - serviços de assinaturas e planos.
/// Depende de Core. Usa conectores compartilhados de pagamento.
/// </summary>
public sealed class SubscriptionsModule : ModuleBase
{
    public override string Id => "Subscriptions";
    public override string[] DependsOn => new[] { "Core" };
    public override bool IsRequired => false;

    public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Subscription services
        services.AddScoped<SubscriptionService>();
        services.AddScoped<CouponService>();
        services.AddScoped<SubscriptionPlanAdminService>();
        services.AddScoped<SubscriptionCapabilityService>();
        services.AddScoped<SubscriptionPlanSeedService>();
        services.AddScoped<SubscriptionTrialService>();
        services.AddScoped<SubscriptionRenewalService>();
        services.AddScoped<SubscriptionAnalyticsService>();
        services.AddScoped<StripeWebhookService>();
        services.AddScoped<MercadoPagoWebhookService>();

        // Nota: ISubscriptionGateway, ISubscriptionGatewayFactory e IStripeSubscriptionService
        // são conectores compartilhados registrados em AddConnectors (estão no Infrastructure).
        // IPayoutGateway também é um conector compartilhado registrado no host.
    }
}
