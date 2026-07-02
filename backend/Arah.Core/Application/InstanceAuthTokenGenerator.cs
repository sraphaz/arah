namespace Arah.Core.Application;

using System.Security.Cryptography;

internal static class InstanceAuthTokenGenerator
{
    public static string CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
