using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

public static class RuntimeEnvironmentCollectionName
{
    public const string Value = "RuntimeEnvironment";
}

[CollectionDefinition(RuntimeEnvironmentCollectionName.Value)]
public sealed class RuntimeEnvironmentCollection;
