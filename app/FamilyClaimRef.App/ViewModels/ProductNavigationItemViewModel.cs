namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductNavigationItemViewModel
{
    public ProductNavigationItemViewModel(string id, string displayText)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Navigation item ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(displayText))
        {
            throw new ArgumentException("Navigation item display text is required.", nameof(displayText));
        }

        Id = id;
        DisplayText = displayText;
    }

    public string Id { get; }

    public string DisplayText { get; }
}
