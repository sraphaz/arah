using System.Reflection;
using Araponga.Api.Configuration;
using Araponga.Api.Security;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application services
var persistenceProvider = builder.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
var applyMigrations = builder.Configuration.GetValue<bool>("Persistence:ApplyMigrations");
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<PresencePolicyOptions>(builder.Configuration.GetSection("PresencePolicy"));

if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<ArapongaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

    builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ArapongaDbContext>());
    builder.Services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
    builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
    builder.Services.AddScoped<ITerritoryMembershipRepository, PostgresTerritoryMembershipRepository>();
    builder.Services.AddScoped<IUserTerritoryRepository, PostgresUserTerritoryRepository>();
    builder.Services.AddScoped<IFeedRepository, PostgresFeedRepository>();
    builder.Services.AddScoped<IMapRepository, PostgresMapRepository>();
    builder.Services.AddScoped<IMapEntityRelationRepository, PostgresMapEntityRelationRepository>();
    builder.Services.AddScoped<IPostGeoAnchorRepository, PostgresPostGeoAnchorRepository>();
    builder.Services.AddScoped<IActiveTerritoryStore, PostgresActiveTerritoryStore>();
    builder.Services.AddScoped<IHealthAlertRepository, PostgresHealthAlertRepository>();
    builder.Services.AddScoped<IFeatureFlagService, PostgresFeatureFlagService>();
    builder.Services.AddScoped<IAuditLogger, PostgresAuditLogger>();
    builder.Services.AddScoped<IReportRepository, PostgresReportRepository>();
    builder.Services.AddScoped<IUserBlockRepository, PostgresUserBlockRepository>();
    builder.Services.AddScoped<ISanctionRepository, PostgresSanctionRepository>();
    builder.Services.AddScoped<IOutbox, PostgresOutbox>();
    builder.Services.AddScoped<INotificationInboxRepository, PostgresNotificationInboxRepository>();
    builder.Services.AddHostedService<OutboxDispatcherWorker>();
}
else
{
    builder.Services.AddSingleton<InMemoryDataStore>();
    builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
    builder.Services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
    builder.Services.AddSingleton<IUserTerritoryRepository, InMemoryUserTerritoryRepository>();
    builder.Services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();
    builder.Services.AddSingleton<IMapRepository, InMemoryMapRepository>();
    builder.Services.AddSingleton<IMapEntityRelationRepository, InMemoryMapEntityRelationRepository>();
    builder.Services.AddSingleton<IPostGeoAnchorRepository, InMemoryPostGeoAnchorRepository>();
    builder.Services.AddSingleton<IActiveTerritoryStore, InMemoryActiveTerritoryStore>();
    builder.Services.AddSingleton<IHealthAlertRepository, InMemoryHealthAlertRepository>();
    builder.Services.AddSingleton<IFeatureFlagService, InMemoryFeatureFlagService>();
    builder.Services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();
    builder.Services.AddSingleton<IReportRepository, InMemoryReportRepository>();
    builder.Services.AddSingleton<IUserBlockRepository, InMemoryUserBlockRepository>();
    builder.Services.AddSingleton<ISanctionRepository, InMemorySanctionRepository>();
    builder.Services.AddSingleton<IOutbox, InMemoryOutbox>();
    builder.Services.AddSingleton<INotificationInboxRepository, InMemoryNotificationInboxRepository>();
}

builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddScoped<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<ReportCreatedEvent>, ReportCreatedNotificationHandler>();

builder.Services.AddScoped<TerritoryService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MembershipService>();
builder.Services.AddScoped<AccessEvaluator>();
builder.Services.AddScoped<FeedService>();
builder.Services.AddScoped<MapService>();
builder.Services.AddScoped<ActiveTerritoryService>();
builder.Services.AddScoped<HealthService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserBlockService>();
builder.Services.AddScoped<FeatureFlagService>();
builder.Services.AddScoped<CurrentUserAccessor>();

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

    c.DocInclusionPredicate((_, __) => true);
});

var app = builder.Build();

if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase) && applyMigrations)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ArapongaDbContext>();
    dbContext.Database.Migrate();
}

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
        await context.Response.WriteAsJsonAsync(problem);
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
        if (string.Equals(context.File.Name, "index.html", StringComparison.OrdinalIgnoreCase))
        {
            context.Context.Response.ContentType = "text/html; charset=utf-8";
        }
    }
});

// Importante: como você está rodando só em HTTP, removemos o redirect p/ HTTPS para não gerar warning.
// app.UseHttpsRedirection();

app.UseAuthorization();

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/__throw", (HttpContext _) => throw new InvalidOperationException("boom"));
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "Araponga.Api" }))
    .AllowAnonymous()
    .ExcludeFromDescription();

app.MapControllers();

app.Run();

public partial class Program;
