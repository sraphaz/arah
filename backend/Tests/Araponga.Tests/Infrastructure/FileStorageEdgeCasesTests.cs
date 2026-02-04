using Araponga.Modules.Moderation.Domain.Evidence;
using Araponga.Infrastructure.FileStorage;
using System.Text;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for File Storage services (LocalFileStorage and S3FileStorage),
/// focusing on large files, Unicode filenames, and error handling.
/// </summary>
public class FileStorageEdgeCasesTests
{
    [Fact]
    public async Task LocalFileStorage_SaveAsync_WithUnicodeFileName_HandlesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            var content = new MemoryStream(Encoding.UTF8.GetBytes("Test content"));
            var fileName = "café-naïve-文字-🎉.txt";

            var storageKey = await storage.SaveAsync(
                content,
                fileName,
                "text/plain",
                CancellationToken.None);

            Assert.NotNull(storageKey);
            Assert.Contains(fileName, storageKey);

            // Verificar que o arquivo foi salvo
            var fullPath = Path.Combine(tempPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(fullPath));
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_SaveAsync_WithNullFileName_UsesDefaultName()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            var content = new MemoryStream(Encoding.UTF8.GetBytes("Test content"));

            var storageKey = await storage.SaveAsync(
                content,
                null!,
                "text/plain",
                CancellationToken.None);

            Assert.NotNull(storageKey);
            Assert.Contains("upload.bin", storageKey);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_SaveAsync_WithEmptyFileName_UsesDefaultName()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            var content = new MemoryStream(Encoding.UTF8.GetBytes("Test content"));

            var storageKey = await storage.SaveAsync(
                content,
                "   ",
                "text/plain",
                CancellationToken.None);

            Assert.NotNull(storageKey);
            Assert.Contains("upload.bin", storageKey);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_SaveAsync_WithLargeFile_HandlesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            // Criar arquivo de 10MB
            var largeContent = new MemoryStream();
            var buffer = new byte[1024 * 1024]; // 1MB
            Array.Fill(buffer, (byte)'A');
            for (int i = 0; i < 10; i++)
            {
                await largeContent.WriteAsync(buffer, 0, buffer.Length);
            }
            largeContent.Position = 0;

            var storageKey = await storage.SaveAsync(
                largeContent,
                "large-file.bin",
                "application/octet-stream",
                CancellationToken.None);

            Assert.NotNull(storageKey);

            // Verificar que o arquivo foi salvo
            var fullPath = Path.Combine(tempPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(fullPath));
            var fileInfo = new FileInfo(fullPath);
            Assert.True(fileInfo.Length >= 10 * 1024 * 1024);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_SaveAsync_WithEmptyStream_HandlesCorrectly()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            var emptyContent = new MemoryStream();

            var storageKey = await storage.SaveAsync(
                emptyContent,
                "empty.txt",
                "text/plain",
                CancellationToken.None);

            Assert.NotNull(storageKey);

            // Verificar que o arquivo foi salvo (mesmo que vazio)
            var fullPath = Path.Combine(tempPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(fullPath));
            var fileInfo = new FileInfo(fullPath);
            Assert.Equal(0, fileInfo.Length);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_OpenReadAsync_WithNonExistentKey_ThrowsFileNotFoundException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                await storage.OpenReadAsync("non-existent-key", CancellationToken.None);
            });
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_OpenReadAsync_WithEmptyStorageKey_ThrowsArgumentException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await storage.OpenReadAsync("", CancellationToken.None);
            });
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_OpenReadAsync_WithNullStorageKey_ThrowsArgumentException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await storage.OpenReadAsync(null!, CancellationToken.None);
            });
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public async Task LocalFileStorage_SaveAndRead_WithUnicodeContent_PreservesContent()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);

            var originalContent = "Conteúdo com café, naïve, résumé, 文字 e emoji 🎉";
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));

            var storageKey = await storage.SaveAsync(
                contentStream,
                "unicode-content.txt",
                "text/plain; charset=utf-8",
                CancellationToken.None);

            // Ler o arquivo de volta
            using var readStream = await storage.OpenReadAsync(storageKey, CancellationToken.None);
            using var reader = new StreamReader(readStream, Encoding.UTF8);
            var readContent = await reader.ReadToEndAsync();

            Assert.Equal(originalContent, readContent);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public void S3FileStorage_Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            new S3FileStorage(null!);
        });
    }

    [Fact]
    public void S3FileStorage_Constructor_WithEmptyBucket_ThrowsArgumentException()
    {
        var options = new S3StorageOptions
        {
            Bucket = "",
            AccessKey = "test-key",
            SecretKey = "test-secret"
        };

        Assert.Throws<ArgumentException>(() =>
        {
            new S3FileStorage(options);
        });
    }

    [Fact]
    public void S3FileStorage_Constructor_WithEmptyAccessKey_ThrowsArgumentException()
    {
        var options = new S3StorageOptions
        {
            Bucket = "test-bucket",
            AccessKey = "",
            SecretKey = "test-secret"
        };

        Assert.Throws<ArgumentException>(() =>
        {
            new S3FileStorage(options);
        });
    }

    [Fact]
    public void S3FileStorage_Constructor_WithEmptySecretKey_ThrowsArgumentException()
    {
        var options = new S3StorageOptions
        {
            Bucket = "test-bucket",
            AccessKey = "test-key",
            SecretKey = ""
        };

        Assert.Throws<ArgumentException>(() =>
        {
            new S3FileStorage(options);
        });
    }

    [Fact]
    public void LocalFileStorage_Provider_ReturnsLocal()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var storage = new LocalFileStorage(tempPath);
            Assert.Equal(StorageProvider.Local, storage.Provider);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    [Fact]
    public void S3FileStorage_Provider_ReturnsS3()
    {
        var options = new S3StorageOptions
        {
            Bucket = "test-bucket",
            AccessKey = "test-key",
            SecretKey = "test-secret"
        };

        var storage = new S3FileStorage(options);
        Assert.Equal(StorageProvider.S3, storage.Provider);
    }
}
