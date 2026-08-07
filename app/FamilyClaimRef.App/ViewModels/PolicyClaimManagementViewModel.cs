using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;

namespace FamilyClaimRef.App.ViewModels;

public sealed class PolicyClaimManagementViewModel : INotifyPropertyChanged
{
    private const string CaptureDocumentType = "capture";
    private const string PolicyDocumentType = "policy";
    private const string TermsDocumentType = "terms";

    private readonly IPolicyClaimStorageService policyClaimStorageService;
    private readonly IFamilyMemberStorageService? familyMemberStorageService;
    private readonly IDocumentStorageService? documentStorageService;
    private readonly IManagedDocumentOpener? managedDocumentOpener;
    private readonly IUiTextProvider uiTextProvider;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private readonly Dictionary<string, DocumentRecord> activeInsuranceDocumentsByType =
        new(StringComparer.Ordinal);

    private IReadOnlyList<PolicyRecord> availablePolicies = [];
    private IReadOnlyList<ClaimRecord> availableClaims = [];
    private string? selectedPolicyId;
    private string? selectedClaimId;
    private string? selectedPolicyForClaimId;
    private string? newPolicyDisplayTitle;
    private string? newClaimDisplayTitle;
    private string? managementMessage;
    private IReadOnlyList<InsurancePolicyListItemViewModel> availableInsurancePolicies = [];
    private IReadOnlyList<FamilyMemberRecord> availablePolicyFamilyMembers = [];
    private string? editingInsurancePolicyId;
    private string? insuranceDisplayTitle;
    private string? selectedInsuranceFamilyMemberId;
    private string? insuranceInsurerName;
    private string? insuranceContractStatus;
    private DateTime? insuranceEnrollmentDate;
    private string? insuranceCoveragePeriod;
    private string? insurancePremiumPaymentPeriod;
    private string? insuranceTotalPlannedPremiumAmountText;
    private string? insuranceRenewalType;
    private string? insuranceRefundType;
    private string? insuranceBusinessType;
    private string? insuranceProductCategory;
    private string? insuranceRegistrationSource;
    private bool insuranceContractStatusNeedsReview;
    private string? insurancePolicyMessage;
    private string insuranceCaptureDocumentStatus;
    private string insurancePolicyDocumentStatus;
    private string insuranceTermsDocumentStatus;
    private IReadOnlyList<InsurancePolicyDocumentHistoryItemViewModel>
        insurancePolicyDocumentHistory = [];

