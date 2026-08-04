namespace FamilyClaimRef.App.ViewModels;

public static class ProductScreenRoutes
{
    public const string HomeDashboard = "01_home_dashboard";
    public const string FamilyMembers = "02_family_members";
    public const string PolicyList = "03_policy_list";
    public const string PolicyDetail = "04_policy_detail";
    public const string DocumentBox = "05_document_box";
    public const string OcrReview = "06_ocr_review";
    public const string ClaimCase = "07_claim_case";
    public const string ClaimSubmission = "08_claim_submission";
    public const string ClaimReferenceResult = "09_claim_reference_result";
    public const string HistoryView = "10_history_view";
    public const string PolicyManage = "11_policy_manage";
    public const string PolicyRegister = "12_policy_register";
    public const string FamilyRegister = "13_family_register";
    public const string ClaimComplete = "14_claim_complete";
    public const string ManageHome = "15_manage_home";
    public const string CategoryManage = "16_category_manage";
    public const string PolicyDocumentRegister = "17_policy_document_register";
    public const string ClaimDocumentRegister = "18_claim_document_register";
    public const string CategoryRegister = "19_category_register";
    public const string CategoryItemRegister = "20_category_item_register";
    public const string HistoryDetail = "21_history_detail";

    public static IReadOnlyList<string> All { get; } =
    [
        HomeDashboard,
        FamilyMembers,
        PolicyList,
        PolicyDetail,
        DocumentBox,
        OcrReview,
        ClaimCase,
        ClaimSubmission,
        ClaimReferenceResult,
        HistoryView,
        PolicyManage,
        PolicyRegister,
        FamilyRegister,
        ClaimComplete,
        ManageHome,
        CategoryManage,
        PolicyDocumentRegister,
        ClaimDocumentRegister,
        CategoryRegister,
        CategoryItemRegister,
        HistoryDetail
    ];
}
