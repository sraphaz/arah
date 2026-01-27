namespace Araponga.Application.Interfaces;

/// <summary>
/// Factory para obter gateways de assinatura.
/// </summary>
public interface ISubscriptionGatewayFactory
{
    /// <summary>
    /// Obtém o gateway de assinatura configurado.
    /// </summary>
    ISubscriptionGateway? GetGateway();

    /// <summary>
    /// Obtém um gateway específico pelo nome.
    /// </summary>
    ISubscriptionGateway? GetGateway(string gatewayName);
}
