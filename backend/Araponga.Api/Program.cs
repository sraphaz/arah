using System.Reflection;
using Araponga.Api.Configuration;
using Araponga.Api.Extensions;
using Araponga.Api.HealthChecks;
using Araponga.Api.Middleware;
using Araponga.Api.Security;
using Araponga.Application.Configuration;
using Araponga.Application.Exceptions;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Araponga.Api.Swagger;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.RateLimiting;
using Prometheus;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Araponga.Application.Metrics;
using Araponga.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration with centralized logging
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
        .Enrich.WithProperty("Application", "Araponga")
        .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown")
        .MinimumLevel.Is(minLevel)
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .WriteTo.Console(
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/araponga-.log",
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

// Configuration Validation - JWT Secret
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? builder.Configuration["JWT__SIGNINGKEY"];
if (string.IsNullOrWhiteSpace(jwtSigningKey))
{
    throw new InvalidOperationException(
        "JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY or appsettings.json. " +
        "Never leave this empty.");
}

if (jwtSigningKey == "dev-only-change-me")
{
    if (builder.Environment.IsProduction())
    {
        throw new InvalidOperationException(
            "JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY in production. " +
            "Never use the default value in production.");
    }
    else if (!builder.Environment.IsEnvironment("Testing"))
    {
        Log.Warning("Using default JWT SigningKey. This should be changed in production.");
    }
}

// Validate secret strength (minimum 32 characters for production)
if (builder.Environment.IsProduction() && jwtSigningKey.Length < 32)
{
    throw new InvalidOperationException(
        $"JWT SigningKey must be at least 32 characters long in production. Current length: {jwtSigningKey.Length}");
}

// Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<PresencePolicyOptions>(builder.Configuration.GetSection("PresencePolicy"));
builder.Services.Configure<Araponga.Api.Configuration.ClientCredentialsOptions>(builder.Configuration.GetSection("ClientCredentials"));
builder.Services.Configure<PasswordResetOptions>(builder.Configuration.GetSection("Auth:PasswordReset"));

// CORS Configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                     (builder.Environment.IsDevelopment() ? new[] { "*" } : Array.Empty<string>());

if (builder.Environment.IsProduction())
{
    if (allowedOrigins == null || allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
    {
        throw new InvalidOperationException(
            "Cors:AllowedOrigins must be configured with specific origins in production. " +
            "Wildcard (*) is not allowed in production.");
    }
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", corsBuilder =>
    {
        if (allowedOrigins.Contains("*"))
        {
            corsBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        }
        else
        {
            corsBuilder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .SetPreflightMaxAge(TimeSpan.FromHours(24));
        }
    });
});

// Rate Limiting Configuration
// OpenTelemetry Configuration
var serviceName = "Araponga";
var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

var otlpEndpoint = builder.Configuration["OpenTelemetry:Otlp:Endpoint"];
var jaegerEndpoint = builder.Configuration["OpenTelemetry:Jaeger:Endpoint"];

var otelBuilder = builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion));

// Tracing
otelBuilder.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("Araponga.*");

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
        if (builder.Environment.IsDevelopment())
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
        .AddMeter("Araponga");

    // Exporters
    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
    {
        metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
    }
    else if (builder.Environment.IsDevelopment())
    {
        metrics.AddConsoleExporter();
    }
});

// Prometheus Metrics - configured via UseMetricServer() in app pipeline

builder.Services.AddRateLimiter(options =>
{
    // Global limiter (IP-based)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // Try to get authenticated user ID first, fallback to IP
        var userId = context.User?.FindFirst("sub")?.Value;
        var partitionKey = userId ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 60),
                Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60)),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit", 0)
            });
    });

    // Auth endpoints - stricter limits (5 req/min)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Feed endpoints - moderate limits (100 req/min)
    options.AddFixedWindowLimiter("feed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Read operations - moderate limits (100 req/min)
    options.AddFixedWindowLimiter("read", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Write operations - stricter limits (30 req/min)
    options.AddFixedWindowLimiter("write", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 5;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        // Add rate limit headers
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.Append("Retry-After", retryAfter.TotalSeconds.ToString());
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "Too Many Requests",
            Status = StatusCodes.Status429TooManyRequests,
            Detail = "Rate limit exceeded. Please try again later.",
            Instance = context.HttpContext.Request.Path
        }, cancellationToken);
    };
});

// Health Checks Configuration
var healthChecksBuilder = builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is healthy"), tags: new[] { "self", "live" })
    .AddCheck<CacheHealthCheck>("cache", tags: new[] { "cache", "ready" })
    .AddCheck<StorageHealthCheck>("storage", tags: new[] { "storage", "ready" })
    .AddCheck<EventBusHealthCheck>("event_bus", tags: new[] { "eventbus", "ready" });

var persistenceProvider = builder.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    // DbContext will be registered in AddInfrastructure
    // Database health check will be added after infrastructure is registered
}

// Infrastructure (repositories, unit of work, etc.) - must be called before adding database health check
builder.Services.AddInfrastructure(builder.Configuration);

// ConnectionPoolMetricsService é registrado em AddInfrastructure quando Postgres

// Add database health check after infrastructure is registered
if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    healthChecksBuilder.AddCheck<DatabaseHealthCheck>(
        "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "ready", "postgres" });
}

