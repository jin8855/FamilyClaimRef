using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;
using FamilyClaimRef.App.ViewModels;
using Xunit;

namespace FamilyClaimRef.App.Tests;

public sealed class ProductShellViewModelTests
{
    [Fact]
    public void Constructor_rejects_null_ui_text_provider()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);
        var documentList = CreateDocumentListViewModel(uiTextProvider);
        var policyClaimManagement = CreatePolicyClaimManagementViewModel(uiTextProvider);
        var familyMemberManagement = CreateFamilyMemberManagementViewModel(uiTextProvider);

        var exception = Record.Exception(
            () => new ProductShellViewModel(
                null!,
                documentRegistration,
                documentList,
                policyClaimManagement,
                familyMemberManagement));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_document_registration_view_model()
    {
        var uiTextProvider = CreateUiTextProvider();
        var exception = Record.Exception(
            () => new ProductShellViewModel(
                uiTextProvider,
                null!,
                CreateDocumentListViewModel(uiTextProvider),
                CreatePolicyClaimManagementViewModel(uiTextProvider),
                CreateFamilyMemberManagementViewModel(uiTextProvider)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_document_list_view_model()
    {
        var uiTextProvider = CreateUiTextProvider();
        var exception = Record.Exception(
            () => new ProductShellViewModel(
                uiTextProvider,
                CreateDocumentRegistrationViewModel(uiTextProvider),
                null!,
                CreatePolicyClaimManagementViewModel(uiTextProvider),
                CreateFamilyMemberManagementViewModel(uiTextProvider)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_policy_claim_management_view_model()
    {
        var uiTextProvider = CreateUiTextProvider();
        var exception = Record.Exception(
            () => new ProductShellViewModel(
                uiTextProvider,
                CreateDocumentRegistrationViewModel(uiTextProvider),
                CreateDocumentListViewModel(uiTextProvider),
                null!,
                CreateFamilyMemberManagementViewModel(uiTextProvider)));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_rejects_null_family_member_management_view_model()
    {
        var uiTextProvider = CreateUiTextProvider();
        var exception = Record.Exception(
            () => new ProductShellViewModel(
                uiTextProvider,
                CreateDocumentRegistrationViewModel(uiTextProvider),
                CreateDocumentListViewModel(uiTextProvider),
                CreatePolicyClaimManagementViewModel(uiTextProvider),
                null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Constructor_resolves_shell_title()
    {
        var viewModel = CreateViewModel();

        Assert.Equal("FamilyClaimRef", viewModel.ShellTitle);
    }

    [Fact]
    public void Navigation_items_have_expected_count_order_ids_and_display_text()
    {
        var viewModel = CreateViewModel();

        Assert.Collection(
            viewModel.NavigationItems,
            item =>
            {
                Assert.Equal("Home", item.Id);
                Assert.Equal("Home display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("PolicyContracts", item.Id);
                Assert.Equal("Policy display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("ClaimCases", item.Id);
                Assert.Equal("Claim display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("DocumentRegistration", item.Id);
                Assert.Equal("Registration display", item.DisplayText);
            },
            item =>
            {
                Assert.Equal("DocumentList", item.Id);
                Assert.Equal("List display", item.DisplayText);
            });
    }

    [Fact]
    public void Initial_selection_is_home()
    {
        var viewModel = CreateViewModel();

        Assert.Same(viewModel.NavigationItems[0], viewModel.SelectedNavigationItem);
        Assert.Equal("Home", viewModel.SelectedNavigationItem!.Id);
        Assert.Equal(ProductScreenRoutes.HomeDashboard, viewModel.CurrentRouteId);
        Assert.Same(viewModel.Screens[0], viewModel.CurrentScreen);
    }

    [Fact]
    public void Selection_change_updates_route_and_raises_PropertyChanged()
    {
        var viewModel = CreateViewModel();
        var propertyNames = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyNames.Add(args.PropertyName);

        viewModel.SelectedNavigationItem = viewModel.NavigationItems[1];

        Assert.Same(viewModel.NavigationItems[1], viewModel.SelectedNavigationItem);
        Assert.Equal(ProductScreenRoutes.PolicyManage, viewModel.CurrentRouteId);
        Assert.Contains(nameof(ProductShellViewModel.SelectedNavigationItem), propertyNames);
        Assert.Contains(nameof(ProductShellViewModel.CurrentScreen), propertyNames);
        Assert.Contains(nameof(ProductShellViewModel.CurrentRouteId), propertyNames);
    }

    [Fact]
    public void Assigning_same_selection_does_not_raise_PropertyChanged()
    {
        var viewModel = CreateViewModel();
        var notificationCount = 0;
        viewModel.PropertyChanged += (_, _) => notificationCount++;

        viewModel.SelectedNavigationItem = viewModel.SelectedNavigationItem;

        Assert.Equal(0, notificationCount);
    }

    [Fact]
    public void Null_selection_is_ignored()
    {
        var viewModel = CreateViewModel();
        var initialSelection = viewModel.SelectedNavigationItem;

        viewModel.SelectedNavigationItem = null;

        Assert.Same(initialSelection, viewModel.SelectedNavigationItem);
    }

    [Fact]
    public void Foreign_navigation_item_is_rejected()
    {
        var viewModel = CreateViewModel();
        var foreignItem = new ProductNavigationItemViewModel("Foreign", "Foreign display");

        var exception = Record.Exception(() => viewModel.SelectedNavigationItem = foreignItem);

        Assert.IsType<ArgumentException>(exception);
        Assert.Same(viewModel.NavigationItems[0], viewModel.SelectedNavigationItem);
    }

    [Fact]
    public void Constructor_dependencies_match_approved_composition_contract()
    {
        var constructor = Assert.Single(typeof(ProductShellViewModel).GetConstructors());
        var parameters = constructor.GetParameters();

        Assert.Collection(
            parameters,
            parameter => Assert.Equal(typeof(IUiTextProvider), parameter.ParameterType),
            parameter => Assert.Equal(typeof(DocumentRegistrationViewModel), parameter.ParameterType),
            parameter => Assert.Equal(typeof(ProductDocumentListViewModel), parameter.ParameterType),
            parameter => Assert.Equal(typeof(PolicyClaimManagementViewModel), parameter.ParameterType),
            parameter => Assert.Equal(typeof(FamilyMemberManagementViewModel), parameter.ParameterType));
    }

    [Fact]
    public void Screens_cover_all_approved_wireframe_routes_in_order()
    {
        var viewModel = CreateViewModel();

        Assert.Equal(21, viewModel.Screens.Count);
        Assert.Equal(ProductScreenRoutes.All, viewModel.Screens.Select(screen => screen.Id));
        Assert.Equal(
            Enumerable.Range(1, 21).Select(number => number.ToString("00")),
            viewModel.Screens.Select(screen => screen.WireframeNumber));
    }

    [Fact]
    public void Claim_flow_routes_have_shared_ordered_steps_and_context_without_raw_ids()
    {
        var viewModel = CreateViewModel();
        var expected = new[]
        {
            (ProductScreenRoutes.ClaimCase, 1),
            (ProductScreenRoutes.OcrReview, 2),
            (ProductScreenRoutes.ClaimReferenceResult, 3),
            (ProductScreenRoutes.ClaimSubmission, 4),
            (ProductScreenRoutes.ClaimComplete, 5)
        };

        foreach (var (routeId, stepNumber) in expected)
        {
            var screen = Assert.Single(viewModel.Screens, candidate => candidate.Id == routeId);
            Assert.True(screen.ShowClaimFlow);
            Assert.Equal(stepNumber, screen.ClaimStepNumber);
        }

        Assert.Equal(ProductScreenTextKeys.EmptyValue, viewModel.ClaimContextPolicyDisplayTitle);
        Assert.Equal(ProductScreenTextKeys.EmptyValue, viewModel.ClaimContextClaimDisplayTitle);
        Assert.DoesNotContain("Id", viewModel.ClaimContextPolicyDisplayTitle, StringComparison.Ordinal);
        Assert.DoesNotContain("Id", viewModel.ClaimContextClaimDisplayTitle, StringComparison.Ordinal);
    }

    [Fact]
    public void Unsupported_presentation_commands_are_explicitly_disabled()
    {
        var viewModel = CreateViewModel();
        var unsupportedRoutes = new[]
        {
            ProductScreenRoutes.PolicyRegister,
            ProductScreenRoutes.FamilyRegister,
            ProductScreenRoutes.CategoryRegister,
            ProductScreenRoutes.CategoryItemRegister
        };

        foreach (var routeId in unsupportedRoutes)
        {
            var screen = Assert.Single(viewModel.Screens, candidate => candidate.Id == routeId);
            Assert.True(screen.HasDeferredCommands);
            Assert.Contains(screen.Commands, command => !command.IsEnabled && command.RouteId is null);
            Assert.Contains(screen.Commands, command => command.IsEnabled && command.RouteId is not null);
        }
    }

    [Fact]
    public void Navigate_to_registration_routes_configures_target_kind_without_replacing_child()
    {
        var viewModel = CreateViewModel();
        var child = viewModel.DocumentRegistration;

        viewModel.NavigateTo(ProductScreenRoutes.ClaimDocumentRegister);
        Assert.Equal(DocumentRegistrationViewModel.ClaimTargetKind, child.TargetKind);
        Assert.Same(child, viewModel.DocumentRegistration);
        Assert.Equal("DocumentRegistration", viewModel.SelectedNavigationItem!.Id);

        viewModel.NavigateTo(ProductScreenRoutes.PolicyDocumentRegister);
        Assert.Equal(DocumentRegistrationViewModel.PolicyTargetKind, child.TargetKind);
        Assert.Same(child, viewModel.DocumentRegistration);
        Assert.Equal("DocumentRegistration", viewModel.SelectedNavigationItem!.Id);
    }

    [Fact]
    public void Policy_document_action_presets_policy_target_and_document_type()
    {
        var viewModel = CreateViewModel();
        viewModel.PolicyClaimManagement.SelectedPolicyId = "policy_synthetic";

        Assert.True(viewModel.NavigateToPolicyDocumentRegistration("terms"));

        Assert.Equal(ProductScreenRoutes.PolicyDocumentRegister, viewModel.CurrentScreen.Id);
        Assert.Equal(DocumentRegistrationViewModel.PolicyTargetKind, viewModel.DocumentRegistration.TargetKind);
        Assert.Equal("policy_synthetic", viewModel.DocumentRegistration.SelectedPolicyId);
        Assert.Equal("terms", viewModel.DocumentRegistration.DocumentType);
    }

    [Fact]
    public void Presentation_input_is_retained_when_navigating_away_and_back()
    {
        var viewModel = CreateViewModel();
        viewModel.NavigateTo(ProductScreenRoutes.ClaimCase);
        var field = Assert.Single(
            viewModel.CurrentScreen.Fields,
            candidate => candidate.Label.Contains("메모", StringComparison.Ordinal)
                || candidate.Label.EndsWith("Fields", StringComparison.Ordinal));
        field.Value = "retained presentation value";

        viewModel.NavigateTo(ProductScreenRoutes.HomeDashboard);
        viewModel.NavigateTo(ProductScreenRoutes.ClaimCase);

        Assert.Equal("retained presentation value", field.Value);
        Assert.Same(field, viewModel.CurrentScreen.Fields.Single(candidate => candidate.Label == field.Label));
    }

    [Fact]
    public void Unknown_route_is_rejected_without_changing_current_screen()
    {
        var viewModel = CreateViewModel();
        var initial = viewModel.CurrentScreen;

        Assert.Throws<ArgumentException>(() => viewModel.NavigateTo("unknown"));
        Assert.Same(initial, viewModel.CurrentScreen);
    }

    [Fact]
    public void Family_register_catalog_enables_save_and_deactivate_but_keeps_delete_disabled()
    {
        var viewModel = CreateViewModel();
        var screen = viewModel.Screens.Single(candidate =>
            candidate.Id == ProductScreenRoutes.FamilyRegister);

        Assert.True(screen.Commands[0].IsEnabled);
        Assert.False(screen.Commands[1].IsEnabled);
        Assert.True(screen.Commands[2].IsEnabled);
        Assert.True(screen.Commands[3].IsEnabled);
    }

    [Fact]
    public async Task Family_edit_navigation_uses_explicit_id_and_direct_route_resets_to_create_mode()
    {
        var uiTextProvider = CreateUiTextProvider();
        var testRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(testRoot, "data", "local");

        try
        {
            var storage = new JsonFamilyMemberStorageService(metadataRoot);
            var record = await storage.CreateFamilyMemberAsync(new FamilyMemberDraft(
                "synthetic family",
                FamilyMemberRelationValues.Mother,
                null));
            var family = new FamilyMemberManagementViewModel(storage, uiTextProvider);
            var viewModel = new ProductShellViewModel(
                uiTextProvider,
                CreateDocumentRegistrationViewModel(uiTextProvider),
                CreateDocumentListViewModel(uiTextProvider),
                CreatePolicyClaimManagementViewModel(uiTextProvider),
                family);

            Assert.True(await viewModel.NavigateToFamilyEditAsync(record.Id, record.Version));
            Assert.Equal(ProductScreenRoutes.FamilyRegister, viewModel.CurrentRouteId);
            Assert.True(family.IsEditMode);
            Assert.Equal(record.Id, family.EditingTargetId);

            viewModel.NavigateTo(ProductScreenRoutes.FamilyRegister);

            Assert.False(family.IsEditMode);
            Assert.Null(family.EditingTargetId);
            Assert.False(family.CanDeactivate);
        }
        finally
        {
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Family_create_and_edit_save_return_to_refreshed_family_list()
    {
        var uiTextProvider = CreateUiTextProvider();
        var testRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(testRoot, "data", "local");

        try
        {
            var storage = new JsonFamilyMemberStorageService(metadataRoot);
            var family = new FamilyMemberManagementViewModel(storage, uiTextProvider);
            var viewModel = CreateViewModel(uiTextProvider, family);

            viewModel.NavigateToFamilyCreate();
            family.DisplayName = "synthetic family";
            family.SelectedRelation = FamilyMemberRelationValues.Mother;
            family.Memo = "synthetic memo";

            Assert.True(await viewModel.SaveFamilyMemberAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.FamilyMembers, viewModel.CurrentRouteId);
            var created = Assert.Single(family.AvailableMembers);
            Assert.Equal(FamilyMemberRelationValues.Mother, created.Relation);

            Assert.True(await viewModel.NavigateToFamilyEditAsync(created.Id, created.Version));
            family.DisplayName = "updated synthetic family";

            Assert.True(await viewModel.SaveFamilyMemberAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.FamilyMembers, viewModel.CurrentRouteId);
            var updated = Assert.Single(family.AvailableMembers);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal("updated synthetic family", updated.DisplayName);
            Assert.Equal(2, updated.Version);

            var persisted = Assert.Single(await storage.GetActiveFamilyMembersAsync());
            Assert.Equal(updated, persisted);
        }
        finally
        {
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Family_save_failure_keeps_editor_route_and_does_not_write()
    {
        var uiTextProvider = CreateUiTextProvider();
        var metadataRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"),
            "data",
            "local");
        var storage = new JsonFamilyMemberStorageService(metadataRoot);
        var family = new FamilyMemberManagementViewModel(storage, uiTextProvider);
        var viewModel = CreateViewModel(uiTextProvider, family);

        viewModel.NavigateToFamilyCreate();
        family.DisplayName = "synthetic family";
        family.SelectedRelation = "가족 후보";

        Assert.False(await viewModel.SaveFamilyMemberAndReturnAsync());
        Assert.Equal(ProductScreenRoutes.FamilyRegister, viewModel.CurrentRouteId);
        Assert.Empty(await storage.GetActiveFamilyMembersAsync());
        Assert.False(File.Exists(Path.Combine(
            metadataRoot,
            JsonFamilyMemberStorageService.StoreFileName)));
    }

    [Fact]
    public async Task Insurance_create_and_edit_save_return_to_refreshed_policy_list()
    {
        var uiTextProvider = CreateUiTextProvider();
        var testRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(testRoot, "data", "local");

        try
        {
            var familyStorage = new JsonFamilyMemberStorageService(metadataRoot);
            var family = await familyStorage.CreateFamilyMemberAsync(new FamilyMemberDraft(
                "synthetic family",
                FamilyMemberRelationValues.Self,
                null));
            var policyStorage = new JsonPolicyClaimStorageService(metadataRoot, familyStorage);
            var management = new PolicyClaimManagementViewModel(
                policyStorage,
                familyStorage,
                uiTextProvider);
            var shell = CreateViewModel(
                uiTextProvider,
                new FamilyMemberManagementViewModel(familyStorage, uiTextProvider),
                management);

            Assert.True(await shell.NavigateToInsurancePolicyCreateAsync());
            Assert.Equal(ProductScreenRoutes.PolicyRegister, shell.CurrentRouteId);
            FillInsuranceEditor(management, family.Id, "synthetic policy");

            Assert.True(await shell.SaveInsurancePolicyAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.PolicyManage, shell.CurrentRouteId);
            var created = Assert.Single(management.AvailableInsurancePolicies);
            Assert.Equal(created.Policy, Assert.Single(management.AvailablePolicies));
            Assert.Equal("synthetic policy", created.DisplayTitle);

            Assert.True(await shell.NavigateToInsurancePolicyEditAsync(created.Id));
            Assert.Equal(ProductScreenRoutes.PolicyRegister, shell.CurrentRouteId);
            Assert.Equal(created.Id, management.SelectedPolicyId);
            management.InsuranceDisplayTitle = "updated synthetic policy";

            Assert.True(await shell.SaveInsurancePolicyAndReturnAsync());
            Assert.Equal(ProductScreenRoutes.PolicyManage, shell.CurrentRouteId);
            var updated = Assert.Single(management.AvailableInsurancePolicies);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal("updated synthetic policy", updated.DisplayTitle);
            Assert.Equal(updated.Policy, Assert.Single(await policyStorage.GetPoliciesAsync()));
        }
        finally
        {
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Insurance_save_failure_stays_on_editor_and_cancel_writes_nothing()
    {
        var uiTextProvider = CreateUiTextProvider();
        var metadataRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"),
            "data",
            "local");
        var familyStorage = new JsonFamilyMemberStorageService(metadataRoot);
        var policyStorage = new JsonPolicyClaimStorageService(metadataRoot, familyStorage);
        var family = new FamilyMemberManagementViewModel(familyStorage, uiTextProvider);
        var management = new PolicyClaimManagementViewModel(
            policyStorage,
            familyStorage,
            uiTextProvider);
        var shell = CreateViewModel(uiTextProvider, family, management);

        Assert.True(await shell.NavigateToInsurancePolicyCreateAsync());
        Assert.False(await shell.SaveInsurancePolicyAndReturnAsync());
        Assert.Equal(ProductScreenRoutes.PolicyRegister, shell.CurrentRouteId);

        shell.NavigateTo(ProductScreenRoutes.PolicyManage);

        Assert.Equal(ProductScreenRoutes.PolicyManage, shell.CurrentRouteId);
        Assert.Empty(await policyStorage.GetPoliciesAsync());
        Assert.False(File.Exists(Path.Combine(metadataRoot, "policies.json")));
    }

    [Fact]
    public void Document_registration_property_exposes_injected_instance()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);
        var documentList = CreateDocumentListViewModel(uiTextProvider);
        var policyClaimManagement = CreatePolicyClaimManagementViewModel(uiTextProvider);
        var familyMemberManagement = CreateFamilyMemberManagementViewModel(uiTextProvider);

        var viewModel = new ProductShellViewModel(
            uiTextProvider,
            documentRegistration,
            documentList,
            policyClaimManagement,
            familyMemberManagement);

        Assert.Same(documentRegistration, viewModel.DocumentRegistration);
    }

    [Fact]
    public void Document_list_property_exposes_injected_instance()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);
        var documentList = CreateDocumentListViewModel(uiTextProvider);
        var policyClaimManagement = CreatePolicyClaimManagementViewModel(uiTextProvider);
        var familyMemberManagement = CreateFamilyMemberManagementViewModel(uiTextProvider);

        var viewModel = new ProductShellViewModel(
            uiTextProvider,
            documentRegistration,
            documentList,
            policyClaimManagement,
            familyMemberManagement);

        Assert.Same(documentList, viewModel.DocumentList);
    }

    [Fact]
    public void Policy_claim_management_property_exposes_injected_instance()
    {
        var uiTextProvider = CreateUiTextProvider();
        var documentRegistration = CreateDocumentRegistrationViewModel(uiTextProvider);
        var documentList = CreateDocumentListViewModel(uiTextProvider);
        var policyClaimManagement = CreatePolicyClaimManagementViewModel(uiTextProvider);
        var familyMemberManagement = CreateFamilyMemberManagementViewModel(uiTextProvider);

        var viewModel = new ProductShellViewModel(
            uiTextProvider,
            documentRegistration,
            documentList,
            policyClaimManagement,
            familyMemberManagement);

        Assert.Same(policyClaimManagement, viewModel.PolicyClaimManagement);
    }

    [Fact]
    public void Family_member_management_property_exposes_injected_instance()
    {
        var uiTextProvider = CreateUiTextProvider();
        var familyMemberManagement = CreateFamilyMemberManagementViewModel(uiTextProvider);
        var viewModel = new ProductShellViewModel(
            uiTextProvider,
            CreateDocumentRegistrationViewModel(uiTextProvider),
            CreateDocumentListViewModel(uiTextProvider),
            CreatePolicyClaimManagementViewModel(uiTextProvider),
            familyMemberManagement);

        Assert.Same(familyMemberManagement, viewModel.FamilyMemberManagement);
    }

    private static ProductShellViewModel CreateViewModel()
    {
        var uiTextProvider = CreateUiTextProvider();
        return new ProductShellViewModel(
            uiTextProvider,
            CreateDocumentRegistrationViewModel(uiTextProvider),
            CreateDocumentListViewModel(uiTextProvider),
            CreatePolicyClaimManagementViewModel(uiTextProvider),
            CreateFamilyMemberManagementViewModel(uiTextProvider));
    }

    private static ProductShellViewModel CreateViewModel(
        IUiTextProvider uiTextProvider,
        FamilyMemberManagementViewModel familyMemberManagement)
    {
        return new ProductShellViewModel(
            uiTextProvider,
            CreateDocumentRegistrationViewModel(uiTextProvider),
            CreateDocumentListViewModel(uiTextProvider),
            CreatePolicyClaimManagementViewModel(uiTextProvider),
            familyMemberManagement);
    }

    private static ProductShellViewModel CreateViewModel(
        IUiTextProvider uiTextProvider,
        FamilyMemberManagementViewModel familyMemberManagement,
        PolicyClaimManagementViewModel policyClaimManagement)
    {
        return new ProductShellViewModel(
            uiTextProvider,
            CreateDocumentRegistrationViewModel(uiTextProvider),
            CreateDocumentListViewModel(uiTextProvider),
            policyClaimManagement,
            familyMemberManagement);
    }

    private static void FillInsuranceEditor(
        PolicyClaimManagementViewModel management,
        string familyMemberId,
        string displayTitle)
    {
        management.InsuranceDisplayTitle = displayTitle;
        management.SelectedInsuranceFamilyMemberId = familyMemberId;
        management.InsuranceInsurerName = "synthetic insurer";
        management.InsuranceContractStatus = InsurancePolicyValues.ContractStatusActive;
        management.InsuranceEnrollmentDate = new DateTime(2026, 8, 4);
        management.InsuranceCoveragePeriod = "2026-2027";
        management.InsurancePremiumPaymentPeriod = "20년납";
        management.InsuranceTotalPlannedPremiumAmountText = "12,000,000";
        management.InsuranceRenewalType = InsurancePolicyValues.RenewalTypeFixed;
        management.InsuranceRefundType = InsurancePolicyValues.RefundTypeRefundable;
        management.InsuranceBusinessType = InsurancePolicyValues.BusinessTypeLife;
        management.InsuranceProductCategory = InsurancePolicyValues.ProductCategoryCancer;
    }

    private static FamilyMemberManagementViewModel CreateFamilyMemberManagementViewModel(
        IUiTextProvider uiTextProvider)
    {
        var metadataRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"),
            "data",
            "local");

        return new FamilyMemberManagementViewModel(
            new JsonFamilyMemberStorageService(metadataRoot),
            uiTextProvider);
    }

    private static PolicyClaimManagementViewModel CreatePolicyClaimManagementViewModel(
        IUiTextProvider uiTextProvider)
    {
        var metadataRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"),
            "data",
            "local");

        return new PolicyClaimManagementViewModel(
            new JsonPolicyClaimStorageService(metadataRoot),
            uiTextProvider);
    }

    private static ProductDocumentListViewModel CreateDocumentListViewModel(
        IUiTextProvider uiTextProvider)
    {
        var metadataRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"),
            "data",
            "local");

        return new ProductDocumentListViewModel(
            new JsonDocumentStorageService(metadataRoot),
            uiTextProvider);
    }

    private static DocumentRegistrationViewModel CreateDocumentRegistrationViewModel(
        IUiTextProvider uiTextProvider)
    {
        var testRoot = Path.Combine(
            Path.GetTempPath(),
            "FamilyClaimRef.App.Tests",
            "ProductShellViewModelTests",
            Guid.NewGuid().ToString("N"));
        var metadataRoot = Path.Combine(testRoot, "data", "local");
        var attachmentRoot = Path.Combine(testRoot, "attachments");
        var documentStorage = new JsonDocumentStorageService(metadataRoot);
        var policyClaimStorage = new JsonPolicyClaimStorageService(metadataRoot);
        var fileAttachment = new LocalFileAttachmentService(attachmentRoot);
        var workflow = new DocumentRegistrationWorkflow(
            new DocumentAttachmentCoordinator(documentStorage, fileAttachment),
            new DocumentLinkCoordinator(documentStorage, policyClaimStorage),
            documentStorage,
            fileAttachment);

        return new DocumentRegistrationViewModel(
            workflow,
            new WpfFilePickerService(),
            policyClaimStorage,
            uiTextProvider);
    }

    private static IUiTextProvider CreateUiTextProvider()
    {
        return new FakeUiTextProvider(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [UiTextKeys.ProductShellTitle] = "FamilyClaimRef",
            [UiTextKeys.ProductNavigationHome] = "Home display",
            [UiTextKeys.ProductNavigationPolicyContracts] = "Policy display",
            [UiTextKeys.ProductNavigationClaimCases] = "Claim display",
            [UiTextKeys.ProductNavigationDocumentRegistration] = "Registration display",
            [UiTextKeys.ProductNavigationDocumentList] = "List display",
            [UiTextKeys.ProductDocumentListTitle] = "List title",
            [UiTextKeys.ProductDocumentListEmptyMessage] = "List empty",
            [UiTextKeys.ProductDocumentListLoadFailedMessage] = "List failed"
        });
    }

    private sealed class FakeUiTextProvider(IReadOnlyDictionary<string, string> values) : IUiTextProvider
    {
        public string Get(string key)
        {
            return values.TryGetValue(key, out var value) ? value : key;
        }

        public string Format(string key, params object?[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
