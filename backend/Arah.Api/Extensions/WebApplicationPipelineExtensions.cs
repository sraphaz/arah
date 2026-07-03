using Arah.Api.Middleware;
using Arah.Application.Metrics;
using Arah.Application.Services;
using Arah.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Serilog;

namespace Arah.Api.Extensions;

/// <summary>
/// Pipeline HTTP do Arah. A ORDEM dos middlewares é significativa e deve ser preservada:
/// migrações (startup), métricas, logging, exception handler, arquivos estáticos, CORS,
/// headers de segurança, correlação, rate limiting, auth e mapeamento de endpoints.
/// </summary>
public static class WebApplicationPipelineExtensions
{
    public static WebApplication UseArahPipeline(this WebApplication app)
    {
        var persistenceProvider = app.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";

        // Aplicar migrações do Postgres quando Persistence:ApplyMigrations = true (ex.: Docker dev)
        var applyMigrations = app.Configuration.GetValue<bool>("Persistence:ApplyMigrations");
        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase) && applyMigrations)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            const int maxRetries = 10;
            const int delaySeconds = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using (var scope = app.Services.CreateScope())
                    {
                        var mainDb = scope.ServiceProvider.GetRequiredService<ArahDbContext>();
                        mainDb.Database.Migrate();
                    }
                    logger.LogInformation("ArahDbContext migrations applied successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Attempt {Attempt}/{Max} failed to apply migrations (waiting {Delay}s before retry)", attempt, maxRetries, delaySeconds);
                    if (attempt == maxRetries)
                    {
                        logger.LogError(ex, "Failed to apply ArahDbContext migrations after {Max} attempts. Ensure Postgres is running and connection string is correct.", maxRetries);
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }

        // Configure connection pool metrics if using Postgres
        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var metricsService = app.Services.GetRequiredService<ConnectionPoolMetricsService>();
                ConnectionPoolMetricsService.ConfigureMetrics(metricsService);
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Failed to configure connection pool metrics");
            }
        }

        // Prometheus Metrics Endpoint
        app.UseMetricServer();
        app.UseHttpMetrics();

        // Serilog Request Logging
        app.UseSerilogRequestLogging();

        // HTTPS Redirection and HSTS (Production only)
        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        // Exception Handler
        app.UseArahExceptionHandler();

        // Swagger only in Development (padrão)
        app.UseArahSwagger();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/devportal")
            {
                context.Request.Path = "/devportal/index.html";
            }

            await next();
        });

        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context =>
            {
                if (string.Equals(context.File.Name, "index.html", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(context.File.Name, "config.html", StringComparison.OrdinalIgnoreCase))
                {
                    context.Context.Response.ContentType = "text/html; charset=utf-8";
                }
            }
        });

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            RequestPath = "/devportal",
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "devportal"))
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/devportal",
            FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "devportal")),
            OnPrepareResponse = context =>
            {
                var fileName = context.File.Name;
                var extension = Path.GetExtension(fileName);
                if (string.Equals(fileName, "index.html", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(extension, ".html", StringComparison.OrdinalIgnoreCase))
                {
                    context.Context.Response.ContentType = "text/html; charset=utf-8";
                    context.Context.Response.Headers.Append("Content-Type", "text/html; charset=utf-8");
                }
            }
        });

        // Response Compression - deve vir antes de outros middlewares que escrevem resposta
        app.UseResponseCompression();

        // CORS
        app.UseCors("Default");

        // IMPORTANTE: No ASP.NET Core, middlewares são executados na ordem de registro (FIFO)
        // Security Headers - deve ser um dos primeiros para aplicar em todas as respostas
        app.UseMiddleware<SecurityHeadersMiddleware>();

        // Correlation ID middleware - registrado primeiro, executa PRIMEIRO
        // Isso garante que o correlation ID esteja disponível quando o RequestLoggingMiddleware executar
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Request logging middleware - registrado depois, executa DEPOIS do CorrelationIdMiddleware
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Rate Limiting
        app.UseRateLimiter();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Health Checks
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception?.Message,
                        duration = entry.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception?.Message,
                        duration = entry.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        exception = entry.Value.Exception?.Message,
                        duration = entry.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                });
                await context.Response.WriteAsync(result);
            }
        });

        // Endpoint de teste removido - não deve estar em produção

        app.MapGet("/liveness", () => Results.Ok(new { status = "ok" }))
            .AllowAnonymous()
            .ExcludeFromDescription();

        app.MapControllers();

        return app;
    }
}
