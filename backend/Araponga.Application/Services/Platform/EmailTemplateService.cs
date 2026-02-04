using System.Reflection;
using System.Text;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para renderização de templates de email usando substituição de placeholders.
/// </summary>
public sealed class EmailTemplateService : IEmailTemplateService
{
    private readonly ILogger<EmailTemplateService> _logger;
    private readonly string _templatesPath;
    private readonly Dictionary<string, string> _templateCache = new();

    public EmailTemplateService(
        ILogger<EmailTemplateService> logger,
        string? templatesPath = null)
    {
        _logger = logger;
        _templatesPath = templatesPath ?? Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            "Email");
    }

    public async Task<string> RenderTemplateAsync(
        string templateName,
        object data,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        var template = await LoadTemplateAsync(templateName, cancellationToken);
        var layout = await LoadLayoutAsync(cancellationToken);

        // Renderiza o template com os dados
        var renderedContent = RenderContent(template, data);

        // Aplica o layout
        var finalHtml = layout.Replace("{{CONTENT}}", renderedContent);

        // Renderiza o layout com os dados também (para variáveis globais como BaseUrl)
        finalHtml = RenderContent(finalHtml, data);

        return finalHtml;
    }

    private async Task<string> LoadTemplateAsync(string templateName, CancellationToken cancellationToken)
    {
        // Remove extensão se presente
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(templateName);
        var templateFile = Path.Combine(_templatesPath, $"{nameWithoutExtension}.html");

        if (!File.Exists(templateFile))
        {
            _logger.LogError("Template file not found: {TemplateFile}", templateFile);
            throw new FileNotFoundException($"Template file not found: {templateFile}");
        }

        if (_templateCache.TryGetValue(templateFile, out var cached))
        {
            return cached;
        }

        var content = await File.ReadAllTextAsync(templateFile, Encoding.UTF8, cancellationToken);
        _templateCache[templateFile] = content;
        return content;
    }

    private async Task<string> LoadLayoutAsync(CancellationToken cancellationToken)
    {
        var layoutFile = Path.Combine(_templatesPath, "_layout.html");

        if (!File.Exists(layoutFile))
        {
            _logger.LogWarning("Layout file not found: {LayoutFile}, using minimal layout", layoutFile);
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>{{CONTENT}}</body></html>";
        }

        if (_templateCache.TryGetValue(layoutFile, out var cached))
        {
            return cached;
        }

        var content = await File.ReadAllTextAsync(layoutFile, Encoding.UTF8, cancellationToken);
        _templateCache[layoutFile] = content;
        return content;
    }

    private string RenderContent(string template, object data)
    {
        var result = template;

        // Processa condicionais {{#if PropertyName}}...{{/if}}
        result = ProcessConditionals(result, data);

        // Processa loops {{#each Items}}...{{/each}}
        result = ProcessLoops(result, data);

        // Substitui propriedades do objeto data
        var properties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var value = property.GetValue(data);
            var placeholder = $"{{{{{property.Name}}}}}";
            var stringValue = FormatValue(value);
            result = result.Replace(placeholder, stringValue, StringComparison.OrdinalIgnoreCase);
        }

        // Substitui propriedades aninhadas (ex: {{Items.0.Name}})
        result = ReplaceNestedProperties(result, data);

        return result;
    }

    private string ProcessConditionals(string template, object data)
    {
        var result = template;
        var pattern = @"\{\{#if\s+([A-Za-z_][A-Za-z0-9_]*)\}\}(.*?)\{\{/if\}\}";
        var matches = System.Text.RegularExpressions.Regex.Matches(result, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);

        foreach (System.Text.RegularExpressions.Match match in matches.Cast<System.Text.RegularExpressions.Match>().Reverse())
        {
            var propertyName = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            var value = GetPropertyValue(data, propertyName);
            var shouldInclude = value != null && !string.IsNullOrWhiteSpace(FormatValue(value));

            if (shouldInclude)
            {
                result = result.Replace(match.Value, content);
            }
            else
            {
                result = result.Replace(match.Value, string.Empty);
            }
        }

        return result;
    }

    private string ProcessLoops(string template, object data)
    {
        var result = template;
        var pattern = @"\{\{#each\s+([A-Za-z_][A-Za-z0-9_]*)\}\}(.*?)\{\{/each\}\}";
        var matches = System.Text.RegularExpressions.Regex.Matches(result, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);

        foreach (System.Text.RegularExpressions.Match match in matches.Cast<System.Text.RegularExpressions.Match>().Reverse())
        {
            var propertyName = match.Groups[1].Value;
            var itemTemplate = match.Groups[2].Value;

            var value = GetPropertyValue(data, propertyName);
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                var items = enumerable.Cast<object>().ToList();
                var renderedItems = new StringBuilder();

                foreach (var item in items)
                {
                    var itemContent = itemTemplate;
                    var itemProperties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in itemProperties)
                    {
                        var propValue = prop.GetValue(item);
                        var placeholder = $"{{{{{prop.Name}}}}}";
                        var stringValue = FormatValue(propValue);
                        itemContent = itemContent.Replace(placeholder, stringValue);
                    }
                    renderedItems.Append(itemContent);
                }

                result = result.Replace(match.Value, renderedItems.ToString());
            }
            else
            {
                result = result.Replace(match.Value, string.Empty);
            }
        }

        return result;
    }

    private object? GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return property?.GetValue(obj);
    }

    private string ReplaceNestedProperties(string template, object data)
    {
        var result = template;

        // Procura por placeholders aninhados como {{Items.0.Name}}
        var matches = System.Text.RegularExpressions.Regex.Matches(
            template,
            @"\{\{([A-Za-z_][A-Za-z0-9_]*(\.[0-9]+)?(\.[A-Za-z_][A-Za-z0-9_]*)*)\}\}");

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var path = match.Groups[1].Value;
            var value = GetNestedValue(data, path);
            var stringValue = FormatValue(value);
            result = result.Replace(match.Value, stringValue);
        }

        return result;
    }

    private object? GetNestedValue(object obj, string path)
    {
        var parts = path.Split('.');
        var current = obj;

        foreach (var part in parts)
        {
            if (current == null)
                return null;

            // Se for um índice de array/lista (ex: "0", "1")
            if (int.TryParse(part, out var index))
            {
                if (current is System.Collections.IEnumerable enumerable)
                {
                    var list = enumerable.Cast<object?>().ToList();
                    if (index >= 0 && index < list.Count)
                    {
                        current = list[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var property = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                    return null;

                current = property.GetValue(current);
            }
        }

        return current;
    }

    private string FormatValue(object? value)
    {
        if (value == null)
            return string.Empty;

        if (value is DateTime dateTime)
            return dateTime.ToString("dd/MM/yyyy HH:mm");

        if (value is decimal decimalValue)
            return decimalValue.ToString("C", new System.Globalization.CultureInfo("pt-BR"));

        if (value is IReadOnlyList<string> stringList)
            return string.Join(", ", stringList);

        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            var items = enumerable.Cast<object>().Select(FormatValue);
            return string.Join(", ", items);
        }

        return value.ToString() ?? string.Empty;
    }
}
