using System.Reflection;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application services
var persistenceProvider = builder.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<ArapongaDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

    builder.Services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
    builder.Services.AddScoped<IUserRepository, PostgresUserRepository>();
    builder.Services.AddScoped<ITerritoryMembershipRepository, PostgresTerritoryMembershipRepository>();
    builder.Services.AddScoped<IUserTerritoryRepository, PostgresUserTerritoryRepository>();
    builder.Services.AddScoped<IFeedRepository, PostgresFeedRepository>();
    builder.Services.AddScoped<IMapRepository, PostgresMapRepository>();
    builder.Services.AddScoped<IActiveTerritoryStore, PostgresActiveTerritoryStore>();
    builder.Services.AddScoped<IHealthAlertRepository, PostgresHealthAlertRepository>();
    builder.Services.AddScoped<IFeatureFlagService, PostgresFeatureFlagService>();
    builder.Services.AddScoped<IAuditLogger, PostgresAuditLogger>();
}
else
{
    builder.Services.AddSingleton<InMemoryDataStore>();
    builder.Services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
    builder.Services.AddSingleton<IUserTerritoryRepository, InMemoryUserTerritoryRepository>();
    builder.Services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();
    builder.Services.AddSingleton<IMapRepository, InMemoryMapRepository>();
    builder.Services.AddSingleton<IActiveTerritoryStore, InMemoryActiveTerritoryStore>();
    builder.Services.AddSingleton<IHealthAlertRepository, InMemoryHealthAlertRepository>();
    builder.Services.AddSingleton<IFeatureFlagService, InMemoryFeatureFlagService>();
    builder.Services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();
}

builder.Services.AddSingleton<ITokenService, SimpleTokenService>();

builder.Services.AddSingleton<TerritoryService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MembershipService>();
builder.Services.AddSingleton<AccessEvaluator>();
builder.Services.AddSingleton<FeedService>();
builder.Services.AddSingleton<MapService>();
builder.Services.AddSingleton<ActiveTerritoryService>();
builder.Services.AddSingleton<HealthService>();
builder.Services.AddSingleton<CurrentUserAccessor>();

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

// Importante: como você está rodando só em HTTP, removemos o redirect p/ HTTPS para não gerar warning.
// app.UseHttpsRedirection();

app.UseAuthorization();

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

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/__throw", (HttpContext _) => throw new InvalidOperationException("boom"));
}

app.MapControllers();

app.Run();

public partial class Program;
