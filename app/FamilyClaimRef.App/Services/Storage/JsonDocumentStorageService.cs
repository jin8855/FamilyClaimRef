using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services;

namespace FamilyClaimRef.App.Services.Storage;

public sealed class JsonDocumentStorageService : IDocumentStorageService
{
    private const string DocumentsFileName = "documents.json";
    private const string PolicyDocumentsFileName = "policy-documents.json";
    private const string ClaimDocumentsFileName = "claim-documents.json";
    private const string ClaimScope = "claim";
    private const string PolicyScope = "policy";

    private readonly JsonFileStore<DocumentRecord> documentStore;
    private readonly JsonFileStore<PolicyDocumentRecord> policyDocumentStore;
    private readonly JsonFileStore<ClaimDocumentRecord> claimDocumentStore;

    public JsonDocumentStorageService(string metadataRootPath)
    {
        if (string.IsNullOrWhiteSpace(metadataRootPath))
        {
            throw new ArgumentException("Metadata root path is required.", nameof(metadataRootPath));
        }

        documentStore = new JsonFileStore<DocumentRecord>(metadataRootPath, DocumentsFileName);
        policyDocumentStore = new JsonFileStore<PolicyDocumentRecord>(metadataRootPath, PolicyDocumentsFileName);
        claimDocumentStore = new JsonFileStore<ClaimDocumentRecord>(metadataRootPath, ClaimDocumentsFileName);
    }

    public async Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(CancellationToken cancellationToken = default)
    {
        var envelope = await documentStore.LoadAsync(cancellationToken);

        return envelope.Items;
    }

    public async Task<DocumentRecord?> GetDocumentByIdAsync(string documentId, CancellationToken cancellationToken = default)
    {
        var normalizedDocumentId = NormalizeRequiredValue(documentId, nameof(documentId));
        var documents = await GetDocumentsAsync(cancellationToken);

        return documents.FirstOrDefault(document => document.Id == normalizedDocumentId);
    }

    public async Task<DocumentRecord> AddDocumentAsync(DocumentDraft draft, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var gate8Metadata = NormalizeGate8Metadata(draft);
        var documents = (await GetDocumentsAsync(cancellationToken)).ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new DocumentRecord(
            CreateId("doc"),
            NormalizeRequiredValue(draft.PhysicalFileName, nameof(draft.PhysicalFileName)),
            NormalizeRequiredValue(draft.DisplayTitle, nameof(draft.DisplayTitle)),
            NormalizeRequiredValue(draft.Extension, nameof(draft.Extension)).ToLowerInvariant(),
            NormalizeRequiredValue(draft.RelativePath, nameof(draft.RelativePath)),
            timestamp,
            timestamp,
            null,
            gate8Metadata?.OriginalDisplayFileName,
            gate8Metadata?.ValidatedFileType,
            gate8Metadata?.ByteLength,
            gate8Metadata?.Sha256,
            gate8Metadata?.ReferenceDate,
            gate8Metadata?.DocumentType);

        EnsureUniqueId(documents.Select(document => document.Id), record.Id);

        documents.Add(record);
        await documentStore.SaveAsync(documents, cancellationToken);

        return record;
    }

    public async Task DisableDocumentAsync(
        string documentId,
        DateTimeOffset disabledAt,
        CancellationToken cancellationToken = default)
    {
        var normalizedDocumentId = NormalizeRequiredValue(documentId, nameof(documentId));
        var documents = (await GetDocumentsAsync(cancellationToken)).ToList();
        var documentIndex = documents.FindIndex(document => document.Id == normalizedDocumentId);
        if (documentIndex < 0)
        {
            throw new InvalidOperationException("Document was not found.");
        }

        documents[documentIndex] = documents[documentIndex] with
        {
            UpdatedAt = disabledAt,
            DisabledAt = disabledAt
        };

        await documentStore.SaveAsync(documents, cancellationToken);
    }

