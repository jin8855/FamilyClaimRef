using FamilyClaimRef.App.Services;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class FileNamePolicyServiceTests
{
    public static TheoryData<string, string, DateOnly, string, string, int?, string> NormalCases => new()
    {
        { "claim", "000001", new DateOnly(2026, 6, 26), "receipt", "pdf", null, "claim-000001_20260626_receipt.pdf" },
        { "claim", "000001", new DateOnly(2026, 6, 26), "receipt", ".pdf", 1, "claim-000001_20260626_receipt_001.pdf" },
        { "policy", "000003", new DateOnly(2026, 6, 26), "terms", "pdf", null, "policy-000003_20260626_terms.pdf" },
        { "policy", "000003", new DateOnly(2026, 6, 26), "terms", ".pdf", 2, "policy-000003_20260626_terms_002.pdf" },
        { "Claim", "ID_001", new DateOnly(2026, 6, 26), "receipt", "PDF", null, "claim-ID_001_20260626_receipt.pdf" },
        { "policy", "POLICY-001", new DateOnly(2026, 6, 26), "capture", ".PNG", null, "policy-POLICY-001_20260626_capture.png" },
        { "claim", "CLAIM_001", new DateOnly(2026, 6, 26), "etc", "jpg", null, "claim-CLAIM_001_20260626_etc.jpg" }
    };

    public static TheoryData<string, string, DateOnly, string, string, int?, Type> ErrorCases => new()
    {
        { null!, "000001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "   ", "000001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "other", "000001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", null!, DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "   ", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "ID 001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "ID/001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "ID:001", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, null!, "pdf", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "   ", "pdf", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "terms", "pdf", null, typeof(ArgumentException) },
        { "policy", "000003", DefaultDate, "receipt", "pdf", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", null!, null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", "   ", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", "p/df", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", ".", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", "pdf", 0, typeof(ArgumentOutOfRangeException) },
        { "claim", "000001", DefaultDate, "receipt", "pdf", -1, typeof(ArgumentOutOfRangeException) },
        { "claim", "000001", DefaultDate, "receipt", "pdf", 1000, typeof(ArgumentOutOfRangeException) },
        { "claim", "000001", DefaultDate, "receipt", "exe", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", "zip", null, typeof(ArgumentException) },
        { "claim", "000001", DefaultDate, "receipt", "docx", null, typeof(ArgumentException) }
    };

    public static TheoryData<string, string, DateOnly, string, string, int?, string> BoundaryCases => new()
    {
        { "claim", "000001", new DateOnly(2026, 6, 26), "receipt", "pdf", null, "claim-000001_20260626_receipt.pdf" },
        { "claim", "000001", new DateOnly(2026, 6, 26), "receipt", "pdf", 999, "claim-000001_20260626_receipt_999.pdf" },
        { "claim", "000001", new DateOnly(2026, 6, 26), "receipt", ".pdf", null, "claim-000001_20260626_receipt.pdf" },
        { "claim", "000002", new DateOnly(2026, 6, 26), "receipt", "pdf", null, "claim-000002_20260626_receipt.pdf" }
    };

    private static DateOnly DefaultDate => new(2026, 6, 26);

    [Theory]
    [MemberData(nameof(NormalCases))]
    public void CreatePhysicalFileName_returns_expected_name_for_normal_cases(
        string documentScope,
        string id,
        DateOnly date,
        string documentType,
        string extension,
        int? duplicateIndex,
        string expected)
    {
        var actual = FileNamePolicyService.CreatePhysicalFileName(
            documentScope,
            id,
            date,
            documentType,
            extension,
            duplicateIndex);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ErrorCases))]
    public void CreatePhysicalFileName_throws_expected_exception_for_error_cases(
        string documentScope,
        string id,
        DateOnly date,
        string documentType,
        string extension,
        int? duplicateIndex,
        Type expectedExceptionType)
    {
        var exception = Record.Exception(() => FileNamePolicyService.CreatePhysicalFileName(
            documentScope,
            id,
            date,
            documentType,
            extension,
            duplicateIndex));

        Assert.NotNull(exception);
        Assert.IsType(expectedExceptionType, exception);
    }

    [Theory]
    [MemberData(nameof(BoundaryCases))]
    public void CreatePhysicalFileName_returns_expected_name_for_boundary_cases(
        string documentScope,
        string id,
        DateOnly date,
        string documentType,
        string extension,
        int? duplicateIndex,
        string expected)
    {
        var actual = FileNamePolicyService.CreatePhysicalFileName(
            documentScope,
            id,
            date,
            documentType,
            extension,
            duplicateIndex);

        Assert.Equal(expected, actual);
    }
}
