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

    public JsonFileStore(string rootPath, string fileName, int expectedSchemaVersion = DefaultSchemaVersion)
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
            }

            File.Move(tempFilePath, filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
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
