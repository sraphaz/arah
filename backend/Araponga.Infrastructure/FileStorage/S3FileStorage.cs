using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Araponga.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;

namespace Araponga.Infrastructure.FileStorage;

public sealed class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _client;
    private readonly string _bucket;

    public S3FileStorage(S3StorageOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.Bucket))
        {
            throw new ArgumentException("S3 Bucket is required.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.AccessKey) || string.IsNullOrWhiteSpace(options.SecretKey))
        {
            throw new ArgumentException("S3 AccessKey/SecretKey are required.", nameof(options));
        }

        _bucket = options.Bucket.Trim();

        var credentials = new BasicAWSCredentials(options.AccessKey.Trim(), options.SecretKey.Trim());
        var config = new AmazonS3Config
        {
            ForcePathStyle = options.ForcePathStyle,
            RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region ?? "us-east-1")
        };

        if (!string.IsNullOrWhiteSpace(options.ServiceUrl))
        {
            config.ServiceURL = options.ServiceUrl.Trim();
        }

        _client = new AmazonS3Client(credentials, config);
    }

    public StorageProvider Provider => StorageProvider.S3;

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var safeName = string.IsNullOrWhiteSpace(fileName) ? "upload.bin" : Path.GetFileName(fileName);
        var key = $"{DateTime.UtcNow:yyyyMMdd}/{Guid.NewGuid():N}-{safeName}";

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType
        };

        await _client.PutObjectAsync(request, cancellationToken);
        return key;
    }

    public async Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("storageKey is required.", nameof(storageKey));
        }

        var response = await _client.GetObjectAsync(
            new GetObjectRequest
            {
                BucketName = _bucket,
                Key = storageKey
            },
            cancellationToken);

        // Precisamos garantir que o GetObjectResponse seja descartado ao final do streaming.
        return new ResponseDisposingStream(response.ResponseStream, response);
    }

    private sealed class ResponseDisposingStream : Stream
    {
        private readonly Stream _inner;
        private readonly IDisposable _toDispose;

        public ResponseDisposingStream(Stream inner, IDisposable toDispose)
        {
            _inner = inner;
            _toDispose = toDispose;
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;
        public override long Position { get => _inner.Position; set => _inner.Position = value; }

        public override void Flush() => _inner.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _inner.ReadAsync(buffer, offset, count, cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _inner.WriteAsync(buffer, offset, count, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
                _toDispose.Dispose();
            }
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _inner.DisposeAsync();
            _toDispose.Dispose();
            await base.DisposeAsync();
        }
    }
}

