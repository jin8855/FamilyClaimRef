using System.IO;
using System.Text.Json;
using FamilyClaimRef.App.Models.Storage;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonFileStore<T>
{
    private const int DefaultSchemaVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string filePath;
    private readonly int expectedSchemaVersion;
    private readonly bool preserveBackupOnReplace;

    public JsonFileStore(
        string rootPath,
        string fileName,
        int expectedSchemaVersion = DefaultSchemaVersion,
        bool preserveBackupOnReplace = false)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path is required.", nameof(rootPath));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        this.filePath = Path.Combine(rootPath, fileName);
        this.expectedSchemaVersion = expectedSchemaVersion;
        this.preserveBackupOnReplace = preserveBackupOnReplace;
    }

    public async Task<JsonFileEnvelope<T>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return new JsonFileEnvelope<T>
            {
                SchemaVersion = expectedSchemaVersion,
                SavedAt = DateTimeOffset.MinValue
            };
        }

        JsonFileEnvelope<T>? envelope;
        try
        {
            await using var stream = File.OpenRead(filePath);
            envelope = await JsonSerializer.DeserializeAsync<JsonFileEnvelope<T>>(
                stream,
                JsonOptions,
                cancellationToken);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException("JSON storage file is invalid.", exception);
        }

        if (envelope is null)
        {
            throw new InvalidOperationException("JSON storage file is empty or invalid.");
        }

        ValidateEnvelope(envelope);

        return envelope;
    }

    public async Task SaveAsync(IReadOnlyList<T> items, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);

        var envelope = new JsonFileEnvelope<T>
        {
            SchemaVersion = expectedSchemaVersion,
            SavedAt = DateTimeOffset.UtcNow,
            Items = items.ToList()
        };

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tempFilePath = $"{filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = new FileStream(
                tempFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, envelope, JsonOptions, cancellationToken);
                if (preserveBackupOnReplace)
                {
                    await stream.FlushAsync(cancellationToken);
                    stream.Flush(flushToDisk: true);
                }
            }

            if (preserveBackupOnReplace)
            {
                await LoadAndValidatePathAsync(tempFilePath, cancellationToken);
            }

            if (File.Exists(filePath) && preserveBackupOnReplace)
            {
                File.Replace(
                    tempFilePath,
                    filePath,
                    $"{filePath}.bak",
                    ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempFilePath, filePath, overwrite: true);
            }
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    private async Task LoadAndValidatePathAsync(
        string path,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var envelope = await JsonSerializer.DeserializeAsync<JsonFileEnvelope<T>>(
            stream,
            JsonOptions,
            cancellationToken);
        if (envelope is null)
        {
            throw new InvalidOperationException("JSON storage verification failed.");
        }

        ValidateEnvelope(envelope);
    }

    private void ValidateEnvelope(JsonFileEnvelope<T> envelope)
    {
        if (envelope.SchemaVersion != expectedSchemaVersion)
        {
            throw new InvalidOperationException("JSON storage schema version is not supported.");
        }

        if (envelope.SavedAt == default)
        {
            throw new InvalidOperationException("JSON storage savedAt is required.");
        }

        if (envelope.Items is null)
        {
            throw new InvalidOperationException("JSON storage items are required.");
        }
    }
}
