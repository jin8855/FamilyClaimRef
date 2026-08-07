using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class InsurancePolicyManagementViewModelTests
{
    [Fact]
    public async Task Load_projects_policy_with_family_display_name_and_formatted_summary_fields()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var viewModel = CreateViewModel(policyStorage, familyStorage);

            Assert.True(await viewModel.LoadInsurancePoliciesAsync());

            var item = Assert.Single(viewModel.AvailableInsurancePolicies);
            Assert.Equal(policy, Assert.Single(viewModel.AvailablePolicies));
            Assert.Equal(policy.Id, item.Id);
            Assert.Equal("synthetic family", item.FamilyDisplayName);
            Assert.Equal("synthetic policy", item.DisplayTitle);
            Assert.Equal("synthetic insurer", item.InsurerName);
            Assert.Equal(InsurancePolicyValues.ContractStatusActive, item.ContractStatus);
            Assert.Equal("2026-08-04", item.EnrollmentDate);
            Assert.Equal(InsurancePolicyValues.ProductCategoryCancer, item.ProductCategory);
            Assert.Equal("12,000,000원", item.TotalPlannedPremiumAmount);
        });
    }

    [Fact]
    public async Task Prepare_create_lists_only_active_family_members_and_resets_editor()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var active = await CreateFamilyAsync(familyStorage, "active family");
            var inactive = await CreateFamilyAsync(familyStorage, "inactive family");
            await familyStorage.DeactivateFamilyMemberAsync(inactive.Id, inactive.Version);
            var viewModel = CreateViewModel(
                new JsonPolicyClaimStorageService(rootPath, familyStorage),
                familyStorage);
            viewModel.InsuranceDisplayTitle = "stale value";

            Assert.True(await viewModel.PrepareInsurancePolicyCreateAsync());

            var available = Assert.Single(viewModel.AvailablePolicyFamilyMembers);
            Assert.Equal(active.Id, available.Id);
            Assert.False(viewModel.IsInsurancePolicyEditMode);
            Assert.Null(viewModel.InsuranceDisplayTitle);
            Assert.Null(viewModel.SelectedInsuranceFamilyMemberId);
            Assert.Equal(DateTime.Today, viewModel.InsuranceEnrollmentDate);
            Assert.Equal(
                InsurancePolicyValues.RegistrationSourceDirectInput,
                viewModel.InsuranceRegistrationSource);
            Assert.Equal(InsurancePolicyValues.ContractStatuses, viewModel.InsuranceContractStatusOptions);
            Assert.Equal(InsurancePolicyValues.RenewalTypes, viewModel.InsuranceRenewalTypeOptions);
            Assert.Equal(InsurancePolicyValues.RefundTypes, viewModel.InsuranceRefundTypeOptions);
            Assert.Equal(InsurancePolicyValues.BusinessTypes, viewModel.InsuranceBusinessTypeOptions);
            Assert.Equal(InsurancePolicyValues.ProductCategories, viewModel.InsuranceProductCategoryOptions);
        });
    }

    [Fact]
    public async Task Create_and_update_allow_duplicate_display_titles_and_preserve_record_identity()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var viewModel = CreateViewModel(policyStorage, familyStorage);

            await viewModel.PrepareInsurancePolicyCreateAsync();
            FillEditor(viewModel, family.Id, "same title");
            var first = await viewModel.SaveInsurancePolicyAsync();

            await viewModel.PrepareInsurancePolicyCreateAsync();
            FillEditor(viewModel, family.Id, "same title");
            var second = await viewModel.SaveInsurancePolicyAsync();

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.NotEqual(first.Id, second.Id);
            Assert.Equal(2, (await policyStorage.GetPoliciesAsync()).Count);

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(first.Id));
            viewModel.InsuranceInsurerName = "updated insurer";
            var updated = await viewModel.SaveInsurancePolicyAsync();

            Assert.NotNull(updated);
            Assert.Equal(first.Id, updated.Id);
            Assert.Equal(family.Id, updated.FamilyMemberId);
            Assert.Equal("updated insurer", updated.InsurerName);
            Assert.Equal(2, (await policyStorage.GetPoliciesAsync()).Count);
        });
    }

    [Fact]
    public async Task Disable_refreshes_shared_policy_state_and_clears_stale_selection()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var viewModel = CreateViewModel(policyStorage, familyStorage);
            await viewModel.PrepareInsurancePolicyCreateAsync();
            FillEditor(viewModel, family.Id, "synthetic policy");
            var saved = await viewModel.SaveInsurancePolicyAsync();

            Assert.NotNull(saved);
            Assert.True(viewModel.CanDisablePolicy);

            Assert.True(await viewModel.DisableInsurancePolicyAsync(saved.Id));

            Assert.Empty(viewModel.AvailableInsurancePolicies);
            Assert.Empty(viewModel.AvailablePolicies);
            Assert.Null(viewModel.SelectedPolicyId);
            Assert.Null(viewModel.SelectedPolicyForClaimId);
            Assert.False(viewModel.CanDisablePolicy);
        });
    }

    [Fact]
    public async Task Required_field_validation_failure_keeps_editor_state_and_does_not_write()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var viewModel = CreateViewModel(policyStorage, familyStorage);
            await viewModel.PrepareInsurancePolicyCreateAsync();
            FillEditor(viewModel, family.Id, "synthetic policy");
            viewModel.InsuranceCoveragePeriod = " ";

            var saved = await viewModel.SaveInsurancePolicyAsync();

            Assert.Null(saved);
            Assert.False(viewModel.CanSaveInsurancePolicy);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyRequiredFieldsMessage,
                viewModel.InsurancePolicyMessage);
            Assert.Empty(await policyStorage.GetPoliciesAsync());
            Assert.False(File.Exists(Path.Combine(rootPath, "policies.json")));
        });
    }

    [Fact]
    public async Task Orphan_edit_requires_explicit_family_reselection_without_exposing_raw_id()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteOrphanPolicyAsync(rootPath);
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var replacement = await CreateFamilyAsync(familyStorage, "replacement family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var viewModel = CreateViewModel(policyStorage, familyStorage);

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync("policy_orphan"));
            Assert.True(viewModel.IsInsurancePolicyEditMode);
            Assert.Null(viewModel.SelectedInsuranceFamilyMemberId);
            Assert.False(viewModel.CanSaveInsurancePolicy);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyFamilyUnavailableMessage,
                viewModel.InsurancePolicyMessage);
            Assert.DoesNotContain("family_missing", viewModel.InsurancePolicyMessage, StringComparison.Ordinal);

            viewModel.SelectedInsuranceFamilyMemberId = replacement.Id;
            FillRequiredNewFields(viewModel);
            var saved = await viewModel.SaveInsurancePolicyAsync();

            Assert.NotNull(saved);
            Assert.Equal(replacement.Id, saved.FamilyMemberId);
        });
    }

    [Fact]
    public async Task Legacy_active_status_maps_to_active_but_missing_new_fields_require_selection()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WritePreviousSevenFieldPolicyAsync(rootPath);
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            await ReplaceFamilyReferenceAsync(rootPath, family.Id);
            var viewModel = CreateViewModel(policyStorage, familyStorage);

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync("policy_previous"));

            Assert.Equal(InsurancePolicyValues.ContractStatusActive, viewModel.InsuranceContractStatus);
            Assert.Null(viewModel.InsuranceRenewalType);
            Assert.Null(viewModel.InsuranceRefundType);
            Assert.Null(viewModel.InsuranceBusinessType);
            Assert.Null(viewModel.InsuranceProductCategory);
            Assert.False(viewModel.CanSaveInsurancePolicy);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicySelectionRequired,
                viewModel.InsuranceRenewalTypeGuidance);
        });
    }

    [Fact]
    public async Task Arbitrary_legacy_status_requires_explicit_valid_selection()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await WriteOrphanPolicyAsync(rootPath);
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var viewModel = CreateViewModel(
                new JsonPolicyClaimStorageService(rootPath, familyStorage),
                familyStorage);

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync("policy_orphan"));

            Assert.Null(viewModel.InsuranceContractStatus);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired,
                viewModel.InsuranceContractStatusGuidance);
            Assert.False(viewModel.CanSaveInsurancePolicy);
        });
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("not a number")]
    public async Task Invalid_planned_premium_keeps_save_disabled(string value)
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var viewModel = CreateViewModel(
                new JsonPolicyClaimStorageService(rootPath, familyStorage),
                familyStorage);
            await viewModel.PrepareInsurancePolicyCreateAsync();
            FillEditor(viewModel, family.Id, "synthetic policy");

            viewModel.InsuranceTotalPlannedPremiumAmountText = value;

            Assert.False(viewModel.CanSaveInsurancePolicy);
            Assert.Null(await viewModel.SaveInsurancePolicyAsync());
        });
    }

    [Fact]
    public void Registration_source_has_no_public_setter()
    {
        var property = typeof(PolicyClaimManagementViewModel)
            .GetProperty(nameof(PolicyClaimManagementViewModel.InsuranceRegistrationSource));

        Assert.NotNull(property);
        Assert.False(property.SetMethod?.IsPublic ?? false);
    }

    [Fact]
    public async Task Storage_failure_returns_safe_product_message_without_internal_details()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            await File.WriteAllTextAsync(Path.Combine(rootPath, "family-members.json"), "{ invalid json");
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var viewModel = CreateViewModel(
                new JsonPolicyClaimStorageService(rootPath, familyStorage),
                familyStorage);

            Assert.False(await viewModel.LoadInsurancePoliciesAsync());
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyLoadFailedMessage,
                viewModel.InsurancePolicyMessage);
            Assert.DoesNotContain(rootPath, viewModel.InsurancePolicyMessage, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("JSON", viewModel.InsurancePolicyMessage, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task Prepare_edit_projects_registered_document_titles_for_the_selected_policy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var capture = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "capture.png",
                "registered capture",
                "PNG",
                "documents/capture.png",
                "capture.png",
                "PNG",
                12,
                new string('a', 64),
                DocumentType: "capture"));
            var terms = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "terms.pdf",
                "registered terms",
                "PDF",
                "documents/terms.pdf",
                "terms.pdf",
                "PDF",
                12,
                new string('b', 64),
                DocumentType: "terms"));
            await documentStorage.AddPolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, capture.Id, "capture"));
            await documentStorage.AddPolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, terms.Id, "terms"));
            var viewModel = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                documentStorage,
                new KeyEchoUiTextProvider());

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(policy.Id));

            Assert.Equal("registered capture", viewModel.InsuranceCaptureDocumentStatus);
            Assert.True(viewModel.HasInsuranceCaptureDocument);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus,
                viewModel.InsurancePolicyDocumentStatus);
            Assert.False(viewModel.HasInsurancePolicyDocument);
            Assert.Equal("registered terms", viewModel.InsuranceTermsDocumentStatus);
            Assert.True(viewModel.HasInsuranceTermsDocument);
        });
    }

    [Fact]
    public async Task Open_registered_document_uses_only_its_managed_relative_path()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var document = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "terms.pdf",
                "registered terms",
                "PDF",
                "documents/terms.pdf",
                "terms.pdf",
                "PDF",
                12,
                new string('b', 64),
                DocumentType: "terms"));
            await documentStorage.AddPolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, document.Id, "terms"));
            var opener = new RecordingManagedDocumentOpener();
            var viewModel = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                documentStorage,
                opener,
                new KeyEchoUiTextProvider());
            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(policy.Id));

            Assert.True(await viewModel.OpenInsurancePolicyDocumentAsync("terms"));

            Assert.Equal("documents/terms.pdf", opener.RelativePath);
        });
    }

    [Fact]
    public async Task Replaced_document_projects_current_and_archived_history_and_opens_archived_copy()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var previous = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "terms-old.pdf",
                "previous terms",
                "PDF",
                "documents/terms-old.pdf",
                "terms-old.pdf",
                "PDF",
                12,
                new string('a', 64),
                DocumentType: "terms"));
            await documentStorage.AddPolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, previous.Id, "terms"));
            var current = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "terms-current.pdf",
                "current terms",
                "PDF",
                "documents/terms-current.pdf",
                "terms-current.pdf",
                "PDF",
                12,
                new string('b', 64),
                DocumentType: "terms"));
            await documentStorage.ReplaceActivePolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, current.Id, "terms"));
            var opener = new RecordingManagedDocumentOpener();
            var viewModel = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                documentStorage,
                opener,
                new KeyEchoUiTextProvider());

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(policy.Id));

            Assert.True(viewModel.HasInsurancePolicyDocumentHistory);
            Assert.Equal(2, viewModel.InsurancePolicyDocumentHistory.Count);
            Assert.Equal(
                $"{UiTextKeys.ProductInsurancePolicyDocumentHistoryHeaderFormat}:2",
                viewModel.InsurancePolicyDocumentHistoryTitle);
            var currentItem = Assert.Single(
                viewModel.InsurancePolicyDocumentHistory,
                item => item.IsCurrent);
            Assert.Equal("current terms", currentItem.DisplayTitle);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentHistoryCurrentStatus,
                currentItem.Status);
            var archivedItem = Assert.Single(
                viewModel.InsurancePolicyDocumentHistory,
                item => !item.IsCurrent);
            Assert.Equal("previous terms", archivedItem.DisplayTitle);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus,
                archivedItem.Status);
            Assert.NotEmpty(archivedItem.RegisteredAt);

            Assert.True(await viewModel.OpenInsurancePolicyDocumentHistoryAsync(archivedItem));
            Assert.Equal("documents/terms-old.pdf", opener.RelativePath);
        });
    }

    [Fact]
    public async Task Three_row_history_projects_one_current_two_archived_in_descending_created_order()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var olderCreatedAt = new DateTimeOffset(2026, 8, 3, 9, 0, 0, TimeSpan.Zero);
            var newerCreatedAt = new DateTimeOffset(2026, 8, 4, 9, 0, 0, TimeSpan.Zero);
            var currentCreatedAt = new DateTimeOffset(2026, 8, 5, 9, 0, 0, TimeSpan.Zero);
            var documents = new[]
            {
                new DocumentRecord(
                    "doc-history-older",
                    "terms-history-older.pdf",
                    "older archived terms",
                    "PDF",
                    "documents/terms-history-older.pdf",
                    olderCreatedAt,
                    olderCreatedAt,
                    null),
                new DocumentRecord(
                    "doc-current",
                    "terms-current.pdf",
                    "current terms",
                    "PDF",
                    "documents/terms-current.pdf",
                    currentCreatedAt,
                    currentCreatedAt,
                    null),
                new DocumentRecord(
                    "doc-history-newer",
                    "terms-history-newer.pdf",
                    "newer archived terms",
                    "PDF",
                    "documents/terms-history-newer.pdf",
                    newerCreatedAt,
                    newerCreatedAt,
                    null)
            };
            var linksInFixtureOrder = new[]
            {
                new PolicyDocumentRecord(
                    "link-history-older",
                    policy.Id,
                    "doc-history-older",
                    "terms",
                    olderCreatedAt,
                    olderCreatedAt.AddMinutes(1),
                    olderCreatedAt.AddMinutes(1)),
                new PolicyDocumentRecord(
                    "link-current",
                    policy.Id,
                    "doc-current",
                    "terms",
                    currentCreatedAt,
                    currentCreatedAt,
                    null),
                new PolicyDocumentRecord(
                    "link-history-newer",
                    policy.Id,
                    "doc-history-newer",
                    "terms",
                    newerCreatedAt,
                    newerCreatedAt.AddMinutes(1),
                    newerCreatedAt.AddMinutes(1))
            };
            var documentStorage = new OrderedDocumentHistoryStorage(
                documents,
                linksInFixtureOrder);
            var opener = new RecordingManagedDocumentOpener();
            var viewModel = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                documentStorage,
                opener,
                new KeyEchoUiTextProvider());

            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(policy.Id));

            var history = viewModel.InsurancePolicyDocumentHistory;
            Assert.Equal(3, history.Count);
            Assert.Equal(1, history.Count(item => item.IsCurrent));
            Assert.Equal(2, history.Count(item => !item.IsCurrent));
            Assert.Collection(
                history,
                item =>
                {
                    Assert.Equal("current terms", item.DisplayTitle);
                    Assert.True(item.IsCurrent);
                    Assert.Equal(
                        UiTextKeys.ProductInsurancePolicyDocumentHistoryCurrentStatus,
                        item.Status);
                },
                item =>
                {
                    Assert.Equal("newer archived terms", item.DisplayTitle);
                    Assert.False(item.IsCurrent);
                    Assert.Equal(
                        UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus,
                        item.Status);
                },
                item =>
                {
                    Assert.Equal("older archived terms", item.DisplayTitle);
                    Assert.False(item.IsCurrent);
                    Assert.Equal(
                        UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus,
                        item.Status);
                });
            Assert.Equal(3, history.Select(item => item.DisplayTitle).Distinct(StringComparer.Ordinal).Count());

            Assert.True(await viewModel.OpenInsurancePolicyDocumentHistoryAsync(history[0]));
            Assert.Equal("documents/terms-current.pdf", opener.RelativePath);
            Assert.True(await viewModel.OpenInsurancePolicyDocumentHistoryAsync(history[1]));
            Assert.Equal("documents/terms-history-newer.pdf", opener.RelativePath);
            Assert.True(await viewModel.OpenInsurancePolicyDocumentHistoryAsync(history[2]));
            Assert.Equal("documents/terms-history-older.pdf", opener.RelativePath);
        });
    }

    [Fact]
    public async Task Unlink_document_disables_active_link_but_preserves_document_history()
    {
        await UsingTempRootAsync(async rootPath =>
        {
            var familyStorage = new JsonFamilyMemberStorageService(rootPath);
            var family = await CreateFamilyAsync(familyStorage, "synthetic family");
            var policyStorage = new JsonPolicyClaimStorageService(rootPath, familyStorage);
            var policy = await policyStorage.CreateInsurancePolicyAsync(CreateDraft(family.Id));
            var documentStorage = new JsonDocumentStorageService(rootPath);
            var document = await documentStorage.AddDocumentAsync(new DocumentDraft(
                "terms.pdf",
                "registered terms",
                "PDF",
                "documents/terms.pdf",
                "terms.pdf",
                "PDF",
                12,
                new string('b', 64),
                DocumentType: "terms"));
            await documentStorage.AddPolicyDocumentAsync(
                new PolicyDocumentDraft(policy.Id, document.Id, "terms"));
            var viewModel = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                documentStorage,
                new KeyEchoUiTextProvider());
            Assert.True(await viewModel.PrepareInsurancePolicyEditAsync(policy.Id));

            Assert.True(await viewModel.UnlinkInsurancePolicyDocumentAsync("terms"));

            var links = await documentStorage.GetPolicyDocumentsAsync(policy.Id);
            Assert.Single(links);
            Assert.NotNull(links[0].DisabledAt);
            Assert.NotNull(await documentStorage.GetDocumentByIdAsync(document.Id));
            Assert.False(viewModel.HasInsuranceTermsDocument);
            var history = Assert.Single(viewModel.InsurancePolicyDocumentHistory);
            Assert.False(history.IsCurrent);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus,
                history.Status);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus,
                viewModel.InsuranceTermsDocumentStatus);
            Assert.Equal(
                UiTextKeys.ProductInsurancePolicyDocumentUnlinkedMessage,
                viewModel.InsurancePolicyMessage);
        });
    }

    private static PolicyClaimManagementViewModel CreateViewModel(
        IPolicyClaimStorageService policyStorage,
        IFamilyMemberStorageService familyStorage)
    {
        return new PolicyClaimManagementViewModel(
            policyStorage,
            familyStorage,
            new KeyEchoUiTextProvider());
    }

    private static void FillEditor(
        PolicyClaimManagementViewModel viewModel,
        string familyMemberId,
        string displayTitle)
    {
        viewModel.InsuranceDisplayTitle = displayTitle;
        viewModel.SelectedInsuranceFamilyMemberId = familyMemberId;
        viewModel.InsuranceInsurerName = "synthetic insurer";
        viewModel.InsuranceContractStatus = InsurancePolicyValues.ContractStatusActive;
        viewModel.InsuranceEnrollmentDate = new DateTime(2026, 8, 4);
        viewModel.InsuranceCoveragePeriod = "2026-2027";
        FillRequiredNewFields(viewModel);
    }

    private static void FillRequiredNewFields(PolicyClaimManagementViewModel viewModel)
    {
        viewModel.InsuranceContractStatus = InsurancePolicyValues.ContractStatusActive;
        viewModel.InsurancePremiumPaymentPeriod = "20년납";
        viewModel.InsuranceTotalPlannedPremiumAmountText = "12,000,000";
        viewModel.InsuranceRenewalType = InsurancePolicyValues.RenewalTypeFixed;
        viewModel.InsuranceRefundType = InsurancePolicyValues.RefundTypeRefundable;
        viewModel.InsuranceBusinessType = InsurancePolicyValues.BusinessTypeLife;
        viewModel.InsuranceProductCategory = InsurancePolicyValues.ProductCategoryCancer;
    }

    private static InsurancePolicyDraft CreateDraft(string familyMemberId)
    {
        return new InsurancePolicyDraft(
            "synthetic policy",
            familyMemberId,
            "synthetic insurer",
            InsurancePolicyValues.ContractStatusActive,
            new DateOnly(2026, 8, 4),
            "2026-2027",
            "20년납",
            12_000_000m,
            InsurancePolicyValues.RenewalTypeFixed,
            InsurancePolicyValues.RefundTypeRefundable,
            InsurancePolicyValues.BusinessTypeLife,
            InsurancePolicyValues.ProductCategoryCancer);
    }

    private static Task WritePreviousSevenFieldPolicyAsync(string rootPath)
    {
        Directory.CreateDirectory(rootPath);
        return File.WriteAllTextAsync(
            Path.Combine(rootPath, "policies.json"),
            """
            {
              "schemaVersion": 1,
              "savedAt": "2026-08-04T00:00:00Z",
              "items": [
                {
                  "id": "policy_previous",
                  "displayTitle": "previous policy",
                  "referenceDate": "2026-08-04",
                  "createdAt": "2026-08-04T00:00:00Z",
                  "updatedAt": "2026-08-04T00:00:00Z",
                  "disabledAt": null,
                  "familyMemberId": "family_placeholder",
                  "insurerName": "synthetic insurer",
                  "contractStatus": "사용 중",
                  "enrollmentDate": "2026-08-04",
                  "coveragePeriod": "2026-2027",
                  "registrationSource": "직접 입력"
                }
              ]
            }
            """);
    }

    private static async Task ReplaceFamilyReferenceAsync(string rootPath, string familyMemberId)
    {
        var path = Path.Combine(rootPath, "policies.json");
        var json = await File.ReadAllTextAsync(path);
        await File.WriteAllTextAsync(path, json.Replace("family_placeholder", familyMemberId, StringComparison.Ordinal));
    }

    private static Task<FamilyMemberRecord> CreateFamilyAsync(
        JsonFamilyMemberStorageService storage,
        string displayName)
    {
        return storage.CreateFamilyMemberAsync(new FamilyMemberDraft(
            displayName,
            FamilyMemberRelationValues.Self,
            null));
    }

    private static async Task WriteOrphanPolicyAsync(string rootPath)
    {
        Directory.CreateDirectory(rootPath);
        await File.WriteAllTextAsync(
            Path.Combine(rootPath, "policies.json"),
            """
            {
              "schemaVersion": 1,
              "savedAt": "2026-08-04T00:00:00Z",
              "items": [
                {
                  "id": "policy_orphan",
                  "displayTitle": "orphan policy",
                  "referenceDate": "2026-08-04",
                  "createdAt": "2026-08-04T00:00:00Z",
                  "updatedAt": "2026-08-04T00:00:00Z",
                  "disabledAt": null,
                  "familyMemberId": "family_missing",
                  "insurerName": "synthetic insurer",
                  "contractStatus": "active",
                  "enrollmentDate": "2026-08-04",
                  "coveragePeriod": "2026-2027",
                  "registrationSource": "manual"
                }
              ]
            }
            """);
    }

    private static async Task UsingTempRootAsync(Func<string, Task> action)
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            nameof(InsurancePolicyManagementViewModelTests),
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

    private sealed class OrderedDocumentHistoryStorage : IDocumentStorageService
    {
        private readonly IReadOnlyDictionary<string, DocumentRecord> documents;
        private readonly IReadOnlyList<PolicyDocumentRecord> policyDocuments;

        public OrderedDocumentHistoryStorage(
            IEnumerable<DocumentRecord> documents,
            IReadOnlyList<PolicyDocumentRecord> policyDocuments)
        {
            this.documents = documents.ToDictionary(document => document.Id, StringComparer.Ordinal);
            this.policyDocuments = policyDocuments;
        }

        public Task<IReadOnlyList<DocumentRecord>> GetDocumentsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DocumentRecord>>(documents.Values.ToList());

        public Task<DocumentRecord?> GetDocumentByIdAsync(
            string documentId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(documents.GetValueOrDefault(documentId));

        public Task<IReadOnlyList<PolicyDocumentRecord>> GetPolicyDocumentsAsync(
            string policyId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PolicyDocumentRecord>>(
                policyDocuments.Where(link => link.PolicyId == policyId).ToList());

        public Task<DocumentRecord> AddDocumentAsync(
            DocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DisableDocumentAsync(
            string documentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PolicyDocumentRecord> AddPolicyDocumentAsync(
            PolicyDocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DisablePolicyDocumentAsync(
            string policyDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ClaimDocumentRecord>> GetClaimDocumentsAsync(
            string claimId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ClaimDocumentRecord> AddClaimDocumentAsync(
            ClaimDocumentDraft draft,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task DisableClaimDocumentAsync(
            string claimDocumentId,
            DateTimeOffset disabledAt,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class KeyEchoUiTextProvider : IUiTextProvider
    {
        public string Get(string key) => key;

        public string Format(string key, params object?[] args) =>
            $"{key}:{string.Join(',', args)}";
    }

    private sealed class RecordingManagedDocumentOpener : IManagedDocumentOpener
    {
        public string? RelativePath { get; private set; }

        public Task OpenAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            RelativePath = relativePath;
            return Task.CompletedTask;
        }
    }
}
