using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Araponga.Infrastructure.Payments;

/// <summary>
/// Serviço de integração com Stripe Subscriptions.
/// Por enquanto, implementação mock para desenvolvimento.
/// Em produção, implementar com Stripe.net SDK.
/// </summary>
public sealed class StripeSubscriptionService : ISubscriptionGateway, IStripeSubscriptionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripeSubscriptionService> _logger;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public string GatewayName => "Stripe";

    private readonly bool _useRealStripe;

    public StripeSubscriptionService(
        IConfiguration configuration,
        ILogger<StripeSubscriptionService> logger,
        ISubscriptionPlanRepository planRepository,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _planRepository = planRepository;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;

        // Verificar se há credenciais configuradas
        var secretKey = _configuration["Stripe:SecretKey"];
        _useRealStripe = !string.IsNullOrWhiteSpace(secretKey);

        if (_useRealStripe)
        {
            StripeConfiguration.ApiKey = secretKey;
            _logger.LogInformation("Stripe integration enabled with real API");
        }
        else
        {
            _logger.LogWarning("Stripe secret key not configured. Using mock implementation.");
        }
    }

    public async Task<OperationResult<StripeSubscriptionResult>> CreateSubscriptionAsync(
        Guid userId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken)
    {
        // Buscar plano
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return OperationResult<StripeSubscriptionResult>.Failure("Plan not found.");
        }

        // Se for plano FREE, não precisa criar no Stripe
        if (plan.Tier == SubscriptionPlanTier.FREE)
        {
            return OperationResult<StripeSubscriptionResult>.Failure("FREE plan does not require Stripe subscription.");
        }

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return OperationResult<StripeSubscriptionResult>.Failure("User not found.");
        }

        if (!_useRealStripe)
        {
            // Usar mock se não houver credenciais
            return CreateMockSubscription(plan);
        }

        try
        {
            // Buscar ou criar cliente no Stripe
            var customerService = new CustomerService();
            Customer customer;

            // Tentar buscar cliente existente pelo email
            var customerListOptions = new CustomerListOptions
            {
                Email = user.Email,
                Limit = 1
            };
            var existingCustomers = await customerService.ListAsync(customerListOptions, cancellationToken: cancellationToken);
            
            if (existingCustomers.Data.Count > 0)
            {
                customer = existingCustomers.Data[0];
            }
            else
            {
                // Criar novo cliente
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = user.DisplayName,
                    Metadata = new Dictionary<string, string>
                    {
                        ["userId"] = userId.ToString()
                    }
                };
                customer = await customerService.CreateAsync(customerCreateOptions, cancellationToken: cancellationToken);
            }

            // Verificar se o plano tem StripePriceId configurado
            string? priceId = plan.StripePriceId;
            if (string.IsNullOrWhiteSpace(priceId))
            {
                _logger.LogWarning("Plan {PlanId} does not have StripePriceId configured. Creating price on the fly.", planId);
                // Criar preço dinamicamente (não ideal, mas funcional)
                var priceService = new PriceService();
                var priceOptions = new PriceCreateOptions
                {
                    Currency = "brl",
                    UnitAmount = (long)(plan.PricePerCycle!.Value * 100), // Stripe usa centavos
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = MapBillingCycleToStripeInterval(plan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY)
                    },
                    Product = plan.StripeProductId ?? "prod_default"
                };
                var price = await priceService.CreateAsync(priceOptions, cancellationToken: cancellationToken);
                priceId = price.Id;
                
                _logger.LogInformation("Created Stripe price {PriceId} for plan {PlanId}", priceId, planId);
            }

            // Criar assinatura no Stripe
            var subscriptionService = new SubscriptionService();
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["userId"] = userId.ToString(),
                    ["planId"] = planId.ToString()
                }
            };

            // Aplicar cupom se fornecido
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                subscriptionOptions.Coupon = couponCode;
            }

            // Aplicar trial se o plano tiver
            if (plan.TrialDays.HasValue && plan.TrialDays.Value > 0)
            {
                subscriptionOptions.TrialPeriodDays = plan.TrialDays.Value;
            }

            var stripeSubscription = await subscriptionService.CreateAsync(subscriptionOptions, cancellationToken: cancellationToken);

            var result = new StripeSubscriptionResult
            {
                StripeSubscriptionId = stripeSubscription.Id,
                StripeCustomerId = customer.Id,
                CurrentPeriodStart = stripeSubscription.CurrentPeriodStart,
                CurrentPeriodEnd = stripeSubscription.CurrentPeriodEnd,
                Status = MapStripeStatus(stripeSubscription.Status)
            };

            _logger.LogInformation("Created Stripe subscription {SubscriptionId} for user {UserId}", stripeSubscription.Id, userId);
            return OperationResult<StripeSubscriptionResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error creating subscription for user {UserId}", userId);
            return OperationResult<StripeSubscriptionResult>.Failure($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Stripe subscription for user {UserId}", userId);
            return OperationResult<StripeSubscriptionResult>.Failure($"Error: {ex.Message}");
        }
    }

    // Implementação da interface genérica ISubscriptionGateway
    Task<OperationResult<SubscriptionGatewayResult>> ISubscriptionGateway.CreateSubscriptionAsync(
        Guid userId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken)
    {
        return CreateSubscriptionAsync(userId, planId, couponCode, cancellationToken)
            .ContinueWith(t =>
            {
                if (t.Result.IsFailure)
                {
                    return OperationResult<SubscriptionGatewayResult>.Failure(t.Result.Error ?? "Unknown error");
                }

                var stripeResult = t.Result.Value!;
                var gatewayResult = new SubscriptionGatewayResult
                {
                    GatewaySubscriptionId = stripeResult.StripeSubscriptionId,
                    GatewayCustomerId = stripeResult.StripeCustomerId,
                    CurrentPeriodStart = stripeResult.CurrentPeriodStart,
                    CurrentPeriodEnd = stripeResult.CurrentPeriodEnd,
                    Status = stripeResult.Status
                };

                return OperationResult<SubscriptionGatewayResult>.Success(gatewayResult);
            }, cancellationToken);
    }

    Task<OperationResult<SubscriptionGatewayResult>> ISubscriptionGateway.UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken)
    {
        return UpdateSubscriptionAsync(subscriptionId, newPlanId, cancellationToken)
            .ContinueWith(t =>
            {
                if (t.Result.IsFailure)
                {
                    return OperationResult<SubscriptionGatewayResult>.Failure(t.Result.Error ?? "Unknown error");
                }

                var stripeResult = t.Result.Value!;
                var gatewayResult = new SubscriptionGatewayResult
                {
                    GatewaySubscriptionId = stripeResult.StripeSubscriptionId,
                    GatewayCustomerId = stripeResult.StripeCustomerId,
                    CurrentPeriodStart = stripeResult.CurrentPeriodStart,
                    CurrentPeriodEnd = stripeResult.CurrentPeriodEnd,
                    Status = stripeResult.Status
                };

                return OperationResult<SubscriptionGatewayResult>.Success(gatewayResult);
            }, cancellationToken);
    }

    Task<SubscriptionGatewayInfo?> ISubscriptionGateway.GetSubscriptionAsync(
        string gatewaySubscriptionId,
        CancellationToken cancellationToken)
    {
        return GetSubscriptionAsync(gatewaySubscriptionId, cancellationToken)
            .ContinueWith(t =>
            {
                var stripeInfo = t.Result;
                if (stripeInfo == null)
                {
                    return null;
                }

                return new SubscriptionGatewayInfo
                {
                    GatewaySubscriptionId = stripeInfo.StripeSubscriptionId,
                    GatewayCustomerId = stripeInfo.StripeCustomerId,
                    Status = stripeInfo.Status,
                    CurrentPeriodStart = stripeInfo.CurrentPeriodStart,
                    CurrentPeriodEnd = stripeInfo.CurrentPeriodEnd,
                    TrialEnd = stripeInfo.TrialEnd,
                    CancelAtPeriodEnd = stripeInfo.CancelAtPeriodEnd
                };
            }, cancellationToken);
    }

    public async Task<OperationResult<StripeSubscriptionResult>> UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(newPlanId, cancellationToken);
        if (plan == null)
        {
            return OperationResult<StripeSubscriptionResult>.Failure("Plan not found.");
        }

        if (!_useRealStripe)
        {
            return CreateMockSubscription(plan);
        }

        try
        {
            // Buscar assinatura local para obter StripeSubscriptionId
            var localSubscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
            if (localSubscription == null || string.IsNullOrWhiteSpace(localSubscription.StripeSubscriptionId))
            {
                return OperationResult<StripeSubscriptionResult>.Failure("Subscription not found or does not have Stripe subscription ID.");
            }

            var subscriptionService = new SubscriptionService();
            var stripeSubscriptionId = localSubscription.StripeSubscriptionId;
            
            // Buscar assinatura atual no Stripe
            var currentSubscription = await subscriptionService.GetAsync(stripeSubscriptionId, cancellationToken: cancellationToken);
            
            // Verificar se o novo plano tem StripePriceId
            string? newPriceId = plan.StripePriceId;
            if (string.IsNullOrWhiteSpace(newPriceId))
            {
                _logger.LogWarning("Plan {PlanId} does not have StripePriceId. Cannot update subscription.", newPlanId);
                return OperationResult<StripeSubscriptionResult>.Failure("Plan does not have StripePriceId configured.");
            }
            
            // Atualizar item da assinatura
            var updateOptions = new SubscriptionUpdateOptions
            {
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Id = currentSubscription.Items.Data[0].Id,
                        Price = newPriceId
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["planId"] = newPlanId.ToString()
                },
                ProrationBehavior = "create_prorations" // Criar proratação automática
            };

            var updatedSubscription = await subscriptionService.UpdateAsync(stripeSubscriptionId, updateOptions, cancellationToken: cancellationToken);

            var result = new StripeSubscriptionResult
            {
                StripeSubscriptionId = updatedSubscription.Id,
                StripeCustomerId = updatedSubscription.CustomerId,
                CurrentPeriodStart = updatedSubscription.CurrentPeriodStart,
                CurrentPeriodEnd = updatedSubscription.CurrentPeriodEnd,
                Status = MapStripeStatus(updatedSubscription.Status)
            };

            _logger.LogInformation("Updated Stripe subscription {SubscriptionId} to plan {PlanId}", stripeSubscriptionId, newPlanId);
            return OperationResult<StripeSubscriptionResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error updating subscription {SubscriptionId}", subscriptionId);
            return OperationResult<StripeSubscriptionResult>.Failure($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Stripe subscription {SubscriptionId}", subscriptionId);
            return OperationResult<StripeSubscriptionResult>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<OperationResult> CancelSubscriptionAsync(
        Guid subscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken)
    {
        if (!_useRealStripe)
        {
            _logger.LogWarning("StripeSubscriptionService: Using mock implementation for cancel.");
            return OperationResult.Success();
        }

        try
        {
            // Buscar assinatura local para obter StripeSubscriptionId
            var localSubscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
            if (localSubscription == null || string.IsNullOrWhiteSpace(localSubscription.StripeSubscriptionId))
            {
                return OperationResult.Failure("Subscription not found or does not have Stripe subscription ID.");
            }

            var subscriptionService = new SubscriptionService();
            var stripeSubscriptionId = localSubscription.StripeSubscriptionId;

            if (cancelAtPeriodEnd)
            {
                // Cancelar ao fim do período
                var updateOptions = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
                };
                await subscriptionService.UpdateAsync(stripeSubscriptionId, updateOptions, cancellationToken: cancellationToken);
                _logger.LogInformation("Scheduled cancellation of Stripe subscription {SubscriptionId} at period end", stripeSubscriptionId);
            }
            else
            {
                // Cancelar imediatamente
                await subscriptionService.CancelAsync(stripeSubscriptionId, cancellationToken: cancellationToken);
                _logger.LogInformation("Cancelled Stripe subscription {SubscriptionId} immediately", stripeSubscriptionId);
            }

            return OperationResult.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error cancelling subscription {SubscriptionId}", subscriptionId);
            return OperationResult.Failure($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling Stripe subscription {SubscriptionId}", subscriptionId);
            return OperationResult.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<OperationResult> ReactivateSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        if (!_useRealStripe)
        {
            _logger.LogWarning("StripeSubscriptionService: Using mock implementation for reactivate.");
            return OperationResult.Success();
        }

        try
        {
            // Buscar assinatura local para obter StripeSubscriptionId
            var localSubscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
            if (localSubscription == null || string.IsNullOrWhiteSpace(localSubscription.StripeSubscriptionId))
            {
                return OperationResult.Failure("Subscription not found or does not have Stripe subscription ID.");
            }

            var subscriptionService = new SubscriptionService();
            var stripeSubscriptionId = localSubscription.StripeSubscriptionId;

            // Reativar assinatura cancelada
            var updateOptions = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false
            };
            await subscriptionService.UpdateAsync(stripeSubscriptionId, updateOptions, cancellationToken: cancellationToken);

            _logger.LogInformation("Reactivated Stripe subscription {SubscriptionId}", stripeSubscriptionId);
            return OperationResult.Success();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error reactivating subscription {SubscriptionId}", subscriptionId);
            return OperationResult.Failure($"Stripe error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating Stripe subscription {SubscriptionId}", subscriptionId);
            return OperationResult.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<StripeSubscriptionInfo?> GetSubscriptionAsync(
        string stripeSubscriptionId,
        CancellationToken cancellationToken)
    {
        if (!_useRealStripe)
        {
            _logger.LogWarning("StripeSubscriptionService: Using mock implementation for get subscription.");
            return null;
        }

        try
        {
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.GetAsync(stripeSubscriptionId, cancellationToken: cancellationToken);

            return new StripeSubscriptionInfo
            {
                StripeSubscriptionId = subscription.Id,
                StripeCustomerId = subscription.CustomerId,
                Status = MapStripeStatus(subscription.Status),
                CurrentPeriodStart = subscription.CurrentPeriodStart,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                TrialEnd = subscription.TrialEnd,
                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd
            };
        }
        catch (StripeException ex) when (ex.StripeError?.Code == "resource_missing")
        {
            _logger.LogWarning("Stripe subscription {SubscriptionId} not found", stripeSubscriptionId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Stripe subscription {SubscriptionId}", stripeSubscriptionId);
            return null;
        }
    }

    private OperationResult<StripeSubscriptionResult> CreateMockSubscription(SubscriptionPlan plan)
    {
        var mockSubscriptionId = $"sub_mock_{Guid.NewGuid():N}";
        var mockCustomerId = $"cus_mock_{Guid.NewGuid():N}";
        var now = DateTime.UtcNow;
        var periodEnd = CalculatePeriodEnd(now, plan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY);

        var result = new StripeSubscriptionResult
        {
            StripeSubscriptionId = mockSubscriptionId,
            StripeCustomerId = mockCustomerId,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = periodEnd,
            Status = SubscriptionStatus.ACTIVE
        };

        return OperationResult<StripeSubscriptionResult>.Success(result);
    }

    private static string MapBillingCycleToStripeInterval(SubscriptionBillingCycle cycle)
    {
        return cycle switch
        {
            SubscriptionBillingCycle.MONTHLY => "month",
            SubscriptionBillingCycle.QUARTERLY => "month", // Stripe não tem quarterly nativo, usar 3 meses
            SubscriptionBillingCycle.YEARLY => "year",
            _ => "month"
        };
    }

    private static SubscriptionStatus MapStripeStatus(string stripeStatus)
    {
        return stripeStatus switch
        {
            "active" => SubscriptionStatus.ACTIVE,
            "canceled" => SubscriptionStatus.CANCELED,
            "past_due" => SubscriptionStatus.PAST_DUE,
            "unpaid" => SubscriptionStatus.UNPAID,
            "trialing" => SubscriptionStatus.TRIALING,
            "incomplete" => SubscriptionStatus.UNPAID,
            "incomplete_expired" => SubscriptionStatus.EXPIRED,
            _ => SubscriptionStatus.ACTIVE
        };
    }

    private static DateTime CalculatePeriodEnd(DateTime start, SubscriptionBillingCycle cycle)
    {
        return cycle switch
        {
            SubscriptionBillingCycle.MONTHLY => start.AddMonths(1),
            SubscriptionBillingCycle.QUARTERLY => start.AddMonths(3),
            SubscriptionBillingCycle.YEARLY => start.AddYears(1),
            _ => start.AddMonths(1)
        };
    }
}
