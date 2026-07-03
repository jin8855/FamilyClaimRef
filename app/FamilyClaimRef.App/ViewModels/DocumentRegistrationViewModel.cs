using System.ComponentModel;
using System.Runtime.CompilerServices;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Storage;
using FamilyClaimRef.App.Services.UI;

namespace FamilyClaimRef.App.ViewModels;

public sealed class DocumentRegistrationViewModel : INotifyPropertyChanged
{
    public const string ClaimTargetKind = "claim";
    public const string PolicyTargetKind = "policy";

    private const string CleanupFailureMessage =
        "등록 중 일부 정리가 실패했습니다. 다시 시도하거나 관리자에게 문의하세요.";
    private const string NoActiveClaimMessage = "No active claim is available for selection.";
    private const string NoActivePolicyMessage = "No active policy is available for selection.";
    private const string RegistrationFailureMessage = "문서 등록에 실패했습니다.";
    private const string RegistrationSuccessMessage = "문서 등록이 완료되었습니다.";
    private const string SelectClaimMessage = "Select a claim before registering this document.";
    private const string SelectPolicyMessage = "Select a policy before registering this document.";

    private readonly DocumentRegistrationWorkflow registrationWorkflow;
    private readonly IFilePickerService filePickerService;
    private readonly IPolicyClaimStorageService policyClaimStorageService;

    private IReadOnlyList<PolicyRecord> availablePolicies = [];
    private IReadOnlyList<ClaimRecord> availableClaims = [];
    private string? selectedSourceFilePath;
    private string? selectedSourceFileDisplayName;
    private string targetKind = PolicyTargetKind;
    private string? targetId;
    private string? selectedPolicyId;
    private string? selectedClaimId;
    private string? documentType;
    private string? displayTitle;
    private DateOnly referenceDate = DateOnly.FromDateTime(DateTime.Today);
    private bool isBusy;
    private bool targetOptionsLoaded;
    private string? validationMessage;
    private string? statusMessage;
    private string? targetSelectionMessage;
    private string? lastRegistrationSummary;

