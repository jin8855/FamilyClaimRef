namespace FamilyClaimRef.App.Models.Storage;

public static class InsurancePolicyValues
{
    public const string ContractStatusActive = "유지";
    public const string ContractStatusExpired = "만기";
    public const string ContractStatusPremiumWaived = "보험료 납입면제";
    public const string LegacyContractStatusActive = "사용 중";

    public const string RenewalTypeRenewable = "갱신형";
    public const string RenewalTypeFixed = "비갱신형(고정형)";
    public const string RenewalTypePartiallyRenewable = "일부 갱신형";

    public const string RefundTypeRefundable = "환급형";
    public const string RefundTypeNoSurrenderValue = "해약환급금 미지급형";

    public const string BusinessTypeLife = "생명보험";
    public const string BusinessTypeNonLife = "손해보험";

    public const string ProductCategoryMedicalExpense = "실손보험";
    public const string ProductCategoryDriver = "운전자보험";
    public const string ProductCategoryCancer = "암보험";
    public const string ProductCategoryComprehensive = "종합보험";

    public const string RegistrationSourceDirectInput = "직접 입력";
    public const string RegistrationSourceInsuranceDocument = "보험 문서 등록";

    public static IReadOnlyList<string> ContractStatuses { get; } =
    [
        ContractStatusActive,
        ContractStatusExpired,
        ContractStatusPremiumWaived
    ];

    public static IReadOnlyList<string> RenewalTypes { get; } =
    [
        RenewalTypeRenewable,
        RenewalTypeFixed,
        RenewalTypePartiallyRenewable
    ];

    public static IReadOnlyList<string> RefundTypes { get; } =
    [
        RefundTypeRefundable,
        RefundTypeNoSurrenderValue
    ];

    public static IReadOnlyList<string> BusinessTypes { get; } =
    [
        BusinessTypeLife,
        BusinessTypeNonLife
    ];

    public static IReadOnlyList<string> ProductCategories { get; } =
    [
        ProductCategoryMedicalExpense,
        ProductCategoryDriver,
        ProductCategoryCancer,
        ProductCategoryComprehensive
    ];

    public static IReadOnlyList<string> RegistrationSources { get; } =
    [
        RegistrationSourceDirectInput,
        RegistrationSourceInsuranceDocument
    ];
}
