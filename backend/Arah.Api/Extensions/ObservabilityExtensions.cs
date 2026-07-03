using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Arah.Api.Extensions;

/// <summary>
/// Observabilidade: logging estruturado (Serilog) e telemetria (OpenTelemetry tracing + metrics).
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Configura o Serilog como logger central (console, arquivo e opcionalmente Seq).
    /// </summary>
    public static WebApplicationBuilder AddArahSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            var seqUrl = context.Configuration["Logging:Seq:ServerUrl"];
            var logLevel = context.Configuration["Logging:LogLevel:Default"] ?? "Information";
            var minLevel = Enum.TryParse<Serilog.Events.LogEventLevel>(logLevel, true, out var level) ? level : Serilog.Events.LogEventLevel.Information;

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", "Arah")
                .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown")
                .MinimumLevel.Is(minLevel)
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    "logs/Arah-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] [{MachineName}] [{ThreadId}] {Message:lj}{NewLine}{Exception}");

            // Add Seq sink if configured
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                configuration.WriteTo.Seq(
                    serverUrl: seqUrl,
                    apiKey: context.Configuration["Logging:Seq:ApiKey"],
                    restrictedToMinimumLevel: minLevel);
            }
        });

        return builder;
    }

    /// <summary>
    /// Configura OpenTelemetry (tracing + metrics) com exportadores OTLP/Jaeger/Console.
    /// O endpoint Prometheus é exposto no pipeline via UseMetricServer().
    /// </summary>
    public static IServiceCollection AddArahObservability(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // OpenTelemetry Configuration
        var serviceName = "Arah";
        var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        var otlpEndpoint = configuration["OpenTelemetry:Otlp:Endpoint"];
        var jaegerEndpoint = configuration["OpenTelemetry:Jaeger:Endpoint"];

        var otelBuilder = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion));

        // Tracing
        otelBuilder.WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("Arah.*");

            // Exporters
            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            }
            else if (!string.IsNullOrWhiteSpace(jaegerEndpoint))
            {
                tracing.AddJaegerExporter(options => options.Endpoint = new Uri(jaegerEndpoint));
            }
            else
            {
                // Console exporter para desenvolvimento
                if (environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }
            }
        });

        // Metrics
        otelBuilder.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("Arah");

            // Exporters
            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            }
            else if (environment.IsDevelopment())
            {
                metrics.AddConsoleExporter();
            }
        });

        // Prometheus Metrics - configured via UseMetricServer() in app pipeline

        return services;
    }
}
