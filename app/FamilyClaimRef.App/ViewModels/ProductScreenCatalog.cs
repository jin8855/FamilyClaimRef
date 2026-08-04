using FamilyClaimRef.App.Services.Localization;

namespace FamilyClaimRef.App.ViewModels;

public static class ProductScreenCatalog
{
    private static readonly IReadOnlyDictionary<string, string[]> RouteActions =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [ProductScreenRoutes.HomeDashboard] =
            [
                ProductScreenRoutes.ClaimCase,
                ProductScreenRoutes.PolicyList,
                ProductScreenRoutes.HistoryView,
                ProductScreenRoutes.ManageHome
            ],
            [ProductScreenRoutes.FamilyMembers] =
            [
                ProductScreenRoutes.FamilyRegister,
                ProductScreenRoutes.ManageHome
            ],
            [ProductScreenRoutes.PolicyList] =
            [
                ProductScreenRoutes.PolicyDetail,
                ProductScreenRoutes.HomeDashboard
            ],
            [ProductScreenRoutes.PolicyDetail] =
            [
                ProductScreenRoutes.PolicyDocumentRegister,
                ProductScreenRoutes.PolicyList
            ],
            [ProductScreenRoutes.DocumentBox] =
            [
                ProductScreenRoutes.PolicyDocumentRegister,
                ProductScreenRoutes.ClaimDocumentRegister,
                ProductScreenRoutes.OcrReview,
                ProductScreenRoutes.ManageHome
            ],
            [ProductScreenRoutes.OcrReview] =
            [
                ProductScreenRoutes.ClaimCase,
                ProductScreenRoutes.ClaimReferenceResult,
                ProductScreenRoutes.DocumentBox
            ],
            [ProductScreenRoutes.ClaimCase] =
            [
                ProductScreenRoutes.ClaimDocumentRegister,
                ProductScreenRoutes.OcrReview,
                ProductScreenRoutes.ClaimReferenceResult
            ],
            [ProductScreenRoutes.ClaimSubmission] =
            [
                ProductScreenRoutes.ClaimReferenceResult,
                ProductScreenRoutes.ClaimComplete,
                ProductScreenRoutes.HistoryView
            ],
            [ProductScreenRoutes.ClaimReferenceResult] =
            [
                ProductScreenRoutes.OcrReview,
                ProductScreenRoutes.ClaimSubmission,
                ProductScreenRoutes.HistoryView
            ],
            [ProductScreenRoutes.HistoryView] =
            [
                ProductScreenRoutes.HistoryDetail,
                ProductScreenRoutes.HomeDashboard
            ],
            [ProductScreenRoutes.PolicyManage] =
            [
                ProductScreenRoutes.PolicyRegister,
                ProductScreenRoutes.PolicyDocumentRegister,
                ProductScreenRoutes.ManageHome
            ],
            [ProductScreenRoutes.PolicyRegister] =
            [
                ProductScreenRoutes.PolicyDocumentRegister,
                ProductScreenRoutes.PolicyManage
            ],
            [ProductScreenRoutes.FamilyRegister] =
            [
                ProductScreenRoutes.FamilyMembers
            ],
            [ProductScreenRoutes.ClaimComplete] =
            [
                ProductScreenRoutes.ClaimSubmission,
                ProductScreenRoutes.ClaimCase,
                ProductScreenRoutes.HistoryView,
                ProductScreenRoutes.HomeDashboard
            ],
            [ProductScreenRoutes.ManageHome] =
            [
                ProductScreenRoutes.FamilyMembers,
                ProductScreenRoutes.PolicyManage,
                ProductScreenRoutes.DocumentBox,
                ProductScreenRoutes.CategoryManage
            ],
            [ProductScreenRoutes.CategoryManage] =
            [
                ProductScreenRoutes.CategoryRegister,
                ProductScreenRoutes.CategoryItemRegister,
                ProductScreenRoutes.ManageHome
            ],
            [ProductScreenRoutes.PolicyDocumentRegister] =
            [
                ProductScreenRoutes.OcrReview,
                ProductScreenRoutes.PolicyRegister
            ],
            [ProductScreenRoutes.ClaimDocumentRegister] =
            [
                ProductScreenRoutes.OcrReview,
                ProductScreenRoutes.ClaimCase
            ],
            [ProductScreenRoutes.CategoryRegister] =
            [
                ProductScreenRoutes.CategoryManage
            ],
            [ProductScreenRoutes.CategoryItemRegister] =
            [
                ProductScreenRoutes.CategoryManage
            ],
            [ProductScreenRoutes.HistoryDetail] =
            [
                ProductScreenRoutes.HistoryView
            ]
        };

    private static readonly HashSet<string> FormRoutes =
    [
        ProductScreenRoutes.PolicyList,
        ProductScreenRoutes.OcrReview,
        ProductScreenRoutes.ClaimCase,
        ProductScreenRoutes.ClaimReferenceResult,
        ProductScreenRoutes.HistoryView,
        ProductScreenRoutes.PolicyRegister,
        ProductScreenRoutes.FamilyRegister,
        ProductScreenRoutes.CategoryRegister,
        ProductScreenRoutes.CategoryItemRegister,
        ProductScreenRoutes.HistoryDetail
    ];

    private static readonly HashSet<string> StructuredRoutes =
    [
        ProductScreenRoutes.FamilyMembers,
        ProductScreenRoutes.PolicyList,
        ProductScreenRoutes.PolicyDetail,
        ProductScreenRoutes.OcrReview,
        ProductScreenRoutes.ClaimSubmission,
        ProductScreenRoutes.ClaimReferenceResult,
        ProductScreenRoutes.HistoryView,
        ProductScreenRoutes.PolicyRegister,
        ProductScreenRoutes.FamilyRegister,
        ProductScreenRoutes.ClaimComplete,
        ProductScreenRoutes.ManageHome,
        ProductScreenRoutes.CategoryManage,
        ProductScreenRoutes.CategoryRegister,
        ProductScreenRoutes.CategoryItemRegister,
        ProductScreenRoutes.HistoryDetail
    ];

    private static readonly HashSet<string> TableRoutes =
    [
        ProductScreenRoutes.FamilyMembers,
        ProductScreenRoutes.PolicyList,
        ProductScreenRoutes.HistoryView,
        ProductScreenRoutes.PolicyRegister,
        ProductScreenRoutes.CategoryManage
    ];

    private static readonly IReadOnlyDictionary<string, int> ClaimSteps =
        new Dictionary<string, int>(StringComparer.Ordinal)
        {
            [ProductScreenRoutes.ClaimCase] = 1,
            [ProductScreenRoutes.OcrReview] = 2,
            [ProductScreenRoutes.ClaimReferenceResult] = 3,
            [ProductScreenRoutes.ClaimSubmission] = 4,
            [ProductScreenRoutes.ClaimComplete] = 5
        };

    private static readonly IReadOnlyDictionary<string, ProductScreenCommandSpec[]> CommandSpecs =
        new Dictionary<string, ProductScreenCommandSpec[]>(StringComparer.Ordinal)
        {
            [ProductScreenRoutes.FamilyMembers] =
            [
                new(ProductScreenTextKeys.RegisterAction, ProductScreenRoutes.FamilyRegister, true)
            ],
            [ProductScreenRoutes.PolicyList] =
            [
                new(ProductScreenTextKeys.ViewDetailAction, ProductScreenRoutes.PolicyDetail, true)
            ],
            [ProductScreenRoutes.PolicyDetail] =
            [
                new(ProductScreenTextKeys.ViewDocumentAction, ProductScreenRoutes.PolicyDocumentRegister, true),
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.PolicyList, true)
            ],
            [ProductScreenRoutes.OcrReview] =
            [
                new(ProductScreenTextKeys.EditValueAction, null, false),
                new(ProductScreenTextKeys.ExcludeAction, null, false),
                new(ProductScreenTextKeys.ConfirmAction, null, false),
                new(ProductScreenTextKeys.ViewDocumentAction, ProductScreenRoutes.DocumentBox, true)
            ],
            [ProductScreenRoutes.ClaimSubmission] =
            [
                new(ProductScreenTextKeys.ViewDetailAction, ProductScreenRoutes.HistoryDetail, true)
            ],
            [ProductScreenRoutes.ClaimReferenceResult] =
            [
                new(ProductScreenTextKeys.ConfirmAction, null, false),
                new(ProductScreenTextKeys.ViewDetailAction, ProductScreenRoutes.ClaimSubmission, true)
            ],
            [ProductScreenRoutes.HistoryView] =
            [
                new(ProductScreenTextKeys.ViewDetailAction, ProductScreenRoutes.HistoryDetail, true)
            ],
            [ProductScreenRoutes.PolicyRegister] =
            [
                new(ProductScreenTextKeys.SaveAction, null, false),
                new(ProductScreenTextKeys.HoldAction, null, false),
                new(ProductScreenTextKeys.DeleteAction, null, false),
                new(ProductScreenTextKeys.DisableAction, null, false),
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.PolicyManage, true)
            ],
            [ProductScreenRoutes.FamilyRegister] =
            [
                new(ProductScreenTextKeys.SaveAction, null, true),
                new(ProductScreenTextKeys.DeleteAction, null, false),
                new(ProductScreenTextKeys.DisableAction, null, true),
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.FamilyMembers, true)
            ],
            [ProductScreenRoutes.ClaimComplete] =
            [
                new(ProductScreenTextKeys.ViewDetailAction, ProductScreenRoutes.HistoryView, true)
            ],
            [ProductScreenRoutes.CategoryManage] =
            [
                new(ProductScreenTextKeys.RegisterAction, ProductScreenRoutes.CategoryRegister, true),
                new(ProductScreenTextKeys.RegisterAction, ProductScreenRoutes.CategoryItemRegister, true),
                new(ProductScreenTextKeys.EditValueAction, null, false),
                new(ProductScreenTextKeys.DeleteAction, null, false)
            ],
            [ProductScreenRoutes.CategoryRegister] =
            [
                new(ProductScreenTextKeys.SaveAction, null, false),
                new(ProductScreenTextKeys.DeleteAction, null, false),
                new(ProductScreenTextKeys.DisableAction, null, false),
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.CategoryManage, true)
            ],
            [ProductScreenRoutes.CategoryItemRegister] =
            [
                new(ProductScreenTextKeys.SaveAction, null, false),
                new(ProductScreenTextKeys.DeleteAction, null, false),
                new(ProductScreenTextKeys.DisableAction, null, false),
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.CategoryManage, true)
            ],
            [ProductScreenRoutes.HistoryDetail] =
            [
                new(ProductScreenTextKeys.CloseAction, ProductScreenRoutes.HistoryView, true)
            ]
        };

    public static IReadOnlyList<ProductScreenViewModel> Create(IUiTextProvider uiTextProvider)
    {
        ArgumentNullException.ThrowIfNull(uiTextProvider);

        var titles = ProductScreenRoutes.All.ToDictionary(
            routeId => routeId,
            routeId => uiTextProvider.Get(ProductScreenTextKeys.Title(routeId)),
            StringComparer.Ordinal);

        return ProductScreenRoutes.All
            .Select((routeId, index) => CreateScreen(uiTextProvider, titles, routeId, index + 1))
            .ToArray();
    }

    private static ProductScreenViewModel CreateScreen(
        IUiTextProvider uiTextProvider,
        IReadOnlyDictionary<string, string> titles,
        string routeId,
        int index)
    {
        var sections = new[]
        {
            new ProductScreenSectionViewModel(
                uiTextProvider.Get(ProductScreenTextKeys.PrimarySectionTitle),
                uiTextProvider.Get(ProductScreenTextKeys.Primary(routeId))),
            new ProductScreenSectionViewModel(
                uiTextProvider.Get(ProductScreenTextKeys.SecondarySectionTitle),
                uiTextProvider.Get(ProductScreenTextKeys.Secondary(routeId)))
        };

        var fields = FormRoutes.Contains(routeId)
            ? SplitFields(uiTextProvider.Get(ProductScreenTextKeys.Fields(routeId)))
                .Select(label => new ProductScreenFieldViewModel(label))
                .ToArray()
            : [];

        var groups = StructuredRoutes.Contains(routeId)
            ? CreateGroups(uiTextProvider, routeId)
            : [];

        var tableColumns = TableRoutes.Contains(routeId)
            ? SplitValues(uiTextProvider.Get(ProductScreenTextKeys.Columns(routeId)))
            : [];

        var tableTitle = TableRoutes.Contains(routeId)
            ? uiTextProvider.Get(ProductScreenTextKeys.TableTitle(routeId))
            : uiTextProvider.Get(ProductScreenTextKeys.PrimarySectionTitle);

        var actions = RouteActions[routeId]
            .Select(targetRoute => new ProductScreenActionViewModel(
                titles[targetRoute],
                targetRoute,
                $"ProductRoute_{targetRoute}"));

        var commands = CommandSpecs.TryGetValue(routeId, out var commandSpecs)
            ? commandSpecs.Select((spec, commandIndex) => new ProductScreenCommandViewModel(
                uiTextProvider.Get(spec.LabelKey),
                spec.RouteId,
                $"ProductCommand_{routeId}_{commandIndex + 1}",
                spec.IsEnabled))
            : [];

        var claimStepNumber = ClaimSteps.TryGetValue(routeId, out var stepNumber)
            ? stepNumber
            : 0;

        return new ProductScreenViewModel(
            routeId,
            index.ToString("00", System.Globalization.CultureInfo.InvariantCulture),
            titles[routeId],
            uiTextProvider.Get(ProductScreenTextKeys.Subtitle(routeId)),
            CreateBreadcrumb(titles, routeId),
            sections,
            groups,
            tableTitle,
            tableColumns,
            uiTextProvider.Get(ProductScreenTextKeys.EmptySectionMessage),
            fields,
            actions,
            commands,
            GetGroupColumnCount(routeId, groups.Count),
            fields.Length >= 7 ? 4 : 3,
            claimStepNumber,
            string.Equals(routeId, ProductScreenRoutes.ManageHome, StringComparison.Ordinal));
    }

    private static IEnumerable<string> SplitFields(string value)
    {
        return SplitValues(value);
    }

    private static IReadOnlyList<ProductScreenGroupViewModel> CreateGroups(
        IUiTextProvider uiTextProvider,
        string routeId)
    {
        var titles = SplitValues(uiTextProvider.Get(ProductScreenTextKeys.GroupTitles(routeId)));
        var bodies = SplitValues(uiTextProvider.Get(ProductScreenTextKeys.GroupBodies(routeId)));
        var fieldGroups = SplitGroups(uiTextProvider.Get(ProductScreenTextKeys.GroupFields(routeId)));

        if (titles.Count != bodies.Count || titles.Count != fieldGroups.Count)
        {
            throw new InvalidOperationException(
                $"Product screen group resources are misaligned for route '{routeId}'.");
        }

        return titles
            .Select((title, index) => new ProductScreenGroupViewModel(
                title,
                bodies[index],
                fieldGroups[index]))
            .ToArray();
    }

    private static IReadOnlyList<IReadOnlyList<string>> SplitGroups(string value)
    {
        return value
            .Split("||", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(group => (IReadOnlyList<string>)SplitValues(group))
            .ToArray();
    }

    private static int GetGroupColumnCount(
        string routeId,
        int groupCount)
    {
        if (string.Equals(routeId, ProductScreenRoutes.OcrReview, StringComparison.Ordinal)
            || string.Equals(routeId, ProductScreenRoutes.ManageHome, StringComparison.Ordinal))
        {
            return 4;
        }

        return Math.Clamp(groupCount, 1, 3);
    }

    private static IReadOnlyList<string> SplitValues(string value)
    {
        return value
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string CreateBreadcrumb(
        IReadOnlyDictionary<string, string> titles,
        string routeId)
    {
        if (string.Equals(routeId, ProductScreenRoutes.HomeDashboard, StringComparison.Ordinal))
        {
            return titles[routeId];
        }

        return $"{titles[ProductScreenRoutes.HomeDashboard]}  /  {titles[routeId]}";
    }

    private sealed record ProductScreenCommandSpec(
        string LabelKey,
        string? RouteId,
        bool IsEnabled);
}