    public PolicyClaimManagementViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        IUiTextProvider uiTextProvider)
        : this(policyClaimStorageService, null, null, uiTextProvider)
    {
    }

    public PolicyClaimManagementViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        IFamilyMemberStorageService? familyMemberStorageService,
        IUiTextProvider uiTextProvider)
        : this(policyClaimStorageService, familyMemberStorageService, null, uiTextProvider)
    {
    }

    public PolicyClaimManagementViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        IFamilyMemberStorageService? familyMemberStorageService,
        IDocumentStorageService? documentStorageService,
        IUiTextProvider uiTextProvider)
        : this(
            policyClaimStorageService,
            familyMemberStorageService,
            documentStorageService,
            null,
            uiTextProvider)
    {
    }

    public PolicyClaimManagementViewModel(
        IPolicyClaimStorageService policyClaimStorageService,
        IFamilyMemberStorageService? familyMemberStorageService,
        IDocumentStorageService? documentStorageService,
        IManagedDocumentOpener? managedDocumentOpener,
        IUiTextProvider uiTextProvider)
    {
        this.policyClaimStorageService = policyClaimStorageService
            ?? throw new ArgumentNullException(nameof(policyClaimStorageService));
        this.familyMemberStorageService = familyMemberStorageService;
        this.documentStorageService = documentStorageService;
        this.managedDocumentOpener = managedDocumentOpener;
        this.uiTextProvider = uiTextProvider
            ?? throw new ArgumentNullException(nameof(uiTextProvider));
        insuranceCaptureDocumentStatus = uiTextProvider.Get(
            UiTextKeys.ProductInsurancePolicyDocumentCreateStatus);
        insurancePolicyDocumentStatus = uiTextProvider.Get(
            UiTextKeys.ProductInsurancePolicyDocumentCreateStatus);
        insuranceTermsDocumentStatus = uiTextProvider.Get(
            UiTextKeys.ProductInsurancePolicyDocumentCreateStatus);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<PolicyRecord> AvailablePolicies
    {
        get => availablePolicies;
        private set
        {
            if (SetProperty(ref availablePolicies, value))
            {
                OnPropertyChanged(nameof(HasAvailablePolicies));
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public IReadOnlyList<ClaimRecord> AvailableClaims
    {
        get => availableClaims;
        private set
        {
            if (SetProperty(ref availableClaims, value))
            {
                OnPropertyChanged(nameof(HasAvailableClaims));
            }
        }
    }

    public string? SelectedPolicyId
    {
        get => selectedPolicyId;
        set
        {
            if (SetProperty(ref selectedPolicyId, value))
            {
                OnPropertyChanged(nameof(CanDisablePolicy));
            }
        }
    }

    public string? SelectedClaimId
    {
        get => selectedClaimId;
        set
        {
            if (SetProperty(ref selectedClaimId, value))
            {
                OnPropertyChanged(nameof(CanDisableClaim));
            }
        }
    }

    public string? SelectedPolicyForClaimId
    {
        get => selectedPolicyForClaimId;
        set
        {
            if (SetProperty(ref selectedPolicyForClaimId, value))
            {
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public string? NewPolicyDisplayTitle
    {
        get => newPolicyDisplayTitle;
        set
        {
            if (SetProperty(ref newPolicyDisplayTitle, value))
            {
                OnPropertyChanged(nameof(CanCreatePolicy));
            }
        }
    }

    public string? NewClaimDisplayTitle
    {
        get => newClaimDisplayTitle;
        set
        {
            if (SetProperty(ref newClaimDisplayTitle, value))
            {
                OnPropertyChanged(nameof(CanCreateClaim));
            }
        }
    }

    public string? ManagementMessage
    {
        get => managementMessage;
        private set => SetProperty(ref managementMessage, value);
    }

    public bool HasAvailablePolicies => AvailablePolicies.Count > 0;

    public bool HasAvailableClaims => AvailableClaims.Count > 0;

    public bool CanCreatePolicy => !string.IsNullOrWhiteSpace(NewPolicyDisplayTitle);

    public bool CanCreateClaim =>
        !string.IsNullOrWhiteSpace(NewClaimDisplayTitle)
        && !string.IsNullOrWhiteSpace(SelectedPolicyForClaimId);

    public bool CanDisablePolicy => !string.IsNullOrWhiteSpace(SelectedPolicyId);

    public bool CanDisableClaim => !string.IsNullOrWhiteSpace(SelectedClaimId);

    public IReadOnlyList<InsurancePolicyListItemViewModel> AvailableInsurancePolicies
    {
        get => availableInsurancePolicies;
        private set
        {
            if (SetProperty(ref availableInsurancePolicies, value))
            {
                OnPropertyChanged(nameof(HasAvailableInsurancePolicies));
            }
        }
    }

    public IReadOnlyList<FamilyMemberRecord> AvailablePolicyFamilyMembers
    {
        get => availablePolicyFamilyMembers;
        private set => SetProperty(ref availablePolicyFamilyMembers, value);
    }

    public string? InsuranceDisplayTitle
    {
        get => insuranceDisplayTitle;
        set
        {
            if (SetProperty(ref insuranceDisplayTitle, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? SelectedInsuranceFamilyMemberId
    {
        get => selectedInsuranceFamilyMemberId;
        set
        {
            if (SetProperty(ref selectedInsuranceFamilyMemberId, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceInsurerName
    {
        get => insuranceInsurerName;
        set
        {
            if (SetProperty(ref insuranceInsurerName, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceContractStatus
    {
        get => insuranceContractStatus;
        set
        {
            if (SetProperty(ref insuranceContractStatus, value))
            {
                insuranceContractStatusNeedsReview = false;
                OnPropertyChanged(nameof(InsuranceContractStatusGuidance));
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public DateTime? InsuranceEnrollmentDate
    {
        get => insuranceEnrollmentDate;
        set
        {
            if (SetProperty(ref insuranceEnrollmentDate, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceCoveragePeriod
    {
        get => insuranceCoveragePeriod;
        set
        {
            if (SetProperty(ref insuranceCoveragePeriod, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsurancePremiumPaymentPeriod
    {
        get => insurancePremiumPaymentPeriod;
        set
        {
            if (SetProperty(ref insurancePremiumPaymentPeriod, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceTotalPlannedPremiumAmountText
    {
        get => insuranceTotalPlannedPremiumAmountText;
        set
        {
            if (SetProperty(ref insuranceTotalPlannedPremiumAmountText, value))
            {
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceRenewalType
    {
        get => insuranceRenewalType;
        set
        {
            if (SetProperty(ref insuranceRenewalType, value))
            {
                OnPropertyChanged(nameof(InsuranceRenewalTypeGuidance));
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceRefundType
    {
        get => insuranceRefundType;
        set
        {
            if (SetProperty(ref insuranceRefundType, value))
            {
                OnPropertyChanged(nameof(InsuranceRefundTypeGuidance));
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceBusinessType
    {
        get => insuranceBusinessType;
        set
        {
            if (SetProperty(ref insuranceBusinessType, value))
            {
                OnPropertyChanged(nameof(InsuranceBusinessTypeGuidance));
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceProductCategory
    {
        get => insuranceProductCategory;
        set
        {
            if (SetProperty(ref insuranceProductCategory, value))
            {
                OnPropertyChanged(nameof(InsuranceProductCategoryGuidance));
                OnInsurancePolicyCommandStateChanged();
            }
        }
    }

    public string? InsuranceRegistrationSource
    {
        get => insuranceRegistrationSource;
        private set => SetProperty(ref insuranceRegistrationSource, value);
    }

    public IReadOnlyList<string> InsuranceContractStatusOptions =>
        InsurancePolicyValues.ContractStatuses;

    public IReadOnlyList<string> InsuranceRenewalTypeOptions => InsurancePolicyValues.RenewalTypes;

    public IReadOnlyList<string> InsuranceRefundTypeOptions => InsurancePolicyValues.RefundTypes;

    public IReadOnlyList<string> InsuranceBusinessTypeOptions => InsurancePolicyValues.BusinessTypes;

    public IReadOnlyList<string> InsuranceProductCategoryOptions =>
        InsurancePolicyValues.ProductCategories;

    public string? InsuranceContractStatusGuidance => InsuranceContractStatus is not null
        ? null
        : uiTextProvider.Get(insuranceContractStatusNeedsReview
            ? UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired
            : UiTextKeys.ProductInsurancePolicySelectionRequired);

    public string? InsuranceRenewalTypeGuidance => GetSelectionGuidance(InsuranceRenewalType);

    public string? InsuranceRefundTypeGuidance => GetSelectionGuidance(InsuranceRefundType);

    public string? InsuranceBusinessTypeGuidance => GetSelectionGuidance(InsuranceBusinessType);

    public string? InsuranceProductCategoryGuidance => GetSelectionGuidance(InsuranceProductCategory);

    public string? InsurancePolicyMessage
    {
        get => insurancePolicyMessage;
        private set => SetProperty(ref insurancePolicyMessage, value);
    }

    public string InsuranceCaptureDocumentStatus
    {
        get => insuranceCaptureDocumentStatus;
        private set => SetProperty(ref insuranceCaptureDocumentStatus, value);
    }

    public string InsurancePolicyDocumentStatus
    {
        get => insurancePolicyDocumentStatus;
        private set => SetProperty(ref insurancePolicyDocumentStatus, value);
    }

    public string InsuranceTermsDocumentStatus
    {
        get => insuranceTermsDocumentStatus;
        private set => SetProperty(ref insuranceTermsDocumentStatus, value);
    }

    public IReadOnlyList<InsurancePolicyDocumentHistoryItemViewModel>
        InsurancePolicyDocumentHistory
    {
        get => insurancePolicyDocumentHistory;
        private set
        {
            if (SetProperty(ref insurancePolicyDocumentHistory, value))
            {
                OnPropertyChanged(nameof(HasInsurancePolicyDocumentHistory));
                OnPropertyChanged(nameof(InsurancePolicyDocumentHistoryTitle));
            }
        }
    }

    public bool HasInsurancePolicyDocumentHistory =>
        InsurancePolicyDocumentHistory.Count > 0;

    public string InsurancePolicyDocumentHistoryTitle => uiTextProvider.Format(
        UiTextKeys.ProductInsurancePolicyDocumentHistoryHeaderFormat,
        InsurancePolicyDocumentHistory.Count);

    public bool HasInsuranceCaptureDocument =>
        HasInsurancePolicyDocumentType(CaptureDocumentType);

    public bool HasInsurancePolicyDocument =>
        HasInsurancePolicyDocumentType(PolicyDocumentType);

    public bool HasInsuranceTermsDocument =>
        HasInsurancePolicyDocumentType(TermsDocumentType);

    public bool HasAvailableInsurancePolicies => AvailableInsurancePolicies.Count > 0;

    public bool IsInsurancePolicyEditMode => editingInsurancePolicyId is not null;

    public bool CanSaveInsurancePolicy =>
        familyMemberStorageService is not null
        && !string.IsNullOrWhiteSpace(InsuranceDisplayTitle)
        && !string.IsNullOrWhiteSpace(SelectedInsuranceFamilyMemberId)
        && !string.IsNullOrWhiteSpace(InsuranceInsurerName)
        && IsAllowedValue(InsuranceContractStatus, InsurancePolicyValues.ContractStatuses)
        && InsuranceEnrollmentDate is not null
        && !string.IsNullOrWhiteSpace(InsuranceCoveragePeriod)
        && !string.IsNullOrWhiteSpace(InsurancePremiumPaymentPeriod)
        && TryParsePlannedPremiumAmount(out _)
        && IsAllowedValue(InsuranceRenewalType, InsurancePolicyValues.RenewalTypes)
        && IsAllowedValue(InsuranceRefundType, InsurancePolicyValues.RefundTypes)
        && IsAllowedValue(InsuranceBusinessType, InsurancePolicyValues.BusinessTypes)
        && IsAllowedValue(InsuranceProductCategory, InsurancePolicyValues.ProductCategories);

    public void ClearManagementMessage()
    {
        ManagementMessage = null;
    }

    public void ClearInsurancePolicyMessage()
    {
        InsurancePolicyMessage = null;
    }

    public bool HasInsurancePolicyDocumentType(string documentType) =>
        activeInsuranceDocumentsByType.ContainsKey(documentType);

    public async Task<bool> OpenInsurancePolicyDocumentAsync(
        string documentType,
        CancellationToken cancellationToken = default)
    {
        if (managedDocumentOpener is null
            || !activeInsuranceDocumentsByType.TryGetValue(documentType, out var document))
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage);
            return false;
        }

        try
        {
            await managedDocumentOpener.OpenAsync(document.RelativePath, cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage);
            return false;
        }
    }

    public async Task<bool> OpenInsurancePolicyDocumentHistoryAsync(
        InsurancePolicyDocumentHistoryItemViewModel historyItem,
        CancellationToken cancellationToken = default)
    {
        if (managedDocumentOpener is null
            || historyItem is null
            || !InsurancePolicyDocumentHistory.Contains(historyItem))
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage);
            return false;
        }

        try
        {
            await managedDocumentOpener.OpenAsync(historyItem.RelativePath, cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentOpenFailedMessage);
            return false;
        }
    }

    public async Task<bool> UnlinkInsurancePolicyDocumentAsync(
        string documentType,
        CancellationToken cancellationToken = default)
    {
        if (documentStorageService is null
            || editingInsurancePolicyId is null
            || !activeInsuranceDocumentsByType.ContainsKey(documentType))
        {
            return false;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var disabledCount = await documentStorageService.DisableActivePolicyDocumentsByTypeAsync(
                editingInsurancePolicyId,
                documentType,
                DateTimeOffset.UtcNow,
                cancellationToken);
            if (disabledCount == 0)
            {
                return false;
            }

            await LoadInsurancePolicyDocumentStatusesAsync(
                editingInsurancePolicyId,
                cancellationToken);
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentUnlinkedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentUnlinkFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> LoadInsurancePoliciesAsync(
        CancellationToken cancellationToken = default)
    {
        if (familyMemberStorageService is null)
        {
            return false;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            await SetInsurancePolicyDataAsync(policies, cancellationToken);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyLoadFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> PrepareInsurancePolicyCreateAsync(
        CancellationToken cancellationToken = default)
    {
        if (familyMemberStorageService is null)
        {
            return false;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            AvailablePolicyFamilyMembers =
                await familyMemberStorageService.GetActiveFamilyMembersAsync(cancellationToken);
            ResetInsurancePolicyEditor();
            InsuranceEnrollmentDate = DateTime.Today;
            InsuranceRegistrationSource = InsurancePolicyValues.RegistrationSourceDirectInput;
            InsurancePolicyMessage = null;
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyLoadFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> PrepareInsurancePolicyEditAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (familyMemberStorageService is null)
        {
            return false;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var policy = await policyClaimStorageService.GetPolicyAsync(id, cancellationToken);
            if (policy is null)
            {
                InsurancePolicyMessage = uiTextProvider.Get(
                    UiTextKeys.ProductInsurancePolicyTargetUnavailableMessage);
                return false;
            }

            var allFamilyMembers =
                await familyMemberStorageService.GetFamilyMembersAsync(cancellationToken);
            var linkedFamilyExists = policy.FamilyMemberId is not null
                && allFamilyMembers.Any(member => string.Equals(
                    member.Id,
                    policy.FamilyMemberId,
                    StringComparison.Ordinal));
            AvailablePolicyFamilyMembers = allFamilyMembers
                .Where(member => member.DisabledAt is null
                    || string.Equals(member.Id, policy.FamilyMemberId, StringComparison.Ordinal))
                .ToList();

            editingInsurancePolicyId = policy.Id;
            SelectedPolicyId = policy.Id;
            InsuranceDisplayTitle = policy.DisplayTitle;
            SelectedInsuranceFamilyMemberId = linkedFamilyExists
                ? policy.FamilyMemberId
                : null;
            InsuranceInsurerName = policy.InsurerName;
            SetInsuranceContractStatus(policy.ContractStatus);
            InsuranceEnrollmentDate = policy.EnrollmentDate?.ToDateTime(TimeOnly.MinValue);
            InsuranceCoveragePeriod = policy.CoveragePeriod;
            InsurancePremiumPaymentPeriod = policy.PremiumPaymentPeriod;
            InsuranceTotalPlannedPremiumAmountText = policy.TotalPlannedPremiumAmount?.ToString(
                "N0",
                CultureInfo.InvariantCulture);
            InsuranceRenewalType = GetAllowedValue(policy.RenewalType, InsurancePolicyValues.RenewalTypes);
            InsuranceRefundType = GetAllowedValue(policy.RefundType, InsurancePolicyValues.RefundTypes);
            InsuranceBusinessType = GetAllowedValue(
                policy.InsuranceBusinessType,
                InsurancePolicyValues.BusinessTypes);
            InsuranceProductCategory = GetAllowedValue(
                policy.ProductCategory,
                InsurancePolicyValues.ProductCategories);
            InsuranceRegistrationSource = GetRegistrationSourceDisplay(policy.RegistrationSource);
            InsurancePolicyMessage = linkedFamilyExists
                ? null
                : uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyFamilyUnavailableMessage);
            await LoadInsurancePolicyDocumentStatusesAsync(policy.Id, cancellationToken);
            OnInsurancePolicyEditorModeChanged();
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<PolicyRecord?> SaveInsurancePolicyAsync(
        CancellationToken cancellationToken = default)
    {
        if (familyMemberStorageService is null)
        {
            return null;
        }

        await operationGate.WaitAsync(cancellationToken);
        try
        {
            if (!TryCreateInsurancePolicyDraft(out var draft))
            {
                InsurancePolicyMessage = uiTextProvider.Get(
                    UiTextKeys.ProductInsurancePolicyRequiredFieldsMessage);
                return null;
            }

            var saved = editingInsurancePolicyId is null
                ? await policyClaimStorageService.CreateInsurancePolicyAsync(draft, cancellationToken)
                : await policyClaimStorageService.UpdateInsurancePolicyAsync(
                    editingInsurancePolicyId,
                    draft,
                    cancellationToken);

            editingInsurancePolicyId = saved.Id;
            SelectedPolicyId = saved.Id;
            SelectedPolicyForClaimId = saved.Id;
            var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            await SetInsurancePolicyDataAsync(policies, cancellationToken);
            InsurancePolicyMessage = uiTextProvider.Get(UiTextKeys.ProductInsurancePolicySavedMessage);
            OnInsurancePolicyEditorModeChanged();
            return saved;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyOperationFailedMessage);
            return null;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> DisableInsurancePolicyAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var activeClaims = await policyClaimStorageService.GetClaimsByPolicyIdAsync(
                id,
                cancellationToken);
            if (activeClaims.Count > 0)
            {
                InsurancePolicyMessage = uiTextProvider.Get(
                    UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims);
                return false;
            }

            await policyClaimStorageService.DisablePolicyAsync(id, cancellationToken);
            var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            await SetInsurancePolicyDataAsync(policies, cancellationToken);
            InsurancePolicyMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementMessageDisabled);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            InsurancePolicyMessage = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            return await LoadCoreAsync(cancellationToken);
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> CreatePolicyAsync(CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var title = NormalizeOptionalTitle(NewPolicyDisplayTitle);
            if (title is null)
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementValidationTitleRequired);
                return false;
            }

            var activePolicies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            if (activePolicies.Any(policy => string.Equals(
                    policy.DisplayTitle,
                    title,
                    StringComparison.OrdinalIgnoreCase)))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductPolicyContractsDuplicateTitleMessage);
                return false;
            }

            var policy = await policyClaimStorageService.AddPolicyAsync(
                new PolicyDraft(title, DateOnly.FromDateTime(DateTime.Today)),
                cancellationToken);

            NewPolicyDisplayTitle = null;
            if (await LoadCoreAsync(cancellationToken))
            {
                SelectedPolicyId = policy.Id;
                SelectedPolicyForClaimId = policy.Id;
                ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementMessageCreated);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductPolicyContractsOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> CreateClaimAsync(CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            var title = NormalizeOptionalTitle(NewClaimDisplayTitle);
            if (title is null)
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationTitleRequired);
                return false;
            }

            var activePolicies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(SelectedPolicyForClaimId)
                || !activePolicies.Any(policy => string.Equals(
                    policy.Id,
                    SelectedPolicyForClaimId,
                    StringComparison.Ordinal)))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationSelectPolicyBeforeCreate);
                return false;
            }

            var activeClaims = await policyClaimStorageService.GetClaimsAsync(cancellationToken);
            if (activeClaims.Any(claim => string.Equals(
                    claim.DisplayTitle,
                    title,
                    StringComparison.OrdinalIgnoreCase)))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCasesDuplicateTitleMessage);
                return false;
            }

            var claim = await policyClaimStorageService.AddClaimAsync(
                new ClaimDraft(
                    SelectedPolicyForClaimId,
                    title,
                    DateOnly.FromDateTime(DateTime.Today)),
                cancellationToken);

            NewClaimDisplayTitle = null;
            if (await LoadCoreAsync(cancellationToken))
            {
                SelectedClaimId = claim.Id;
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementMessageCreated);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> DisableSelectedPolicyAsync(CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            if (string.IsNullOrWhiteSpace(SelectedPolicyId))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementValidationSelectPolicyTarget);
                return false;
            }

            var activeClaims = await policyClaimStorageService.GetClaimsByPolicyIdAsync(
                SelectedPolicyId,
                cancellationToken);
            if (activeClaims.Count > 0)
            {
                ManagementMessage =
                    uiTextProvider.Get(UiTextKeys.PolicyManagementValidationDisableBlockedByActiveClaims);
                return false;
            }

            await policyClaimStorageService.DisablePolicyAsync(SelectedPolicyId, cancellationToken);
            if (await LoadCoreAsync(cancellationToken))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.PolicyManagementMessageDisabled);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductPolicyContractsOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> DisableSelectedClaimAsync(CancellationToken cancellationToken = default)
    {
        await operationGate.WaitAsync(cancellationToken);
        try
        {
            if (string.IsNullOrWhiteSpace(SelectedClaimId))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementValidationSelectClaimTarget);
                return false;
            }

            await policyClaimStorageService.DisableClaimAsync(SelectedClaimId, cancellationToken);
            if (await LoadCoreAsync(cancellationToken))
            {
                ManagementMessage = uiTextProvider.Get(UiTextKeys.ClaimManagementMessageDisabled);
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    private async Task<bool> LoadCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
            var claims = await policyClaimStorageService.GetClaimsAsync(cancellationToken);

            AvailablePolicies = policies.ToList();
            AvailableClaims = claims.ToList();

            if (familyMemberStorageService is not null)
            {
                await SetInsurancePolicyDataAsync(policies, cancellationToken);
            }

            if (!AvailablePolicies.Any(policy => string.Equals(
                    policy.Id,
                    SelectedPolicyId,
                    StringComparison.Ordinal)))
            {
                SelectedPolicyId = null;
            }

            if (!AvailableClaims.Any(claim => string.Equals(
                    claim.Id,
                    SelectedClaimId,
                    StringComparison.Ordinal)))
            {
                SelectedClaimId = null;
            }

            if (!AvailablePolicies.Any(policy => string.Equals(
                    policy.Id,
                    SelectedPolicyForClaimId,
                    StringComparison.Ordinal)))
            {
                SelectedPolicyForClaimId = AvailablePolicies.FirstOrDefault()?.Id;
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ManagementMessage = uiTextProvider.Get(UiTextKeys.ProductManagementLoadFailedMessage);
            return false;
        }
    }

    private static string? NormalizeOptionalTitle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private async Task SetInsurancePolicyDataAsync(
        IReadOnlyList<PolicyRecord> policies,
        CancellationToken cancellationToken)
    {
        if (familyMemberStorageService is null)
        {
            return;
        }

        AvailablePolicies = policies.ToList();
        if (!AvailablePolicies.Any(policy => string.Equals(
                policy.Id,
                SelectedPolicyId,
                StringComparison.Ordinal)))
        {
            SelectedPolicyId = null;
        }

        if (!AvailablePolicies.Any(policy => string.Equals(
                policy.Id,
                SelectedPolicyForClaimId,
                StringComparison.Ordinal)))
        {
            SelectedPolicyForClaimId = AvailablePolicies.FirstOrDefault()?.Id;
        }

        var familyMembers = await familyMemberStorageService.GetFamilyMembersAsync(cancellationToken);
        var familyMembersById = familyMembers.ToDictionary(
            member => member.Id,
            StringComparer.Ordinal);
        AvailablePolicyFamilyMembers = familyMembers
            .Where(member => member.DisabledAt is null)
            .ToList();
        AvailableInsurancePolicies = policies
            .Select(policy => new InsurancePolicyListItemViewModel(
                policy,
                policy.FamilyMemberId is not null
                && familyMembersById.TryGetValue(policy.FamilyMemberId, out var familyMember)
                    ? familyMember.DisplayName
                    : uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyFamilyUnavailableValue),
                uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyUnregisteredValue),
                uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyLegacyValueReviewRequired)))
            .ToList();
    }

    private bool TryCreateInsurancePolicyDraft(out InsurancePolicyDraft draft)
    {
        if (string.IsNullOrWhiteSpace(InsuranceDisplayTitle)
            || string.IsNullOrWhiteSpace(SelectedInsuranceFamilyMemberId)
            || string.IsNullOrWhiteSpace(InsuranceInsurerName)
            || !IsAllowedValue(InsuranceContractStatus, InsurancePolicyValues.ContractStatuses)
            || InsuranceEnrollmentDate is null
            || string.IsNullOrWhiteSpace(InsuranceCoveragePeriod)
            || string.IsNullOrWhiteSpace(InsurancePremiumPaymentPeriod)
            || !TryParsePlannedPremiumAmount(out var plannedPremiumAmount)
            || !IsAllowedValue(InsuranceRenewalType, InsurancePolicyValues.RenewalTypes)
            || !IsAllowedValue(InsuranceRefundType, InsurancePolicyValues.RefundTypes)
            || !IsAllowedValue(InsuranceBusinessType, InsurancePolicyValues.BusinessTypes)
            || !IsAllowedValue(InsuranceProductCategory, InsurancePolicyValues.ProductCategories))
        {
            draft = null!;
            return false;
        }

        draft = new InsurancePolicyDraft(
            InsuranceDisplayTitle,
            SelectedInsuranceFamilyMemberId,
            InsuranceInsurerName,
            InsuranceContractStatus!,
            DateOnly.FromDateTime(InsuranceEnrollmentDate.Value),
            InsuranceCoveragePeriod,
            InsurancePremiumPaymentPeriod,
            plannedPremiumAmount,
            InsuranceRenewalType!,
            InsuranceRefundType!,
            InsuranceBusinessType!,
            InsuranceProductCategory!);
        return true;
    }

    private void ResetInsurancePolicyEditor()
    {
        editingInsurancePolicyId = null;
        InsuranceDisplayTitle = null;
        SelectedInsuranceFamilyMemberId = null;
        InsuranceInsurerName = null;
        InsuranceContractStatus = null;
        InsuranceEnrollmentDate = null;
        InsuranceCoveragePeriod = null;
        InsurancePremiumPaymentPeriod = null;
        InsuranceTotalPlannedPremiumAmountText = null;
        InsuranceRenewalType = null;
        InsuranceRefundType = null;
        InsuranceBusinessType = null;
        InsuranceProductCategory = null;
        InsuranceRegistrationSource = null;
        SetInsurancePolicyDocumentStatuses(
            uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyDocumentCreateStatus));
        insuranceContractStatusNeedsReview = false;
        OnPropertyChanged(nameof(InsuranceContractStatusGuidance));
        OnInsurancePolicyEditorModeChanged();
    }

    private async Task LoadInsurancePolicyDocumentStatusesAsync(
        string policyId,
        CancellationToken cancellationToken)
    {
        if (documentStorageService is null)
        {
            SetInsurancePolicyDocumentStatuses(
                uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus));
            return;
        }

        try
        {
            var links = (await documentStorageService.GetPolicyDocumentsAsync(
                    policyId,
                    cancellationToken))
                .OrderByDescending(link => link.CreatedAt)
                .ToList();
            var documents = new Dictionary<string, DocumentRecord>(StringComparer.Ordinal);
            foreach (var documentId in links.Select(link => link.DocumentId).Distinct(StringComparer.Ordinal))
            {
                var document = await documentStorageService.GetDocumentByIdAsync(
                    documentId,
                    cancellationToken);
                if (document is not null)
                {
                    documents[document.Id] = document;
                }
            }

            var emptyStatus = uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentNotRegisteredStatus);
            ClearActiveInsuranceDocuments();
            var currentLinkIds = new HashSet<string>(StringComparer.Ordinal);
            InsuranceCaptureDocumentStatus = ProjectActiveDocument(
                links,
                documents,
                CaptureDocumentType,
                emptyStatus,
                currentLinkIds);
            InsurancePolicyDocumentStatus = ProjectActiveDocument(
                links,
                documents,
                PolicyDocumentType,
                emptyStatus,
                currentLinkIds);
            InsuranceTermsDocumentStatus = ProjectActiveDocument(
                links,
                documents,
                TermsDocumentType,
                emptyStatus,
                currentLinkIds);
            InsurancePolicyDocumentHistory = BuildInsurancePolicyDocumentHistory(
                links,
                documents,
                currentLinkIds);
            OnInsuranceDocumentAvailabilityChanged();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            SetInsurancePolicyDocumentStatuses(
                uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyDocumentStatusUnavailable));
        }
    }

    private string ProjectActiveDocument(
        IReadOnlyList<PolicyDocumentRecord> links,
        IReadOnlyDictionary<string, DocumentRecord> documents,
        string documentType,
        string emptyStatus,
        ISet<string> currentLinkIds)
    {
        var link = links.FirstOrDefault(candidate =>
            candidate.DisabledAt is null
            && string.Equals(candidate.DocumentType, documentType, StringComparison.Ordinal)
            && documents.TryGetValue(candidate.DocumentId, out var document)
            && document.DisabledAt is null);
        if (link is null || !documents.TryGetValue(link.DocumentId, out var document))
        {
            return emptyStatus;
        }

        currentLinkIds.Add(link.Id);
        activeInsuranceDocumentsByType[documentType] = document;
        return string.IsNullOrWhiteSpace(document.DisplayTitle)
            ? emptyStatus
            : document.DisplayTitle.Trim();
    }

    private IReadOnlyList<InsurancePolicyDocumentHistoryItemViewModel>
        BuildInsurancePolicyDocumentHistory(
            IReadOnlyList<PolicyDocumentRecord> links,
            IReadOnlyDictionary<string, DocumentRecord> documents,
            IReadOnlySet<string> currentLinkIds)
    {
        var currentStatus = uiTextProvider.Get(
            UiTextKeys.ProductInsurancePolicyDocumentHistoryCurrentStatus);
        var archivedStatus = uiTextProvider.Get(
            UiTextKeys.ProductInsurancePolicyDocumentHistoryArchivedStatus);

        return links
            .Where(link => documents.ContainsKey(link.DocumentId))
            .Select(link =>
            {
                var document = documents[link.DocumentId];
                var isCurrent = currentLinkIds.Contains(link.Id);
                return new InsurancePolicyDocumentHistoryItemViewModel(
                    GetInsurancePolicyDocumentTypeDisplayName(link.DocumentType),
                    document.DisplayTitle.Trim(),
                    link.CreatedAt.ToLocalTime().ToString(
                        "yyyy-MM-dd HH:mm",
                        CultureInfo.InvariantCulture),
                    isCurrent ? currentStatus : archivedStatus,
                    isCurrent,
                    document.RelativePath);
            })
            .ToList();
    }

    private string GetInsurancePolicyDocumentTypeDisplayName(string documentType) =>
        documentType switch
        {
            CaptureDocumentType => uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentCaptureType),
            PolicyDocumentType => uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentPolicyType),
            TermsDocumentType => uiTextProvider.Get(
                UiTextKeys.ProductInsurancePolicyDocumentTermsType),
            _ => documentType
        };

    private void SetInsurancePolicyDocumentStatuses(string status)
    {
        ClearActiveInsuranceDocuments();
        InsurancePolicyDocumentHistory = [];
        InsuranceCaptureDocumentStatus = status;
        InsurancePolicyDocumentStatus = status;
        InsuranceTermsDocumentStatus = status;
        OnInsuranceDocumentAvailabilityChanged();
    }

    private void ClearActiveInsuranceDocuments()
    {
        activeInsuranceDocumentsByType.Clear();
    }

    private void OnInsuranceDocumentAvailabilityChanged()
    {
        OnPropertyChanged(nameof(HasInsuranceCaptureDocument));
        OnPropertyChanged(nameof(HasInsurancePolicyDocument));
        OnPropertyChanged(nameof(HasInsuranceTermsDocument));
    }

    private void SetInsuranceContractStatus(string? storedValue)
    {
        var needsReview = !string.IsNullOrWhiteSpace(storedValue)
            && !string.Equals(
                storedValue,
                InsurancePolicyValues.LegacyContractStatusActive,
                StringComparison.Ordinal)
            && !InsurancePolicyValues.ContractStatuses.Contains(storedValue, StringComparer.Ordinal);
        InsuranceContractStatus = string.Equals(
            storedValue,
            InsurancePolicyValues.LegacyContractStatusActive,
            StringComparison.Ordinal)
                ? InsurancePolicyValues.ContractStatusActive
                : GetAllowedValue(storedValue, InsurancePolicyValues.ContractStatuses);
        insuranceContractStatusNeedsReview = needsReview;
        OnPropertyChanged(nameof(InsuranceContractStatusGuidance));
    }

    private string? GetSelectionGuidance(string? selectedValue)
    {
        return selectedValue is null
            ? uiTextProvider.Get(UiTextKeys.ProductInsurancePolicySelectionRequired)
            : null;
    }

    private static string? GetAllowedValue(string? value, IReadOnlyList<string> allowedValues)
    {
        return value is not null && allowedValues.Contains(value, StringComparer.Ordinal)
            ? value
            : null;
    }

    private static bool IsAllowedValue(string? value, IReadOnlyList<string> allowedValues)
    {
        return value is not null && allowedValues.Contains(value, StringComparer.Ordinal);
    }

    private string GetRegistrationSourceDisplay(string? value)
    {
        return value is not null
            && InsurancePolicyValues.RegistrationSources.Contains(value, StringComparer.Ordinal)
                ? value
                : uiTextProvider.Get(UiTextKeys.ProductInsurancePolicyUnregisteredValue);
    }

    private bool TryParsePlannedPremiumAmount(out decimal? amount)
    {
        if (string.IsNullOrWhiteSpace(InsuranceTotalPlannedPremiumAmountText))
        {
            amount = null;
            return true;
        }

        var text = InsuranceTotalPlannedPremiumAmountText.Trim();
        if ((!decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed)
                && !decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
            || parsed < 0
            || parsed != decimal.Truncate(parsed))
        {
            amount = null;
            return false;
        }

        amount = parsed;
        return true;
    }

    private void OnInsurancePolicyEditorModeChanged()
    {
        OnPropertyChanged(nameof(IsInsurancePolicyEditMode));
        OnInsurancePolicyCommandStateChanged();
    }

    private void OnInsurancePolicyCommandStateChanged()
    {
        OnPropertyChanged(nameof(CanSaveInsurancePolicy));
    }

    private bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
