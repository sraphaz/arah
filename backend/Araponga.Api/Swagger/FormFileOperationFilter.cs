using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Araponga.Api.Swagger;

/// <summary>
/// Enables Swashbuckle to describe endpoints that use [FromForm] IFormFile (multipart/form-data).
/// </summary>
public sealed class FormFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Source == BindingSource.Form)
            .ToList();

        if (formParameters.Count == 0)
        {
            return;
        }

        // Remove form parameters from "parameters" (OpenAPI treats them as requestBody)
        var formParameterNames = new HashSet<string>(formParameters.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);
        operation.Parameters = operation.Parameters
            .Where(p => !formParameterNames.Contains(p.Name))
            .ToList();

        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
            Required = new HashSet<string>()
        };

        foreach (var p in formParameters)
        {
            if (string.IsNullOrWhiteSpace(p.Name))
            {
                continue;
            }

            var isFile = p.Type == typeof(IFormFile);
            schema.Properties[p.Name] = isFile
                ? new OpenApiSchema { Type = "string", Format = "binary" }
                : context.SchemaGenerator.GenerateSchema(p.Type, context.SchemaRepository);

            if (p.IsRequired)
            {
                schema.Required.Add(p.Name);
            }
        }

        operation.RequestBody ??= new OpenApiRequestBody();
        operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
        {
            Schema = schema
        };
    }
}

