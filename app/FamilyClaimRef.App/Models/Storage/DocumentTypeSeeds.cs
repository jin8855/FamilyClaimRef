namespace FamilyClaimRef.App.Models.Storage;

public static class DocumentTypeSeeds
{
    public const string ClaimScope = "claim";
    public const string PolicyScope = "policy";

    private static readonly DocumentTypeSeed[] ClaimSeedItems =
    [
        new("receipt", "영수증", ClaimScope, 10, null),
        new("diagnosis", "진단서", ClaimScope, 20, null),
        new("medicine", "약제비 서류", ClaimScope, 30, null),
        new("visit", "통원 확인 서류", ClaimScope, 40, null),
        new("admission", "입퇴원 확인 서류", ClaimScope, 50, null),
        new("surgery", "수술 확인 서류", ClaimScope, 60, null),
        new("etc", "기타", ClaimScope, 999, null)
    ];

    private static readonly DocumentTypeSeed[] PolicySeedItems =
    [
        new("policy", "보험증권", PolicyScope, 10, null),
        new("terms", "약관", PolicyScope, 20, null),
        new("contract", "계약서", PolicyScope, 30, null),
        new("capture", "캡처", PolicyScope, 40, null),
        new("etc", "기타", PolicyScope, 999, null)
    ];

    private static readonly DocumentTypeSeed[] AllSeedItems =
    [
        .. ClaimSeedItems,
        .. PolicySeedItems
    ];

    public static IReadOnlyList<DocumentTypeSeed> Claim { get; } = Array.AsReadOnly(ClaimSeedItems);

    public static IReadOnlyList<DocumentTypeSeed> Policy { get; } = Array.AsReadOnly(PolicySeedItems);

    public static IReadOnlyList<DocumentTypeSeed> All { get; } = Array.AsReadOnly(AllSeedItems);
}
