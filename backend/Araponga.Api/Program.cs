using System.Reflection;
using Araponga.Api.Configuration;
using Araponga.Api.Extensions;
using Araponga.Api.Middleware;
using Araponga.Api.Security;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using FluentValidation;
using FluentValidation.AspNetCore;
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

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/araponga-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// Configuration Validation
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? builder.Configuration["JWT__SIGNINGKEY"];
if (builder.Environment.IsProduction() && (string.IsNullOrWhiteSpace(jwtSigningKey) || jwtSigningKey == "dev-only-change-me"))
{
    throw new InvalidOperationException(
        "JWT SigningKey must be configured via environment variable JWT__SIGNINGKEY in production. " +
        "Never use the default value in production.");
}

// Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<PresencePolicyOptions>(builder.Configuration.GetSection("PresencePolicy"));

// CORS Configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                     (builder.Environment.IsDevelopment() ? new[] { "*" } : Array.Empty<string>());

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
                       .AllowCredentials();
        }
    });
});

// Rate Limiting Configuration
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", limiterOptions =>
    {
        limiterOptions.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 60);
        limiterOptions.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60));
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit", 0);
    });
    
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 60),
                Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60)),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit", 0)
            }));
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
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
    .AddCheck("self", () => HealthCheckResult.Healthy("API is healthy"));

var persistenceProvider = builder.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    // DbContext will be registered in AddInfrastructure
    // Health check for database will be added after infrastructure is registered
}

// Infrastructure (repositories, unit of work, etc.) - must be called before adding database health check
builder.Services.AddInfrastructure(builder.Configuration);

// Add database health check after infrastructure is registered
if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    healthChecksBuilder.AddDbContextCheck<ArapongaDbContext>(
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "postgres" });
}

// Controllers with FluentValidation
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Memory cache
builder.Services.AddMemoryCache();

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

var app = builder.Build();

// Serilog Request Logging
app.UseSerilogRequestLogging();

// HTTPS Redirection (Production only)
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
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

        var includeDetails = app.Environment.IsDevelopment();
        var statusCode = exception is ArgumentException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Title = "Unexpected error",
            Status = statusCode,
            Detail = includeDetails ? exception?.Message : "An unexpected error occurred.",
            Instance = feature?.Path
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["path"] = feature?.Path;

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

// CORS
app.UseCors("Default");

// IMPORTANTE: No ASP.NET Core, middlewares são executados na ordem de registro (FIFO)
// O primeiro UseMiddleware registrado é o primeiro a ser executado
// Correlation ID middleware - registrado primeiro, executa PRIMEIRO
// Isso garante que o correlation ID esteja disponível quando o RequestLoggingMiddleware executar
app.UseMiddleware<CorrelationIdMiddleware>();

// Request logging middleware - registrado depois, executa DEPOIS do CorrelationIdMiddleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Rate Limiting
app.UseRateLimiter();

app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("self"),
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
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("self"),
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

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/__throw", (HttpContext _) => throw new InvalidOperationException("boom"));
}

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
