namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductDocumentListItemViewModel
{
    public ProductDocumentListItemViewModel(string displayTitle)
    {
        ArgumentNullException.ThrowIfNull(displayTitle);

        if (string.IsNullOrWhiteSpace(displayTitle))
        {
            throw new ArgumentException("Display title must not be empty or whitespace.", nameof(displayTitle));
        }

        DisplayTitle = displayTitle;
    }

    public string DisplayTitle { get; }
}
