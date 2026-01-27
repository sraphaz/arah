using Araponga.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Araponga.Infrastructure.Payments;

/// <summary>
/// Factory para criar instâncias de gateways de assinatura baseado na configuração.
/// </summary>
public sealed class SubscriptionGatewayFactory : ISubscriptionGatewayFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public SubscriptionGatewayFactory(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    /// <summary>
    /// Obtém o gateway de assinatura configurado.
    /// </summary>
    public ISubscriptionGateway? GetGateway()
    {
        var gatewayName = _configuration["Subscription:Gateway"] ?? "Stripe";

        return gatewayName.ToLowerInvariant() switch
        {
            "stripe" => _serviceProvider.GetService<ISubscriptionGateway>(),
            "mercadopago" or "mercado-pago" or "mercado_pago" => 
                _serviceProvider.GetServices<ISubscriptionGateway>()
                    .FirstOrDefault(g => g.GatewayName == "MercadoPago"),
            _ => _serviceProvider.GetService<ISubscriptionGateway>() // Default: Stripe
        };
    }

    /// <summary>
    /// Obtém um gateway específico pelo nome.
    /// </summary>
    public ISubscriptionGateway? GetGateway(string gatewayName)
    {
        return _serviceProvider.GetServices<ISubscriptionGateway>()
            .FirstOrDefault(g => g.GatewayName.Equals(gatewayName, StringComparison.OrdinalIgnoreCase));
    }
}
