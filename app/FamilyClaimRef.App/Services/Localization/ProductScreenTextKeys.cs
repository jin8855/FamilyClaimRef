namespace FamilyClaimRef.App.Services.Localization;

public static class ProductScreenTextKeys
{
    public const string PrimarySectionTitle = "Ui.Product.Wireframe.Common.PrimarySectionTitle";
    public const string SecondarySectionTitle = "Ui.Product.Wireframe.Common.SecondarySectionTitle";
    public const string FieldsSectionTitle = "Ui.Product.Wireframe.Common.FieldsSectionTitle";
    public const string PresentationOnlyMessage = "Ui.Product.Wireframe.Common.PresentationOnlyMessage";
    public const string EmptySectionMessage = "Ui.Product.Wireframe.Common.EmptySectionMessage";
    public const string EmptyValue = "Ui.Product.Wireframe.Common.EmptyValue";
    public const string SaveAction = "Ui.Product.Wireframe.Common.SaveAction";
    public const string HoldAction = "Ui.Product.Wireframe.Common.HoldAction";
    public const string DeleteAction = "Ui.Product.Wireframe.Common.DeleteAction";
    public const string DisableAction = "Ui.Product.Wireframe.Common.DisableAction";
    public const string CloseAction = "Ui.Product.Wireframe.Common.CloseAction";
    public const string ConfirmAction = "Ui.Product.Wireframe.Common.ConfirmAction";
    public const string ExcludeAction = "Ui.Product.Wireframe.Common.ExcludeAction";
    public const string EditValueAction = "Ui.Product.Wireframe.Common.EditValueAction";
    public const string ViewDocumentAction = "Ui.Product.Wireframe.Common.ViewDocumentAction";
    public const string ViewDetailAction = "Ui.Product.Wireframe.Common.ViewDetailAction";
    public const string RegisterAction = "Ui.Product.Wireframe.Common.RegisterAction";
    public const string DisabledActionMessage = "Ui.Product.Wireframe.Common.DisabledActionMessage";
    public const string ClaimContextInputValue = "Ui.Product.Wireframe.Claim.ContextInputValue";
    public const string ClaimContextConfirmationValue =
        "Ui.Product.Wireframe.Claim.ContextConfirmationValue";
    public const string DocumentManagedPurpose = "Ui.Product.Wireframe.DocumentBox.ManagedPurpose";
    public const string DocumentClaimPurpose = "Ui.Product.Wireframe.DocumentBox.ClaimPurpose";
    public const string DocumentPurposeUnavailable = "Ui.Product.Wireframe.DocumentBox.PurposeUnavailable";
    public const string DocumentTargetUnavailable = "Ui.Product.Wireframe.DocumentBox.TargetUnavailable";
    public const string DocumentOcrUnavailable = "Ui.Product.Wireframe.DocumentBox.OcrUnavailable";

    public static string Title(string routeId) => $"Ui.Product.Wireframe.{routeId}.Title";

    public static string Subtitle(string routeId) => $"Ui.Product.Wireframe.{routeId}.Subtitle";

    public static string Primary(string routeId) => $"Ui.Product.Wireframe.{routeId}.Primary";

    public static string Secondary(string routeId) => $"Ui.Product.Wireframe.{routeId}.Secondary";

    public static string Fields(string routeId) => $"Ui.Product.Wireframe.{routeId}.Fields";

    public static string GroupTitles(string routeId) => $"Ui.Product.Wireframe.{routeId}.GroupTitles";

    public static string GroupBodies(string routeId) => $"Ui.Product.Wireframe.{routeId}.GroupBodies";

    public static string GroupFields(string routeId) => $"Ui.Product.Wireframe.{routeId}.GroupFields";

    public static string Columns(string routeId) => $"Ui.Product.Wireframe.{routeId}.Columns";

    public static string TableTitle(string routeId) => $"Ui.Product.Wireframe.{routeId}.TableTitle";
}
