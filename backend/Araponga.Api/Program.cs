using System.Reflection;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Application services
builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserTerritoryRepository, InMemoryUserTerritoryRepository>();
builder.Services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();
builder.Services.AddSingleton<IMapRepository, InMemoryMapRepository>();
builder.Services.AddSingleton<IActiveTerritoryStore, InMemoryActiveTerritoryStore>();
builder.Services.AddSingleton<ITokenService, SimpleTokenService>();

builder.Services.AddSingleton<TerritoryService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MembershipService>();
builder.Services.AddSingleton<AccessEvaluator>();
builder.Services.AddSingleton<FeedService>();
builder.Services.AddSingleton<MapService>();
builder.Services.AddSingleton<ActiveTerritoryService>();
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

app.MapControllers();

app.Run();

public partial class Program;
