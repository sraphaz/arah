using System.Reflection;
using Arah.Api.Contracts.Common;
using Arah.Api.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Arah.Api.Extensions;

/// <summary>
/// Documentação OpenAPI/Swagger: geração dos documentos v1 (API) e v2 (jornadas) e
/// exposição da UI apenas em Development.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddArahSwagger(this IServiceCollection services)
    {
        // Swagger / OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Arah API",
                Version = "v1",
                Description =
                    "Arah é uma plataforma comunitária orientada a território. " +
                    "Esta API expõe recursos de território, feed comunitário, entidades do mapa e saúde do território. " +
                    "O objetivo do MVP é viabilizar cadastro, visualização e curadoria comunitária com segurança e governança mínima.",
                Contact = new OpenApiContact
                {
                    Name = "Arah (maintainers)",
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                }
            });
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Arah API - Jornadas (v2)",
                Version = "v2",
                Description =
                    "Endpoints de jornadas (onboarding, feed, eventos). " +
                    "O frontend pode consumir via aplicação BFF (Arah.Api.Bff), que é uma aplicação separada que encaminha para esta API. " +
                    "Base path: /api/v2/journeys.",
                Contact = new OpenApiContact
                {
                    Name = "Arah (maintainers)",
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

            // Support multipart/form-data + IFormFile endpoints in Swagger (evita 500 ao gerar swagger.json)
            c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
            c.MapType<FileUploadRequest>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["File"] = new OpenApiSchema { Type = "string", Format = "binary", Description = "Arquivo enviado" }
                },
                Required = new HashSet<string> { "File" }
            });
            c.OperationFilter<FormFileOperationFilter>();

            // Incluir no doc v1 apenas rotas api/v1; no v2 apenas api/v2 (evita duplicatas e 500 na geração)
            c.DocInclusionPredicate((docName, api) =>
            {
                var path = api.RelativePath ?? "";
                return docName == "v1" ? path.StartsWith("api/v1/", StringComparison.OrdinalIgnoreCase)
                     : docName == "v2" ? path.StartsWith("api/v2/", StringComparison.OrdinalIgnoreCase)
                     : true;
            });

            // OperationIds únicos (path + method) para evitar conflito entre documentos
            c.CustomOperationIds(api =>
            {
                var path = (api.RelativePath ?? "")
                    .Replace("/", "_", StringComparison.Ordinal)
                    .Replace("-", "_", StringComparison.Ordinal)
                    .Trim('_');
                var method = api.HttpMethod ?? "Get";
                return $"{path}_{method}";
            });
        });

        return services;
    }

    /// <summary>
    /// Expõe o Swagger e a Swagger UI apenas em Development (padrão).
    /// </summary>
    public static WebApplication UseArahSwagger(this WebApplication app)
    {
        // Swagger only in Development (padrão)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Arah API Docs";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arah API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Arah API - Jornadas v2");
                c.DisplayRequestDuration();
            });
        }

        return app;
    }
}
