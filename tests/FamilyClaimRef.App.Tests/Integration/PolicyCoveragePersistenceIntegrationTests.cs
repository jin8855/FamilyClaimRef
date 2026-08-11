using System.Text.Json;
using FamilyClaimRef.App.Composition;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using Xunit;

namespace FamilyClaimRef.App.Tests.Integration;

[Collection(RuntimeEnvironmentCollectionName.Value)]
public sealed class PolicyCoveragePersistenceIntegrationTests
{
    private const string RuntimeOverrideEnabledVariable =
        "FAMILYCLAIMREF_ENABLE_DEV_RUNTIME_ROOT_OVERRIDE";
    private const string RuntimeRootVariable = "FAMILYCLAIMREF_RUNTIME_ROOT";

    [Fact]
    public async Task AppServices_isolated_runtime_persists_full_coverage_lifecycle_across_reloads()
    {
        var testRunRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef-TestRuns",
            $"policy-coverage-{Guid.NewGuid():N}");
        var isolatedRuntimeRoot = Path.Combine(testRunRoot, "runtime");

        try
        {
            using var _ = new EnvironmentVariableScope(
                (RuntimeOverrideEnabledVariable, "1"),
                (RuntimeRootVariable, isolatedRuntimeRoot));

            var createdServices = AppServices.CreateDefault();
            Assert.Equal(Path.GetFullPath(isolatedRuntimeRoot), createdServices.RuntimeRootPath);
            var families = new JsonFamilyMemberStorageService(createdServices.MetadataRootPath);
            var policies = new JsonPolicyClaimStorageService(
                createdServices.MetadataRootPath,
                families);
            var family = await families.CreateFamilyMemberAsync(new FamilyMemberDraft(
                "family A",
                FamilyMemberRelationValues.Self,
                null));
            var policy = await policies.CreateInsurancePolicyAsync(new InsurancePolicyDraft(
                DisplayTitle: "policy A",
                FamilyMemberId: family.Id,
                InsurerName: "insurer A",
                ContractStatus: InsurancePolicyValues.ContractStatusActive,
                EnrollmentDate: new DateOnly(2026, 1, 1),
                CoveragePeriod: "2026-2027",
                PremiumPaymentPeriod: "20 years",
                TotalPlannedPremiumAmount: 12_000_000m,
                RenewalType: InsurancePolicyValues.RenewalTypeFixed,
                RefundType: InsurancePolicyValues.RefundTypeRefundable,
                InsuranceBusinessType: InsurancePolicyValues.BusinessTypeLife,
                ProductCategory: InsurancePolicyValues.ProductCategoryCancer));

            var created = await createdServices.PolicyCoverageStorageService
                .CreatePolicyCoverageAsync(CreateDraft(policy.Id));
            Assert.Equal(PolicyCoverageValues.ReviewStatusNeedsReview, created.ReviewStatus);

            var confirmedServices = AppServices.CreateDefault();
            Assert.NotSame(
                createdServices.PolicyCoverageStorageService,
                confirmedServices.PolicyCoverageStorageService);
            var confirmed = await confirmedServices.PolicyCoverageStorageService
                .ChangePolicyCoverageReviewStatusAsync(
                    created.PolicyCoverageId,
                    created.Revision,
                    PolicyCoverageValues.ReviewStatusUserConfirmed);

            var updateServices = AppServices.CreateDefault();
            var updated = await updateServices.PolicyCoverageStorageService
                .UpdatePolicyCoverageAsync(
                    confirmed.PolicyCoverageId,
                    confirmed.Revision,
                    CreateUpdateDraft(confirmed) with
                    {
                        SurgeryRule = PolicyCoverageValues.ConditionExcluded
                    });
            Assert.Equal(PolicyCoverageValues.ReviewStatusNeedsReview, updated.ReviewStatus);

            var disableServices = AppServices.CreateDefault();
            var disabled = await disableServices.PolicyCoverageStorageService
                .DisablePolicyCoverageAsync(updated.PolicyCoverageId, updated.Revision);
            Assert.NotNull(disabled.DisabledAt);

            var restoreServices = AppServices.CreateDefault();
            var restored = await restoreServices.PolicyCoverageStorageService
                .RestorePolicyCoverageAsync(disabled.PolicyCoverageId, disabled.Revision);
            Assert.Null(restored.DisabledAt);

            var finalServices = AppServices.CreateDefault();
            var reloaded = await finalServices.PolicyCoverageStorageService
                .GetPolicyCoverageAsync(restored.PolicyCoverageId);
            Assert.NotNull(reloaded);
            Assert.Equal(restored.DiagnosisCodePrefixes, reloaded.DiagnosisCodePrefixes);
            Assert.Equal(
                restored with { DiagnosisCodePrefixes = reloaded.DiagnosisCodePrefixes },
                reloaded);
            Assert.Equal(5, reloaded!.Revision);

            var coveragePath = Path.Combine(
                finalServices.MetadataRootPath,
                JsonPolicyCoverageStorageService.StoreFileName);
            var backupPath = coveragePath + ".bak";
            Assert.True(File.Exists(coveragePath));
            Assert.True(File.Exists(backupPath));
            Assert.Empty(Directory.GetFiles(
                finalServices.MetadataRootPath,
                "*.tmp",
                SearchOption.TopDirectoryOnly));

            using var currentJson = JsonDocument.Parse(await File.ReadAllTextAsync(coveragePath));
            using var backupJson = JsonDocument.Parse(await File.ReadAllTextAsync(backupPath));
            Assert.Equal(1, currentJson.RootElement.GetProperty("schemaVersion").GetInt32());
            Assert.Equal(1, backupJson.RootElement.GetProperty("schemaVersion").GetInt32());
            Assert.Single(currentJson.RootElement.GetProperty("items").EnumerateArray());
            Assert.Single(backupJson.RootElement.GetProperty("items").EnumerateArray());
            Assert.Null(currentJson.RootElement.GetProperty("items")[0].GetProperty("disabledAt").GetString());
            Assert.NotNull(backupJson.RootElement.GetProperty("items")[0].GetProperty("disabledAt").GetString());
        }
        finally
        {
            if (Directory.Exists(testRunRoot))
            {
                Directory.Delete(testRunRoot, recursive: true);
            }
        }

