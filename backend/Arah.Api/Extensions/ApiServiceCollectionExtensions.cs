using Arah.Api.HealthChecks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Arah.Api.Extensions;

/// <summary>
/// Registros de composição da API: compressão de resposta, controllers/JSON,
/// caching (memory/Redis), health checks e o gateway de pagamento por ambiente.
/// </summary>
public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddArahResponseCompression(this IServiceCollection services)
    {
        // Response Compression (gzip/brotli)
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/json", "application/xml", "text/plain", "text/css", "application/javascript" });
        });

        services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Optimal;
        });

        return services;
    }

    public static IServiceCollection AddArahControllers(this IServiceCollection services)
    {
        // Controllers with FluentValidation
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Otimizar serialização JSON
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = false; // Reduzir tamanho em produção
            });
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        return services;
    }

    public static IServiceCollection AddArahCaching(this IServiceCollection services, IConfiguration configuration)
    {
        // Memory cache
        services.AddMemoryCache();

        // Redis Cache Configuration (optional, falls back to IMemoryCache if not configured)
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }

        // Register distributed cache service with fallback
        services.AddSingleton<Arah.Application.Interfaces.IDistributedCacheService>(
            serviceProvider =>
            {
                var distributedCache = serviceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                var memoryCache = serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
                var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Arah.Infrastructure.Caching.RedisCacheService>>();
                return new Arah.Infrastructure.Caching.RedisCacheService(distributedCache, memoryCache, logger);
            });

        return services;
    }

    /// <summary>
    /// Health checks (self/cache/storage/event_bus e, quando Postgres, database).
    /// Deve ser chamado após AddInfrastructure para que as dependências estejam registradas.
    /// </summary>
    public static IServiceCollection AddArahHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // Health Checks Configuration
        var healthChecksBuilder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is healthy"), tags: new[] { "self", "live" })
            .AddCheck<CacheHealthCheck>("cache", tags: new[] { "cache", "ready" })
            .AddCheck<StorageHealthCheck>("storage", tags: new[] { "storage", "ready" })
            .AddCheck<EventBusHealthCheck>("event_bus", tags: new[] { "eventbus", "ready" });

        var persistenceProvider = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";

        // Add database health check when using Postgres (infrastructure must already be registered)
        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            healthChecksBuilder.AddCheck<DatabaseHealthCheck>(
                "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "ready", "postgres" });
        }

        return services;
    }

    public static IServiceCollection AddArahPaymentGateway(this IServiceCollection services, IWebHostEnvironment environment)
    {
        // Payment gateway (entrada de cobrança): o mock só é aceitável fora de produção.
        // Em produção, sem PSP real integrado, usa-se um gateway que falha explicitamente em vez
        // de criar intenções de pagamento falsas (pi_mock_*) que nenhum provedor real aprovaria.
        if (environment.IsProduction())
        {
            services.AddSingleton<Arah.Application.Interfaces.IPaymentGateway, Arah.Infrastructure.Payments.UnavailablePaymentGateway>();
        }
        else
        {
            // Singleton: o estado das intenções precisa sobreviver entre requisições (criar vs. confirmar).
            services.AddSingleton<Arah.Application.Interfaces.IPaymentGateway, Arah.Infrastructure.Payments.MockPaymentGateway>();
        }

        return services;
    }
}
