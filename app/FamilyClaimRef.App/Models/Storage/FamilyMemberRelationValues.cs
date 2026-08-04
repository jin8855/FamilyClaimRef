using System.Collections.ObjectModel;

namespace FamilyClaimRef.App.Models.Storage;

public static class FamilyMemberRelationValues
{
    public const string Self = "본인";
    public const string Husband = "남편";
    public const string Son = "아들";
    public const string Daughter = "딸";
    public const string Father = "아버지";
    public const string Mother = "어머니";
    public const string YoungerSibling = "동생";
    public const string Grandmother = "할머니";
    public const string Grandfather = "할아버지";
    public const string Other = "기타";

    public static ReadOnlyCollection<string> All { get; } =
        Array.AsReadOnly(
        [
            Self,
            Husband,
            Son,
            Daughter,
            Father,
            Mother,
            YoungerSibling,
            Grandmother,
            Grandfather,
            Other
        ]);

    public static bool IsSupported(string value)
    {
        return All.Contains(value, StringComparer.Ordinal);
    }
}