        Assert.False(Directory.Exists(testRunRoot));
    }

    private static PolicyCoverageCreateDraft CreateDraft(string policyId)
    {
        return new PolicyCoverageCreateDraft(
            PolicyId: policyId,
            DisplayName: "coverage A",
            ReviewStatus: PolicyCoverageValues.ReviewStatusNeedsReview,
            EffectiveFrom: new DateOnly(2026, 1, 1),
            EffectiveTo: new DateOnly(2027, 12, 31),
            VisitTypeRule: PolicyCoverageValues.VisitTypeAny,
            SurgeryRule: PolicyCoverageValues.ConditionRequired,
            PrescriptionRule: PolicyCoverageValues.ConditionAny,
            DiagnosisRuleMode: PolicyCoverageValues.DiagnosisRulePrefixList,
            DiagnosisCodePrefixes: ["A00.1"],
            SourceKind: PolicyCoverageValues.SourceManual,
            SourcePolicyDocumentId: null,
            SourceLocator: "private-locator",
            Memo: "synthetic memo");
    }

    private static PolicyCoverageUpdateDraft CreateUpdateDraft(PolicyCoverageRecord record)
    {
        return new PolicyCoverageUpdateDraft(
            record.DisplayName,
            record.EffectiveFrom,
            record.EffectiveTo,
            record.VisitTypeRule,
            record.SurgeryRule,
            record.PrescriptionRule,
            record.DiagnosisRuleMode,
            record.DiagnosisCodePrefixes,
            record.SourceKind,
            record.SourcePolicyDocumentId,
            record.SourceLocator,
            record.Memo);
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly IReadOnlyList<(string Name, string? Value)> originals;

        public EnvironmentVariableScope(params (string Name, string? Value)[] values)
        {
            originals = values
                .Select(value => (value.Name, Environment.GetEnvironmentVariable(value.Name)))
                .ToList();
            foreach (var (name, value) in values)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            foreach (var (name, value) in originals)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }
    }
}