    public DocumentRegistrationViewModel(
        DocumentRegistrationWorkflow registrationWorkflow,
        IFilePickerService filePickerService,
        IPolicyClaimStorageService policyClaimStorageService)
    {
        this.registrationWorkflow = registrationWorkflow
            ?? throw new ArgumentNullException(nameof(registrationWorkflow));
        this.filePickerService = filePickerService
            ?? throw new ArgumentNullException(nameof(filePickerService));
        this.policyClaimStorageService = policyClaimStorageService
            ?? throw new ArgumentNullException(nameof(policyClaimStorageService));
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

    public bool HasAvailablePolicies => AvailablePolicies.Count > 0;

    public bool HasAvailableClaims => AvailableClaims.Count > 0;

    public string? SelectedSourceFilePath
    {
        get => selectedSourceFilePath;
        set => SetProperty(ref selectedSourceFilePath, value);
    }

    public string? SelectedSourceFileDisplayName
    {
        get => selectedSourceFileDisplayName;
        set => SetProperty(ref selectedSourceFileDisplayName, value);
    }

    public string TargetKind
    {
        get => targetKind;
        set
        {
            if (SetProperty(ref targetKind, value))
            {
                ApplySelectedTargetId();
                RefreshTargetSelectionMessage();
            }
        }
    }

    public string? TargetId
    {
        get => targetId;
        set => SetProperty(ref targetId, value);
    }

    public string? SelectedPolicyId
    {
        get => selectedPolicyId;
        set
        {
            if (SetProperty(ref selectedPolicyId, value))
            {
                if (string.Equals(TargetKind, PolicyTargetKind, StringComparison.Ordinal))
                {
                    TargetId = value;
                }

                RefreshTargetSelectionMessage();
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
                if (string.Equals(TargetKind, ClaimTargetKind, StringComparison.Ordinal))
                {
                    TargetId = value;
                }

                RefreshTargetSelectionMessage();
            }
        }
    }

    public string? DocumentType
    {
        get => documentType;
        set => SetProperty(ref documentType, value);
    }

    public string? DisplayTitle
    {
        get => displayTitle;
        set => SetProperty(ref displayTitle, value);
    }

    public DateOnly ReferenceDate
    {
        get => referenceDate;
        set => SetProperty(ref referenceDate, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public string? ValidationMessage
    {
        get => validationMessage;
        private set => SetProperty(ref validationMessage, value);
    }

    public string? StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    public string? TargetSelectionMessage
    {
        get => targetSelectionMessage;
        private set => SetProperty(ref targetSelectionMessage, value);
    }

    public string? LastRegistrationSummary
    {
        get => lastRegistrationSummary;
        private set => SetProperty(ref lastRegistrationSummary, value);
    }

    public async Task LoadTargetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var policies = await policyClaimStorageService.GetPoliciesAsync(cancellationToken);
        var claims = await policyClaimStorageService.GetClaimsAsync(cancellationToken);

        targetOptionsLoaded = true;
        AvailablePolicies = policies.ToList();
        AvailableClaims = claims.ToList();

        if (!AvailablePolicies.Any(policy => string.Equals(policy.Id, SelectedPolicyId, StringComparison.Ordinal)))
        {
            SelectedPolicyId = null;
        }

        if (!AvailableClaims.Any(claim => string.Equals(claim.Id, SelectedClaimId, StringComparison.Ordinal)))
        {
            SelectedClaimId = null;
        }

        ApplySelectedTargetId();
        RefreshTargetSelectionMessage();
    }

    public async Task SelectFileAsync(CancellationToken cancellationToken = default)
    {
        var result = await filePickerService.PickDocumentFileAsync(cancellationToken);
        if (result is null)
        {
            return;
        }

        SelectedSourceFilePath = result.SourceFilePath;
        SelectedSourceFileDisplayName = result.SafeDisplayName;
        ValidationMessage = null;
        StatusMessage = "파일을 선택했습니다.";
    }

    public async Task RegisterAsync(CancellationToken cancellationToken = default)
    {
        if (!Validate())
        {
            return;
        }

        IsBusy = true;
        ValidationMessage = null;
        StatusMessage = null;

        try
        {
            if (string.Equals(TargetKind, PolicyTargetKind, StringComparison.Ordinal))
            {
                var result = await registrationWorkflow.RegisterPolicyDocumentAsync(
                    new PolicyDocumentRegistrationRequest(
                        SelectedSourceFilePath!,
                        TargetId!,
                        DocumentType!,
                        DisplayTitle!,
                        ReferenceDate),
                    cancellationToken);

                LastRegistrationSummary = CreatePolicySummary(result);
            }
            else
            {
                var result = await registrationWorkflow.RegisterClaimDocumentAsync(
                    new ClaimDocumentRegistrationRequest(
                        SelectedSourceFilePath!,
                        TargetId!,
                        DocumentType!,
                        DisplayTitle!,
                        ReferenceDate),
                    cancellationToken);

                LastRegistrationSummary = CreateClaimSummary(result);
            }

            StatusMessage = RegistrationSuccessMessage;
            ValidationMessage = null;
        }
        catch (AggregateException)
        {
            StatusMessage = CleanupFailureMessage;
        }
        catch (Exception)
        {
            StatusMessage = RegistrationFailureMessage;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(SelectedSourceFilePath))
        {
            ValidationMessage = "파일을 선택해 주세요.";
            StatusMessage = null;
            return false;
        }

        if (!IsSupportedTargetKind(TargetKind))
        {
            ValidationMessage = "저장할 대상 유형을 선택해 주세요.";
            StatusMessage = null;
            return false;
        }

        if (!ValidateTargetSelection())
        {
            StatusMessage = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(TargetId))
        {
            ValidationMessage = "저장할 대상을 입력해 주세요.";
            StatusMessage = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(DocumentType))
        {
            ValidationMessage = "문서 유형을 선택해 주세요.";
            StatusMessage = null;
            return false;
        }

        if (string.IsNullOrWhiteSpace(DisplayTitle))
        {
            ValidationMessage = "표시 제목을 입력해 주세요.";
            StatusMessage = null;
            return false;
        }

        if (ReferenceDate == default)
        {
            ValidationMessage = "기준일을 선택해 주세요.";
            StatusMessage = null;
            return false;
        }

        return true;
    }

    private bool ValidateTargetSelection()
    {
        if (!targetOptionsLoaded)
        {
            return true;
        }

        if (string.Equals(TargetKind, PolicyTargetKind, StringComparison.Ordinal))
        {
            if (!HasAvailablePolicies)
            {
                ValidationMessage = NoActivePolicyMessage;
                return false;
            }

            if (string.IsNullOrWhiteSpace(TargetId))
            {
                ValidationMessage = SelectPolicyMessage;
                return false;
            }
        }

        if (string.Equals(TargetKind, ClaimTargetKind, StringComparison.Ordinal))
        {
            if (!HasAvailableClaims)
            {
                ValidationMessage = NoActiveClaimMessage;
                return false;
            }

            if (string.IsNullOrWhiteSpace(TargetId))
            {
                ValidationMessage = SelectClaimMessage;
                return false;
            }
        }

        return true;
    }

    private void ApplySelectedTargetId()
    {
        if (string.Equals(TargetKind, PolicyTargetKind, StringComparison.Ordinal))
        {
            TargetId = SelectedPolicyId;
            return;
        }

        if (string.Equals(TargetKind, ClaimTargetKind, StringComparison.Ordinal))
        {
            TargetId = SelectedClaimId;
            return;
        }

        TargetId = null;
    }

    private void RefreshTargetSelectionMessage()
    {
        if (!targetOptionsLoaded)
        {
            TargetSelectionMessage = null;
            return;
        }

        if (string.Equals(TargetKind, PolicyTargetKind, StringComparison.Ordinal))
        {
            TargetSelectionMessage = HasAvailablePolicies ? null : NoActivePolicyMessage;
            return;
        }

        if (string.Equals(TargetKind, ClaimTargetKind, StringComparison.Ordinal))
        {
            TargetSelectionMessage = HasAvailableClaims ? null : NoActiveClaimMessage;
            return;
        }

        TargetSelectionMessage = null;
    }

    private static bool IsSupportedTargetKind(string value)
    {
        return string.Equals(value, PolicyTargetKind, StringComparison.Ordinal)
            || string.Equals(value, ClaimTargetKind, StringComparison.Ordinal);
    }

    private static string CreatePolicySummary(PolicyDocumentRegistrationResult result)
    {
        return $"policy:{result.Link.PolicyDocument.PolicyId}; document:{result.Attachment.Document.Id}";
    }

    private static string CreateClaimSummary(ClaimDocumentRegistrationResult result)
    {
        return $"claim:{result.Link.ClaimDocument.ClaimId}; document:{result.Attachment.Document.Id}";
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
