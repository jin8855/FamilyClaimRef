namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductDocumentListItemViewModel
{
    public ProductDocumentListItemViewModel(string displayTitle)
        : this(displayTitle, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }

    public ProductDocumentListItemViewModel(
        string displayTitle,
        string purpose,
        string documentType,
        string target,
        string ocrStatus,
        string referenceDate)
    {
        DisplayTitle = RequireText(displayTitle, nameof(displayTitle));
        Purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
        DocumentType = documentType ?? throw new ArgumentNullException(nameof(documentType));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        OcrStatus = ocrStatus ?? throw new ArgumentNullException(nameof(ocrStatus));
        ReferenceDate = referenceDate ?? throw new ArgumentNullException(nameof(referenceDate));
    }

    public string DisplayTitle { get; }

    public string Purpose { get; }

    public string DocumentType { get; }

    public string Target { get; }

    public string OcrStatus { get; }

    public string ReferenceDate { get; }

    private static string RequireText(string value, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(value, parameterName);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must not be empty or whitespace.", parameterName);
        }

        return value;
    }
}
