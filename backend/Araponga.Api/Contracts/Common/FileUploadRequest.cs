using Microsoft.AspNetCore.Http;

namespace Araponga.Api.Contracts.Common;

public sealed record FileUploadRequest(IFormFile File);

