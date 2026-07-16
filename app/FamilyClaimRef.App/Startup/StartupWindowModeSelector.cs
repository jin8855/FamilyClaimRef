namespace FamilyClaimRef.App.Startup;

public enum StartupWindowMode
{
    MainWindow,
    ProductShellPreview
}

public static class StartupWindowModeSelector
{
    public const string ProductShellPreviewArgument =
        "--product-shell-preview";

    public static StartupWindowMode Select(
        IEnumerable<string>? arguments)
    {
        if (arguments is null)
        {
            return StartupWindowMode.MainWindow;
        }

        foreach (var argument in arguments)
        {
            if (string.Equals(
                    argument,
                    ProductShellPreviewArgument,
                    StringComparison.OrdinalIgnoreCase))
            {
                return StartupWindowMode.ProductShellPreview;
            }
        }

        return StartupWindowMode.MainWindow;
    }
}