// Anti-forgery tokens for CSRF protection
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-Token";
    options.Cookie.Name = "__Host-CSRF";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Authentication - Configurar esquema padrão para Forbid()
// Nota: A autenticação JWT é feita via middleware customizado, mas precisamos de um esquema padrão
// para que ForbidResult funcione corretamente quando retornamos Forbid()
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
    options.DefaultForbidScheme = "Bearer";
})
.AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>(
    "Bearer", _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemAdmin", policy =>
        policy.Requirements.Add(new Araponga.Api.Security.SystemAdminRequirement()));
});
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Araponga.Api.Security.SystemAdminAuthorizationHandler>();

// Response Compression (gzip/brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/xml", "text/plain", "text/css", "application/javascript" });
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

// Controllers with FluentValidation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Otimizar serialização JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = false; // Reduzir tamanho em produção
    });
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Memory cache
builder.Services.AddMemoryCache();

// Redis Cache Configuration (optional, falls back to IMemoryCache if not configured)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
    });
}

// Register distributed cache service with fallback
builder.Services.AddSingleton<Araponga.Application.Interfaces.IDistributedCacheService>(
    serviceProvider =>
    {
        var distributedCache = serviceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
        var memoryCache = serviceProvider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        var logger = serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Araponga.Infrastructure.Caching.RedisCacheService>>();
        return new Araponga.Infrastructure.Caching.RedisCacheService(distributedCache, memoryCache, logger);
    });

// Application services
builder.Services.AddApplicationServices();

// Event handlers
builder.Services.AddEventHandlers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Araponga API",
        Version = "v1",
        Description =
            "Araponga é uma plataforma comunitária orientada a território. " +
            "Esta API expõe recursos de território, feed comunitário, entidades do mapa e saúde do território. " +
            "O objetivo do MVP é viabilizar cadastro, visualização e curadoria comunitária com segurança e governança mínima.",
        Contact = new OpenApiContact
        {
            Name = "Araponga (maintainers)",
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
        }
    });
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Araponga API - Jornadas (v2)",
        Version = "v2",
        Description =
            "Endpoints de jornadas (onboarding, feed, eventos). " +
            "O frontend pode consumir via aplicação BFF (Araponga.Api.Bff), que é uma aplicação separada que encaminha para esta API. " +
            "Base path: /api/v2/journeys.",
        Contact = new OpenApiContact
        {
            Name = "Araponga (maintainers)",
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
        }
    });

    // Tags organizadas no Swagger
    c.TagActionsBy(api =>
    {
        var controller = api.GroupName ?? api.ActionDescriptor.RouteValues["controller"];
        return new[] { controller ?? "General" };
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Support multipart/form-data + IFormFile endpoints in Swagger
    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    c.OperationFilter<FormFileOperationFilter>();

    c.DocInclusionPredicate((_, __) => true);
});

// Configure HSTS (deve ser configurado antes de Build())
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);

    // Only in production
    if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing"))
    {
        options.ExcludedHosts.Clear();
    }
});

var app = builder.Build();

// Aplicar migrações do Postgres quando Persistence:ApplyMigrations = true (ex.: Docker dev)
var applyMigrations = builder.Configuration.GetValue<bool>("Persistence:ApplyMigrations");
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
                var mainDb = scope.ServiceProvider.GetRequiredService<ArapongaDbContext>();
                mainDb.Database.Migrate();
            }
            logger.LogInformation("ArapongaDbContext migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Attempt {Attempt}/{Max} failed to apply migrations (waiting {Delay}s before retry)", attempt, maxRetries, delaySeconds);
            if (attempt == maxRetries)
            {
                logger.LogError(ex, "Failed to apply ArapongaDbContext migrations after {Max} attempts. Ensure Postgres is running and connection string is correct.", maxRetries);
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
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = feature?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        if (exception is not null)
        {
            logger.LogError(exception, "Unhandled exception at {Path}", feature?.Path);
        }

        var includeDetails = app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");
        var statusCode = exception switch
        {
            Araponga.Application.Exceptions.ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            ConflictException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            Araponga.Application.Exceptions.DomainException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Title = "Unexpected error",
            Status = statusCode,
            Detail = includeDetails ? (exception?.Message ?? "An unexpected error occurred.") : "An unexpected error occurred.",
            Instance = feature?.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["path"] = feature?.Path;
        if (includeDetails && exception is not null)
        {
            problem.Extensions["exceptionType"] = exception.GetType().FullName;
            problem.Extensions["stackTrace"] = exception.StackTrace;
            if (exception.InnerException is not null)
            {
                problem.Extensions["innerException"] = exception.InnerException.Message;
            }
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        // Serializar manualmente para garantir que path e traceId sejam propriedades diretas
        var response = new Dictionary<string, object?>
        {
            ["title"] = problem.Title ?? "Unexpected error",
            ["status"] = problem.Status ?? statusCode,
            ["detail"] = problem.Detail,
            ["instance"] = problem.Instance ?? feature?.Path,
            ["traceId"] = context.TraceIdentifier,
            ["path"] = feature?.Path
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});

// Swagger only in Development (padrão)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Araponga API Docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Araponga API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Araponga API - Jornadas v2");
        c.DisplayRequestDuration();
    });
}

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

try
{
    Log.Information("Starting Araponga API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