    public async Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
        string policyId,
        CancellationToken cancellationToken = default)
    {
        var normalizedPolicyId = NormalizeRequiredValue(policyId, nameof(policyId));
        var envelope = await policyDocumentStore.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(policyDocument => policyDocument.PolicyId == normalizedPolicyId)
            .ToList();
    }

    public async Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
        PolicyDocumentDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedPolicyId = NormalizeRequiredValue(draft.PolicyId, nameof(draft.PolicyId));
        var normalizedDocumentId = NormalizeRequiredValue(draft.DocumentId, nameof(draft.DocumentId));
        var normalizedDocumentType = NormalizeDocumentType(PolicyScope, draft.DocumentType, nameof(draft.DocumentType));

        await EnsureActiveDocumentExistsAsync(normalizedDocumentId, cancellationToken);

        var policyDocuments = (await policyDocumentStore.LoadAsync(cancellationToken)).Items;
        var records = policyDocuments.ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new PolicyDocumentRecord(
            CreateId("pdoc"),
            normalizedPolicyId,
            normalizedDocumentId,
            normalizedDocumentType,
            timestamp,
            timestamp,
            null);

        EnsureUniqueId(records.Select(policyDocument => policyDocument.Id), record.Id);

        records.Add(record);
        await policyDocumentStore.SaveAsync(records, cancellationToken);

        return record;
    }

    public async Task DisablePolicyDocumentAsync(
        string policyDocumentId,
        DateTimeOffset disabledAt,
        CancellationToken cancellationToken = default)
    {
        var normalizedPolicyDocumentId = NormalizeRequiredValue(policyDocumentId, nameof(policyDocumentId));
        var records = (await policyDocumentStore.LoadAsync(cancellationToken)).Items.ToList();
        var recordIndex = records.FindIndex(policyDocument => policyDocument.Id == normalizedPolicyDocumentId);
        if (recordIndex < 0)
        {
            throw new InvalidOperationException("Policy document was not found.");
        }

        records[recordIndex] = records[recordIndex] with
        {
            UpdatedAt = disabledAt,
            DisabledAt = disabledAt
        };

        await policyDocumentStore.SaveAsync(records, cancellationToken);
    }

    public async Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(
        string claimId,
        CancellationToken cancellationToken = default)
    {
        var normalizedClaimId = NormalizeRequiredValue(claimId, nameof(claimId));
        var envelope = await claimDocumentStore.LoadAsync(cancellationToken);

        return envelope.Items
            .Where(claimDocument => claimDocument.ClaimId == normalizedClaimId)
            .ToList();
    }

    public async Task<ClaimDocumentRecord> AddClaimDocumentAsync(
        ClaimDocumentDraft draft,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var normalizedClaimId = NormalizeRequiredValue(draft.ClaimId, nameof(draft.ClaimId));
        var normalizedDocumentId = NormalizeRequiredValue(draft.DocumentId, nameof(draft.DocumentId));
        var normalizedDocumentType = NormalizeDocumentType(ClaimScope, draft.DocumentType, nameof(draft.DocumentType));

        await EnsureActiveDocumentExistsAsync(normalizedDocumentId, cancellationToken);

        var claimDocuments = (await claimDocumentStore.LoadAsync(cancellationToken)).Items;
        var records = claimDocuments.ToList();
        var timestamp = DateTimeOffset.UtcNow;
        var record = new ClaimDocumentRecord(
            CreateId("cdoc"),
            normalizedClaimId,
            normalizedDocumentId,
            normalizedDocumentType,
            timestamp,
            timestamp,
            null);

        EnsureUniqueId(records.Select(claimDocument => claimDocument.Id), record.Id);

        records.Add(record);
        await claimDocumentStore.SaveAsync(records, cancellationToken);

        return record;
    }

    public async Task DisableClaimDocumentAsync(
        string claimDocumentId,
        DateTimeOffset disabledAt,
        CancellationToken cancellationToken = default)
    {
        var normalizedClaimDocumentId = NormalizeRequiredValue(claimDocumentId, nameof(claimDocumentId));
        var records = (await claimDocumentStore.LoadAsync(cancellationToken)).Items.ToList();
        var recordIndex = records.FindIndex(claimDocument => claimDocument.Id == normalizedClaimDocumentId);
        if (recordIndex < 0)
        {
            throw new InvalidOperationException("Claim document was not found.");
        }

        records[recordIndex] = records[recordIndex] with
        {
            UpdatedAt = disabledAt,
            DisabledAt = disabledAt
        };

        await claimDocumentStore.SaveAsync(records, cancellationToken);
    }

    public async Task<bool> ActiveTargetDocumentWithSha256ExistsAsync(
        string targetKind,
        string targetId,
        string sha256,
        CancellationToken cancellationToken = default)
    {
        var normalizedTargetKind = NormalizeRequiredValue(targetKind, nameof(targetKind)).ToLowerInvariant();
        var normalizedTargetId = NormalizeRequiredValue(targetId, nameof(targetId));
        var normalizedSha256 = NormalizeSha256(sha256, nameof(sha256));

        IReadOnlyList<string> activeDocumentIds = normalizedTargetKind switch
        {
            PolicyScope => (await GetPolicyDocumentsAsync(normalizedTargetId, cancellationToken))
                .Where(link => link.DisabledAt is null)
                .Select(link => link.DocumentId)
                .ToList(),
            ClaimScope => (await GetClaimDocumentsAsync(normalizedTargetId, cancellationToken))
                .Where(link => link.DisabledAt is null)
                .Select(link => link.DocumentId)
                .ToList(),
            _ => throw new ArgumentException("Target kind must be policy or claim.", nameof(targetKind))
        };

        var documents = await GetDocumentsAsync(cancellationToken);
        return documents.Any(document =>
            activeDocumentIds.Contains(document.Id, StringComparer.Ordinal)
            && document.DisabledAt is null
            && string.Equals(document.Sha256, normalizedSha256, StringComparison.Ordinal));
    }

    private static string CreateId(string prefix)
    {
        return $"{prefix}_{Guid.NewGuid():N}";
    }

    private static string NormalizeRequiredValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }

    private static string NormalizeDocumentType(string documentScope, string documentType, string parameterName)
    {
        var normalizedDocumentType = NormalizeRequiredValue(documentType, parameterName).ToLowerInvariant();
        var allowedDocumentTypes = FileNamePolicyService.GetAllowedDocumentTypes(documentScope);
        if (!allowedDocumentTypes.Contains(normalizedDocumentType, StringComparer.Ordinal))
        {
            throw new ArgumentException("Document type is not allowed for the document scope.", parameterName);
        }

        return normalizedDocumentType;
    }

    private static Gate8DocumentMetadata? NormalizeGate8Metadata(DocumentDraft draft)
    {
        var values = new object?[]
        {
            draft.OriginalDisplayFileName,
            draft.ValidatedFileType,
            draft.ByteLength,
            draft.Sha256,
            draft.ReferenceDate,
            draft.DocumentType
        };
        var suppliedCount = values.Count(value => value is not null);
        if (suppliedCount == 0)
        {
            return null;
        }

        if (suppliedCount != values.Length)
        {
            throw new ArgumentException("Gate8 Document metadata must be supplied as a complete set.", nameof(draft));
        }

        var originalDisplayFileName = NormalizeRequiredValue(
            draft.OriginalDisplayFileName!,
            nameof(draft.OriginalDisplayFileName));
        if (originalDisplayFileName.Length > 255)
        {
            throw new ArgumentException("Original display file name is too long.", nameof(draft));
        }

        var normalizedExtension = NormalizeRequiredValue(draft.Extension, nameof(draft.Extension))
            .ToLowerInvariant();
        var validatedFileType = NormalizeRequiredValue(
            draft.ValidatedFileType!,
            nameof(draft.ValidatedFileType))
            .ToUpperInvariant();
        var expectedValidatedType = normalizedExtension switch
        {
            "pdf" => "PDF",
            "jpg" or "jpeg" => "JPEG",
            "png" => "PNG",
            _ => throw new ArgumentException("Document extension is not supported.", nameof(draft))
        };
        if (!string.Equals(validatedFileType, expectedValidatedType, StringComparison.Ordinal))
        {
            throw new ArgumentException("Validated file type does not match the extension.", nameof(draft));
        }

        if (draft.ByteLength is null or <= 0 or > DocumentFileValidationService.MaximumFileSizeBytes)
        {
            throw new ArgumentException("Document byte length is invalid.", nameof(draft));
        }

        if (draft.ReferenceDate is null || draft.ReferenceDate == default)
        {
            throw new ArgumentException("Reference date is required.", nameof(draft));
        }

        return new Gate8DocumentMetadata(
            originalDisplayFileName,
            validatedFileType,
            draft.ByteLength.Value,
            NormalizeSha256(draft.Sha256!, nameof(draft.Sha256)),
            draft.ReferenceDate.Value,
            NormalizeRequiredValue(draft.DocumentType!, nameof(draft.DocumentType)).ToLowerInvariant());
    }

    private static string NormalizeSha256(string sha256, string parameterName)
    {
        var normalized = NormalizeRequiredValue(sha256, parameterName).ToLowerInvariant();
        if (normalized.Length != 64 || normalized.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new ArgumentException("SHA-256 must be a 64-character hexadecimal value.", parameterName);
        }

        return normalized;
    }

    private static void EnsureUniqueId(IEnumerable<string> ids, string id)
    {
        if (ids.Contains(id, StringComparer.Ordinal))
        {
            throw new InvalidOperationException("Generated id already exists.");
        }
    }

    private async Task EnsureActiveDocumentExistsAsync(string documentId, CancellationToken cancellationToken)
    {
        var document = await GetDocumentByIdAsync(documentId, cancellationToken);
        if (document is null)
        {
            throw new InvalidOperationException("Referenced document was not found.");
        }

        if (document.DisabledAt is not null)
        {
            throw new InvalidOperationException("Referenced document is disabled.");
        }
    }

    private sealed record Gate8DocumentMetadata(
        string OriginalDisplayFileName,
        string ValidatedFileType,
        long ByteLength,
        string Sha256,
        DateOnly ReferenceDate,
        string DocumentType);
}
