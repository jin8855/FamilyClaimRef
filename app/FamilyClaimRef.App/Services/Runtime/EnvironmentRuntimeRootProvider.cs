using System.IO;

namespace FamilyClaimRef.App.Services.Runtime;

public sealed class EnvironmentRuntimeRootProvider : IRuntimeRootProvider
{
    public const string OverrideGuardEnvironmentVariableName = "FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE";
    public const string RuntimeRootEnvironmentVariableName = "FAMILYCLAIMREF_RUNTIME_ROOT";

    private const string AppDataFolderName = "FamilyClaimRef";

    private readonly Func<string> getLocalApplicationDataPath;
    private readonly Func<string?> getOverrideGuardValue;
    private readonly Func<string?> getRuntimeRootOverrideValue;

    public EnvironmentRuntimeRootProvider()
        : this(
            () => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            () => Environment.GetEnvironmentVariable(OverrideGuardEnvironmentVariableName),
            () => Environment.GetEnvironmentVariable(RuntimeRootEnvironmentVariableName))
    {
    }

    public EnvironmentRuntimeRootProvider(
        Func<string> getLocalApplicationDataPath,
        Func<string?> getOverrideGuardValue,
        Func<string?> getRuntimeRootOverrideValue)
    {
        this.getLocalApplicationDataPath = getLocalApplicationDataPath
            ?? throw new ArgumentNullException(nameof(getLocalApplicationDataPath));
        this.getOverrideGuardValue = getOverrideGuardValue
            ?? throw new ArgumentNullException(nameof(getOverrideGuardValue));
        this.getRuntimeRootOverrideValue = getRuntimeRootOverrideValue
            ?? throw new ArgumentNullException(nameof(getRuntimeRootOverrideValue));
    }

    public RuntimeRootPaths GetRuntimeRootPaths()
    {
        return RuntimeRootPaths.FromRuntimeRoot(ResolveRuntimeRootPath());
    }

    private string ResolveRuntimeRootPath()
    {
        var guardValue = getOverrideGuardValue();
        if (!string.Equals(guardValue, "1", StringComparison.Ordinal))
        {
            return GetDefaultRuntimeRootPath();
        }

        var overrideValue = getRuntimeRootOverrideValue();
        if (string.IsNullOrWhiteSpace(overrideValue))
        {
            throw new InvalidOperationException(
                $"{RuntimeRootEnvironmentVariableName} is required when {OverrideGuardEnvironmentVariableName}=1.");
        }

        var trimmedOverrideValue = overrideValue.Trim();
        if (!Path.IsPathFullyQualified(trimmedOverrideValue))
        {
            throw new InvalidOperationException(
                $"{RuntimeRootEnvironmentVariableName} must be an absolute path when {OverrideGuardEnvironmentVariableName}=1.");
        }

        return Path.GetFullPath(trimmedOverrideValue);
    }

    private string GetDefaultRuntimeRootPath()
    {
        var localApplicationDataPath = getLocalApplicationDataPath();
        if (string.IsNullOrWhiteSpace(localApplicationDataPath))
        {
            throw new InvalidOperationException("Local application data path is required.");
        }

        return Path.GetFullPath(Path.Combine(localApplicationDataPath, AppDataFolderName));
    }
}
