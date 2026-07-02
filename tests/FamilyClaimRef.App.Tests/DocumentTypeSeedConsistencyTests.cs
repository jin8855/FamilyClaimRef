using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class DocumentTypeSeedConsistencyTests
{
    private static readonly DateOnly TestDate = new(2026, 6, 26);

    public static IEnumerable<object[]> CurrentSeedCases =>
        DocumentTypeSeeds.All.Select(seed => new object[] { seed.Scope, seed.Code });

    public static IEnumerable<object[]> ScopeMismatchRejectedCases
    {
        get
        {
            var policyCodes = DocumentTypeSeeds.Policy.Select(seed => seed.Code).ToHashSet(StringComparer.Ordinal);
            var claimCodes = DocumentTypeSeeds.Claim.Select(seed => seed.Code).ToHashSet(StringComparer.Ordinal);

            foreach (var claimSeed in DocumentTypeSeeds.Claim.Where(seed => !policyCodes.Contains(seed.Code)))
            {
                yield return [DocumentTypeSeeds.PolicyScope, claimSeed.Code];
            }

            foreach (var policySeed in DocumentTypeSeeds.Policy.Where(seed => !claimCodes.Contains(seed.Code)))
            {
                yield return [DocumentTypeSeeds.ClaimScope, policySeed.Code];
            }
        }
    }

    [Fact]
    public void DocumentTypeSeeds_have_expected_counts()
    {
        Assert.Equal(7, DocumentTypeSeeds.Claim.Count);
        Assert.Equal(5, DocumentTypeSeeds.Policy.Count);
        Assert.Equal(12, DocumentTypeSeeds.All.Count);
    }

    [Fact]
    public void DocumentTypeSeeds_have_required_structure()
    {
        Assert.All(DocumentTypeSeeds.All, seed =>
        {
            Assert.False(string.IsNullOrWhiteSpace(seed.Code));
            Assert.False(string.IsNullOrWhiteSpace(seed.Label));
            Assert.True(seed.Scope is DocumentTypeSeeds.ClaimScope or DocumentTypeSeeds.PolicyScope);
            Assert.Null(seed.DisabledAt);
        });

        var scopeCodeCount = DocumentTypeSeeds.All
            .Select(seed => (seed.Scope, seed.Code))
            .Distinct()
            .Count();
        Assert.Equal(DocumentTypeSeeds.All.Count, scopeCodeCount);

        foreach (var scopeGroup in DocumentTypeSeeds.All.GroupBy(seed => seed.Scope))
        {
            var sortOrderCount = scopeGroup.Select(seed => seed.SortOrder).Distinct().Count();
            Assert.Equal(scopeGroup.Count(), sortOrderCount);
        }
    }

    [Fact]
    public void Claim_seed_codes_match_FileNamePolicyService_accessor()
    {
        var seedCodes = DocumentTypeSeeds.Claim.Select(seed => seed.Code).ToHashSet(StringComparer.Ordinal);
        var accessorCodes = FileNamePolicyService
            .GetAllowedDocumentTypes(DocumentTypeSeeds.ClaimScope)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Empty(seedCodes.Except(accessorCodes));
        Assert.Empty(accessorCodes.Except(seedCodes));
    }

    [Fact]
    public void Policy_seed_codes_match_FileNamePolicyService_accessor()
    {
        var seedCodes = DocumentTypeSeeds.Policy.Select(seed => seed.Code).ToHashSet(StringComparer.Ordinal);
        var accessorCodes = FileNamePolicyService
            .GetAllowedDocumentTypes(DocumentTypeSeeds.PolicyScope)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Empty(seedCodes.Except(accessorCodes));
        Assert.Empty(accessorCodes.Except(seedCodes));
    }

    [Theory]
    [MemberData(nameof(CurrentSeedCases))]
    public void Current_seed_codes_are_accepted_by_FileNamePolicyService(string scope, string documentType)
    {
        var physicalFileName = CreatePhysicalFileName(scope, documentType);

        Assert.Equal($"{scope}-test_000001_20260626_{documentType}.pdf", physicalFileName);
    }

    [Theory]
    [MemberData(nameof(ScopeMismatchRejectedCases))]
    public void Scope_mismatch_seed_codes_are_rejected_by_FileNamePolicyService(string scope, string documentType)
    {
        var exception = Record.Exception(() => CreatePhysicalFileName(scope, documentType));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void New_candidate_claim_codes_are_excluded_from_FileNamePolicyService_accessor()
    {
        var allowedDocumentTypes = FileNamePolicyService.GetAllowedDocumentTypes(DocumentTypeSeeds.ClaimScope);

        Assert.DoesNotContain("statement", allowedDocumentTypes);
        Assert.DoesNotContain("prescription", allowedDocumentTypes);
        Assert.DoesNotContain("capture", allowedDocumentTypes);
    }

    [Fact]
    public void Policy_capture_code_is_included_in_FileNamePolicyService_accessor()
    {
        var allowedDocumentTypes = FileNamePolicyService.GetAllowedDocumentTypes(DocumentTypeSeeds.PolicyScope);

        Assert.Contains("capture", allowedDocumentTypes);
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData("   ")]
    public void Invalid_scope_is_rejected_by_FileNamePolicyService_accessor(string scope)
    {
        var exception = Record.Exception(() => FileNamePolicyService.GetAllowedDocumentTypes(scope));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Shared_etc_code_is_accepted_in_both_scopes()
    {
        var claimPhysicalFileName = CreatePhysicalFileName(DocumentTypeSeeds.ClaimScope, "etc");
        var policyPhysicalFileName = CreatePhysicalFileName(DocumentTypeSeeds.PolicyScope, "etc");

        Assert.Equal("claim-test_000001_20260626_etc.pdf", claimPhysicalFileName);
        Assert.Equal("policy-test_000001_20260626_etc.pdf", policyPhysicalFileName);
    }

    [Theory]
    [InlineData("statement")]
    [InlineData("prescription")]
    [InlineData("capture")]
    public void New_candidate_claim_codes_are_rejected_by_FileNamePolicyService(string documentType)
    {
        var exception = Record.Exception(() => CreatePhysicalFileName(DocumentTypeSeeds.ClaimScope, documentType));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void Policy_capture_code_is_accepted_by_FileNamePolicyService()
    {
        var physicalFileName = CreatePhysicalFileName(DocumentTypeSeeds.PolicyScope, "capture");

        Assert.Equal("policy-test_000001_20260626_capture.pdf", physicalFileName);
    }

    private static string CreatePhysicalFileName(string scope, string documentType)
    {
        return FileNamePolicyService.CreatePhysicalFileName(
            scope,
            "test_000001",
            TestDate,
            documentType,
            "pdf");
    }
}
