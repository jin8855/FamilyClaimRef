namespace FamilyClaimRef.App.ViewModels;

public sealed class InsurancePolicyDocumentHistoryItemViewModel
{
    internal InsurancePolicyDocumentHistoryItemViewModel(
        string documentTypeDisplayName,
        string displayTitle,
        string registeredAt,
        string status,
        bool isCurrent,
        string relativePath)
    {
        DocumentTypeDisplayName = documentTypeDisplayName;
        DisplayTitle = displayTitle;
        RegisteredAt = registeredAt;
        Status = status;
        IsCurrent = isCurrent;
        RelativePath = relativePath;
    }

    public string DocumentTypeDisplayName { get; }

    public string DisplayTitle { get; }

    public string RegisteredAt { get; }

    public string Status { get; }

    public bool IsCurrent { get; }

    internal string RelativePath { get; }
}
