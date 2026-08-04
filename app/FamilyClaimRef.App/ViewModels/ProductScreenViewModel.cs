using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductScreenViewModel : INotifyPropertyChanged
{
    private string? presentationMessage;

    public ProductScreenViewModel(
        string id,
        string wireframeNumber,
        string title,
        string subtitle,
        string breadcrumb,
        IEnumerable<ProductScreenSectionViewModel> sections,
        IEnumerable<ProductScreenGroupViewModel> groups,
        string tableTitle,
        IEnumerable<string> tableColumns,
        string emptyMessage,
        IEnumerable<ProductScreenFieldViewModel> fields,
        IEnumerable<ProductScreenActionViewModel> actions,
        IEnumerable<ProductScreenCommandViewModel> commands,
        int groupColumnCount,
        int fieldColumnCount,
        int claimStepNumber,
        bool isManagementHub)
    {
        Id = RequireText(id, nameof(id));
        WireframeNumber = RequireText(wireframeNumber, nameof(wireframeNumber));
        Title = RequireText(title, nameof(title));
        Subtitle = RequireText(subtitle, nameof(subtitle));
        Breadcrumb = RequireText(breadcrumb, nameof(breadcrumb));
        Sections = Array.AsReadOnly(sections?.ToArray() ?? throw new ArgumentNullException(nameof(sections)));
        Groups = Array.AsReadOnly(groups?.ToArray() ?? throw new ArgumentNullException(nameof(groups)));
        TableTitle = RequireText(tableTitle, nameof(tableTitle));
        TableColumns = Array.AsReadOnly(
            tableColumns?.Select(column => RequireText(column, nameof(tableColumns))).ToArray()
            ?? throw new ArgumentNullException(nameof(tableColumns)));
        EmptyMessage = RequireText(emptyMessage, nameof(emptyMessage));
        Fields = Array.AsReadOnly(fields?.ToArray() ?? throw new ArgumentNullException(nameof(fields)));
        Actions = Array.AsReadOnly(actions?.ToArray() ?? throw new ArgumentNullException(nameof(actions)));
        Commands = Array.AsReadOnly(commands?.ToArray() ?? throw new ArgumentNullException(nameof(commands)));
        GroupColumnCount = Math.Max(1, groupColumnCount);
        FieldColumnCount = Math.Max(1, fieldColumnCount);
        ClaimStepNumber = Math.Max(0, claimStepNumber);
        IsManagementHub = isManagementHub;
        DisplayLabel = $"{WireframeNumber} · {Title}";
        AutomationId = $"ProductScreen_{WireframeNumber}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id { get; }

    public string WireframeNumber { get; }

    public string Title { get; }

    public string Subtitle { get; }

    public string Breadcrumb { get; }

    public string DisplayLabel { get; }

    public string AutomationId { get; }

    public ReadOnlyCollection<ProductScreenSectionViewModel> Sections { get; }

    public ReadOnlyCollection<ProductScreenGroupViewModel> Groups { get; }

    public string TableTitle { get; }

    public ReadOnlyCollection<string> TableColumns { get; }

    public string EmptyMessage { get; }

    public ReadOnlyCollection<ProductScreenFieldViewModel> Fields { get; }

    public ReadOnlyCollection<ProductScreenActionViewModel> Actions { get; }

    public ReadOnlyCollection<ProductScreenCommandViewModel> Commands { get; }

    public int GroupColumnCount { get; }

    public int FieldColumnCount { get; }

    public int TableColumnCount => TableColumns.Count;

    public int ClaimStepNumber { get; }

    public bool IsManagementHub { get; }

    public bool HasFields => Fields.Count > 0;

    public bool HasActions => Actions.Count > 0;

    public bool HasCommands => Commands.Count > 0;

    public bool HasDeferredCommands => Commands.Any(command => !command.IsEnabled);

    public bool HasGroups => Groups.Count > 0;

    public bool ShowStandardGroups => HasGroups && !IsManagementHub;

    public bool ShowClaimFlow => ClaimStepNumber > 0;

    public bool HasLegacySections => !HasGroups;

    public bool HasTable => TableColumns.Count > 0;

    public string? PresentationMessage
    {
        get => presentationMessage;
        set
        {
            if (string.Equals(presentationMessage, value, StringComparison.Ordinal))
            {
                return;
            }

            presentationMessage = value;
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(PresentationMessage)));
        }
    }

    private static string RequireText(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value;
    }
}

public sealed record ProductScreenSectionViewModel(string Title, string Body);

public sealed record ProductScreenGroupViewModel(
    string Title,
    string Body,
    IReadOnlyList<string> FieldLabels);

public sealed record ProductScreenActionViewModel(
    string Label,
    string RouteId,
    string AutomationId);

public sealed record ProductScreenCommandViewModel(
    string Label,
    string? RouteId,
    string AutomationId,
    bool IsEnabled);

public sealed class ProductScreenFieldViewModel : INotifyPropertyChanged
{
    private string? value;

    public ProductScreenFieldViewModel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Label = label;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Label { get; }

    public string? Value
    {
        get => value;
        set
        {
            if (string.Equals(this.value, value, StringComparison.Ordinal))
            {
                return;
            }

            this.value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
