using System.Globalization;

namespace FamilyClaimRef.App.Services;

public static class FileNamePolicyService
{
    private static readonly HashSet<string> ClaimDocumentTypes = new(StringComparer.Ordinal)
    {
        "receipt",
        "diagnosis",
        "medicine",
        "visit",
        "admission",
        "surgery",
        "etc"
    };

    private static readonly HashSet<string> PolicyDocumentTypes = new(StringComparer.Ordinal)
    {
        "policy",
        "terms",
        "contract",
        "capture",
        "etc"
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.Ordinal)
    {
        "pdf",
        "jpg",
        "jpeg",
        "png"
    };

    public static IReadOnlyCollection<string> GetAllowedDocumentTypes(string documentScope)
    {
        var normalizedScope = NormalizeDocumentScope(documentScope);
        var allowedTypes = normalizedScope == "claim" ? ClaimDocumentTypes : PolicyDocumentTypes;

        return allowedTypes.ToArray();
    }

    public static string CreatePhysicalFileName(
        string documentScope,
        string id,
        DateOnly date,
        string documentType,
        string extension,
        int? duplicateIndex = null)
    {
        var normalizedScope = NormalizeDocumentScope(documentScope);
        var normalizedId = NormalizeId(id);
        var normalizedDocumentType = NormalizeDocumentType(normalizedScope, documentType);
        var normalizedExtension = NormalizeExtension(extension);

        if (duplicateIndex is <= 0 or > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(duplicateIndex), "Duplicate index must be between 1 and 999.");
        }

        var prefix = normalizedScope switch
        {
            "claim" => "claim-",
            "policy" => "policy-",
            _ => throw new ArgumentException("Document scope must be claim or policy.", nameof(documentScope))
        };

        var duplicateSuffix = duplicateIndex.HasValue
            ? FormattableString.Invariant($"_{duplicateIndex.Value:000}")
            : string.Empty;

        return string.Create(
            CultureInfo.InvariantCulture,
            $"{prefix}{normalizedId}_{date:yyyyMMdd}_{normalizedDocumentType}{duplicateSuffix}.{normalizedExtension}");
    }

    private static string NormalizeDocumentScope(string documentScope)
    {
        if (string.IsNullOrWhiteSpace(documentScope))
        {
            throw new ArgumentException("Document scope is required.", nameof(documentScope));
        }

        var normalizedScope = documentScope.Trim().ToLowerInvariant();
        if (normalizedScope is not ("claim" or "policy"))
        {
            throw new ArgumentException("Document scope must be claim or policy.", nameof(documentScope));
        }

        return normalizedScope;
    }

    private static string NormalizeId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        var normalizedId = id.Trim();
        if (!IsSafeToken(normalizedId))
        {
            throw new ArgumentException("Id can contain only ASCII letters, digits, hyphen, or underscore.", nameof(id));
        }

        return normalizedId;
    }

    private static string NormalizeDocumentType(string documentScope, string documentType)
    {
        if (string.IsNullOrWhiteSpace(documentType))
        {
            throw new ArgumentException("Document type is required.", nameof(documentType));
        }

        var normalizedDocumentType = documentType.Trim().ToLowerInvariant();
        var allowedTypes = documentScope == "claim" ? ClaimDocumentTypes : PolicyDocumentTypes;

        if (!allowedTypes.Contains(normalizedDocumentType))
        {
            throw new ArgumentException("Document type is not allowed for the document scope.", nameof(documentType));
        }

        return normalizedDocumentType;
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("Extension is required.", nameof(extension));
        }

        var normalizedExtension = extension.Trim();
        if (normalizedExtension[0] == '.')
        {
            normalizedExtension = normalizedExtension[1..];
        }

        if (normalizedExtension.Length == 0 || !IsSafeExtension(normalizedExtension))
        {
            throw new ArgumentException("Extension can contain only ASCII letters or digits.", nameof(extension));
        }

        normalizedExtension = normalizedExtension.ToLowerInvariant();
        if (!AllowedExtensions.Contains(normalizedExtension))
        {
            throw new ArgumentException("Extension is not allowed.", nameof(extension));
        }

        return normalizedExtension;
    }

    private static bool IsSafeToken(string value)
    {
        foreach (var character in value)
        {
            if (!IsAsciiLetterOrDigit(character) && character is not ('-' or '_'))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSafeExtension(string value)
    {
        foreach (var character in value)
        {
            if (!IsAsciiLetterOrDigit(character))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAsciiLetterOrDigit(char character)
    {
        return character is >= 'a' and <= 'z'
            or >= 'A' and <= 'Z'
            or >= '0' and <= '9';
    }
}
