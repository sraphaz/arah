namespace Arah.Core.Application;

using System.Security.Cryptography;

internal static class InstanceKeyPairGenerator
{
    public static (string PublicKeyPem, string PrivateKeyPem) CreateRsa2048()
    {
        using var rsa = RSA.Create(2048);
        return (rsa.ExportSubjectPublicKeyInfoPem(), rsa.ExportPkcs8PrivateKeyPem());
    }
}
