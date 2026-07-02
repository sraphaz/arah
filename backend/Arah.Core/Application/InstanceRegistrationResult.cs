namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed record InstanceRegistrationResult(CoreInstance Instance, string PrivateKeyPem, string InstanceAuthToken);
