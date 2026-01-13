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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<PresencePolicyOptions>(builder.Configuration.GetSection("PresencePolicy"));

// Controllers with FluentValidation
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Infrastructure (repositories, unit of work, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

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

    c.DocInclusionPredicate((_, __) => true);
});

var app = builder.Build();

var persistenceProvider = builder.Configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
var applyMigrations = builder.Configuration.GetValue<bool>("Persistence:ApplyMigrations");

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

// IMPORTANTE: No ASP.NET Core, middlewares são executados na ordem inversa de registro (LIFO)
// O último UseMiddleware registrado é o primeiro a ser executado na pipeline
// Portanto, registramos na ordem inversa da execução desejada

// Correlation ID middleware - registrado por último, executa PRIMEIRO
// Isso garante que o correlation ID esteja disponível quando o RequestLoggingMiddleware executar
app.UseMiddleware<CorrelationIdMiddleware>();

// Request logging middleware - registrado primeiro, executa DEPOIS do CorrelationIdMiddleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/__throw", (HttpContext _) => throw new InvalidOperationException("boom"));
}

app.MapGet("/liveness", () => Results.Ok(new { status = "ok" }))
    .AllowAnonymous()
    .ExcludeFromDescription();

app.MapGet("/readiness", () =>
    Results.Ok(new { status = "ready" }))
    // TODO: add dependency checks
    .AllowAnonymous()
    .ExcludeFromDescription();

app.MapControllers();

app.Run();

public partial class Program;
