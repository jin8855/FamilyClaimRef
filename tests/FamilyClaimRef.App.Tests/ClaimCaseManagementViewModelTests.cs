using System.Text;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ClaimCaseManagementViewModelTests
{
    [Fact]
    public async Task Create_uses_family_owner_and_keeps_exact_selected_claim_id()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var (viewModel, service, familyStore) = CreateContext(rootPath);
            var family = await CreateFamilyAsync(familyStore);
            Assert.True(await viewModel.LoadAsync());
            PopulateEditor(viewModel, family.Id);

            Assert.True(await viewModel.CreateClaimCaseRecordAsync());

            var created = Assert.Single(await service.GetClaimCasesAsync());
            Assert.Equal(created.Id, viewModel.SelectedClaimId);
            Assert.Equal(created.Revision, viewModel.EditingClaimCaseRevision);
            Assert.Equal(family.Id, created.FamilyMemberId);
            Assert.Null(created.PolicyId);
            Assert.Equal(ClaimCaseValues.StatusDraft, created.CaseStatus);
            Assert.Equal("draft-created", viewModel.ClaimCaseOperationMessage);
            Assert.Null(viewModel.ClaimCaseValidationMessage);
        });
    }

    [Fact]
    public async Task Save_updates_exact_claim_and_exposes_safe_success_message()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var (viewModel, service, familyStore) = CreateContext(rootPath);
            var family = await CreateFamilyAsync(familyStore);
            await viewModel.LoadAsync();
            PopulateEditor(viewModel, family.Id);
            viewModel.ClaimCoveredAmountText = "1,000";
            Assert.True(await viewModel.CreateClaimCaseRecordAsync());
            var createdId = viewModel.SelectedClaimId;
            var createdRevision = viewModel.EditingClaimCaseRevision;

            viewModel.ClaimCaseDisplayTitle = "updated claim";
            viewModel.ClaimDiagnosisCode = "b20.1";
            Assert.True(await viewModel.SaveClaimCaseAsync());

            var saved = await service.GetClaimCaseAsync(createdId!);
            Assert.NotNull(saved);
            Assert.Equal("updated claim", saved.DisplayTitle);
            Assert.Equal("B20.1", saved.DiagnosisCode);
            Assert.Equal(1000, saved.CoveredAmount);
            Assert.Equal(ClaimCaseValues.StatusSaved, saved.CaseStatus);
            Assert.Equal(createdRevision + 1, saved.Revision);
            Assert.Equal("saved", viewModel.ClaimCaseOperationMessage);
        });
    }

    [Fact]
    public async Task Stale_save_reports_conflict_without_leaking_internal_detail()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var (viewModel, service, familyStore) = CreateContext(rootPath);
            var family = await CreateFamilyAsync(familyStore);
            await viewModel.LoadAsync();
            PopulateEditor(viewModel, family.Id);
            Assert.True(await viewModel.CreateClaimCaseRecordAsync());
            var claimId = viewModel.SelectedClaimId!;
            var revision = viewModel.EditingClaimCaseRevision!.Value;
            await service.UpdateClaimCaseAsync(
                claimId,
                revision,
                CreateDraft(family.Id, "external winner"));

            viewModel.ClaimCaseDisplayTitle = "stale editor";
            Assert.False(await viewModel.SaveClaimCaseAsync());

            Assert.Equal("conflict", viewModel.ClaimCaseConflictMessage);
            Assert.Null(viewModel.ClaimCaseOperationMessage);
            Assert.DoesNotContain(claimId, viewModel.ClaimCaseConflictMessage, StringComparison.Ordinal);
            Assert.Equal("external winner", (await service.GetClaimCaseAsync(claimId))!.DisplayTitle);
        });
    }

    [Fact]
    public async Task Unresolved_legacy_claim_is_visible_but_edit_and_disable_are_blocked()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var (viewModel, _, familyStore) = CreateContext(rootPath);
            await CreateFamilyAsync(familyStore);
            await WriteLegacyClaimAsync(rootPath);
            Assert.True(await viewModel.LoadAsync());
            viewModel.SelectedClaimId = "claim_legacy";

            Assert.True(await viewModel.LoadSelectedClaimCaseAsync());

            Assert.Equal("legacy-review", viewModel.ClaimCaseLegacyReviewMessage);
            Assert.False(viewModel.CanSaveClaimCase);
            Assert.False(viewModel.CanDisableClaimCase);
            Assert.False(await viewModel.SaveClaimCaseAsync());
            Assert.False(await viewModel.DisableSelectedClaimCaseAsync());
        });
    }

    [Fact]
    public async Task Concurrent_create_and_save_reentry_each_allow_exactly_one_mutation()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var (viewModel, service, familyStore) = CreateContext(rootPath);
            var family = await CreateFamilyAsync(familyStore);
            await viewModel.LoadAsync();
            PopulateEditor(viewModel, family.Id);

            var createResults = await Task.WhenAll(
                viewModel.CreateClaimCaseRecordAsync(),
                viewModel.CreateClaimCaseRecordAsync());

            Assert.Single(createResults, result => result);
            var created = Assert.Single(await service.GetClaimCasesAsync());
            Assert.Equal(1, created.Revision);

            viewModel.ClaimCaseDisplayTitle = "single saved mutation";
            var saveResults = await Task.WhenAll(
                viewModel.SaveClaimCaseAsync(),
                viewModel.SaveClaimCaseAsync());

            Assert.Single(saveResults, result => result);
            var saved = await service.GetClaimCaseAsync(created.Id);
            Assert.NotNull(saved);
            Assert.Equal("single saved mutation", saved.DisplayTitle);
            Assert.Equal(2, saved.Revision);
        });
    }

    [Fact]
    public void Screen_7_uses_dedicated_claim_case_properties_and_no_policy_requirement()
    {
        var xamlPath = Path.Combine(
            FindProjectRoot(),
            "app",
            "FamilyClaimRef.App",
            "Views",
            "ProductClaimCasesView.xaml");
        var xaml = File.ReadAllText(xamlPath);

        Assert.DoesNotContain("CurrentScreen.Fields", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("SelectedPolicyForClaimId", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("ProductClaim_Policy", xaml, StringComparison.Ordinal);
        Assert.Contains("SelectedClaimFamilyMemberId", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimCaseDisplayTitle", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimTreatmentDate", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimHospitalName", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimDiagnosisCode", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimDiagnosisName", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimVisitType", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimHasSurgery", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimHasPrescription", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimCoveredAmountText", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimNonCoveredAmountText", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimPrescriptionAmountText", xaml, StringComparison.Ordinal);
        Assert.Contains("ClaimMemo", xaml, StringComparison.Ordinal);
        Assert.Contains("ProductClaim_ValidationStatus", xaml, StringComparison.Ordinal);
        Assert.Contains("ProductClaim_ConflictStatus", xaml, StringComparison.Ordinal);
        Assert.Contains("ProductClaim_LegacyStatus", xaml, StringComparison.Ordinal);
    }

    private static (
        PolicyClaimManagementViewModel ViewModel,
        JsonPolicyClaimStorageService Service,
        JsonFamilyMemberStorageService FamilyStore) CreateContext(string rootPath)
    {
        var familyStore = new JsonFamilyMemberStorageService(rootPath);
        var service = new JsonPolicyClaimStorageService(rootPath, familyStore);
        var viewModel = new PolicyClaimManagementViewModel(
            service,
            familyStore,
            CreateUiTextProvider());
        return (viewModel, service, familyStore);
    }

    private static void PopulateEditor(
        PolicyClaimManagementViewModel viewModel,
        string familyMemberId)
    {
        viewModel.StartNewClaimCase();
        viewModel.ClaimCaseDisplayTitle = "synthetic claim";
        viewModel.SelectedClaimFamilyMemberId = familyMemberId;
        viewModel.ClaimTreatmentDate = new DateTime(2026, 8, 7);
        viewModel.ClaimHospitalName = "synthetic hospital";
        viewModel.ClaimDiagnosisCode = "a12.3";
        viewModel.ClaimDiagnosisName = "synthetic diagnosis";
        viewModel.ClaimVisitType = ClaimCaseValues.VisitTypeOutpatient;
        viewModel.ClaimHasPrescription = true;
        viewModel.ClaimCoveredAmountText = "1000";
        viewModel.ClaimNonCoveredAmountText = "2000";
        viewModel.ClaimPrescriptionAmountText = "3000";
        viewModel.ClaimMemo = "synthetic memo";
    }

    private static ClaimCaseDraft CreateDraft(string familyMemberId, string title)
    {
        return new ClaimCaseDraft(
            title,
            familyMemberId,
            new DateOnly(2026, 8, 7),
            "synthetic hospital",
            null,
            null,
            ClaimCaseValues.VisitTypeOutpatient,
            false,
            false,
            null,
            null,
            null,
            null);
    }

    private static Task<FamilyMemberRecord> CreateFamilyAsync(
        JsonFamilyMemberStorageService storage)
    {
        return storage.CreateFamilyMemberAsync(new FamilyMemberDraft(
            "synthetic family",
            FamilyMemberRelationValues.Self,
            null));
    }

    private static async Task WriteLegacyClaimAsync(string rootPath)
    {
        var json = """
            {
              "schemaVersion": 1,
              "savedAt": "2026-08-07T00:00:00+00:00",
              "items": [
                {
                  "id": "claim_legacy",
                  "policyId": "policy_missing",
                  "displayTitle": "legacy claim",
                  "referenceDate": "2026-08-01",
                  "createdAt": "2026-08-01T00:00:00+00:00",
                  "updatedAt": "2026-08-01T00:00:00+00:00",
                  "disabledAt": null
                }
              ]
            }
            """;
        await File.WriteAllTextAsync(
            Path.Combine(rootPath, "claims.json"),
            json,
            Encoding.UTF8);
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new ResourceUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductInsurancePolicyDocumentCreateStatus] = "document-create",
            [UiTextKeys.ProductClaimCasesOperationFailedMessage] = "operation-failed",
            [UiTextKeys.ProductClaimCaseVisitTypeOutpatient] = "outpatient",
            [UiTextKeys.ProductClaimCaseVisitTypeInpatient] = "inpatient",
            [UiTextKeys.ProductClaimCaseValidationRequiredMessage] = "validation",
            [UiTextKeys.ProductClaimCaseConflictMessage] = "conflict",
            [UiTextKeys.ProductClaimCaseLegacyReviewRequiredMessage] = "legacy-review",
            [UiTextKeys.ProductClaimCaseDraftCreatedMessage] = "draft-created",
            [UiTextKeys.ProductClaimCaseSavedMessage] = "saved",
            [UiTextKeys.ProductClaimCaseDisabledMessage] = "disabled"
        });
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(ClaimCaseManagementViewModelTests),
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);

        try
        {
            await action(rootPath);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    private static string FindProjectRoot()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory is not null)
        {
            if (File.Exists(Path.Combine(currentDirectory.FullName, "FamilyClaimRef.sln")))
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new InvalidOperationException("Project root was not found.");
    }
}
