using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Araponga.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string IssueToken(Guid userId)
    {
        var now = DateTimeOffset.UtcNow;
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["sub"] = userId.ToString(),
            ["iss"] = _options.Issuer,
            ["aud"] = _options.Audience,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = now.AddMinutes(_options.ExpirationMinutes).ToUnixTimeSeconds()
        };

        var headerSegment = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var payloadSegment = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var signature = ComputeSignature($"{headerSegment}.{payloadSegment}");

        return $"{headerSegment}.{payloadSegment}.{signature}";
    }

    public Guid? ParseToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        var data = $"{parts[0]}.{parts[1]}";
        if (!TimingSafeEquals(ComputeSignature(data), parts[2]))
        {
            return null;
        }

        try
        {
            var payloadJson = Base64UrlDecode(parts[1]);
            using var document = JsonDocument.Parse(payloadJson);
            if (!TryGetString(document, "iss", out var issuer) ||
                !TryGetString(document, "aud", out var audience) ||
                !TryGetString(document, "sub", out var subject))
            {
                return null;
            }

            if (!string.Equals(issuer, _options.Issuer, StringComparison.Ordinal) ||
                !string.Equals(audience, _options.Audience, StringComparison.Ordinal))
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (TryGetLong(document, "nbf", out var notBefore) && now < notBefore)
            {
                return null;
            }

            if (TryGetLong(document, "exp", out var expiresAt) && now >= expiresAt)
            {
                return null;
            }

            return Guid.TryParse(subject, out var userId) ? userId : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool TryGetString(JsonDocument document, string key, out string value)
    {
        if (document.RootElement.TryGetProperty(key, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(value);
        }

        value = string.Empty;
        return false;
    }

    private static bool TryGetLong(JsonDocument document, string key, out long value)
    {
        if (document.RootElement.TryGetProperty(key, out var property) &&
            property.ValueKind == JsonValueKind.Number &&
            property.TryGetInt64(out value))
        {
            return true;
        }

        value = 0;
        return false;
    }

    private string ComputeSignature(string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        padded += (padded.Length % 4) switch
        {
            0 => string.Empty,
            2 => "==",
            3 => "=",
            _ => throw new FormatException("Invalid base64url payload.")
        };

        return Convert.FromBase64String(padded);
    }

    private static bool TimingSafeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);

        if (leftBytes.Length != rightBytes.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < leftBytes.Length; i += 1)
        {
            result |= leftBytes[i] ^ rightBytes[i];
        }

        return result == 0;
    }
}
