using System.Globalization;
using System.Windows;

namespace FamilyClaimRef.App.Services.Localization;

public sealed class ResourceUiTextProvider : IUiTextProvider
{
    private readonly Func<string, object?> getResource;

    public ResourceUiTextProvider(ResourceDictionary resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        getResource = key => resources.Contains(key) ? resources[key] : null;
    }

    public ResourceUiTextProvider(IReadOnlyDictionary<string, string> resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        getResource = key => resources.TryGetValue(key, out var value) ? value : null;
    }

    public string Get(string key)
    {
        ValidateKey(key);

        var value = getResource(key);
        if (value is null)
        {
            return CreateFallback(key);
        }

        if (value is string text)
        {
            return text;
        }

        throw new InvalidOperationException($"UI text resource '{key}' must be a string value.");
    }

    public string Format(string key, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(key), args);
    }

    private static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("UI text key is required.", nameof(key));
        }
    }

    private static string CreateFallback(string key)
    {
        return $"[[{key}]]";
    }
}
