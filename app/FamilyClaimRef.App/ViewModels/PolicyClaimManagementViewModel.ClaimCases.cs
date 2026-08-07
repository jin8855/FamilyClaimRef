using System.Globalization;
using FamilyClaimRef.App.Models.Storage;
using FamilyClaimRef.App.Services.Localization;
using FamilyClaimRef.App.Services.Storage;

namespace FamilyClaimRef.App.ViewModels;

public sealed partial class PolicyClaimManagementViewModel
{
    private readonly IClaimCaseStorageService? claimCaseStorageService;

    private IReadOnlyList<FamilyMemberRecord> availableClaimFamilyMembers = [];
    private string? claimCaseDisplayTitle;
    private string? selectedClaimFamilyMemberId;
    private DateTime? claimTreatmentDate = DateTime.Today;
    private string? claimHospitalName;
    private string? claimDiagnosisCode;
    private string? claimDiagnosisName;
    private string? claimVisitType = ClaimCaseValues.VisitTypeOutpatient;
    private bool claimHasSurgery;
    private bool claimHasPrescription;
    private string? claimCoveredAmountText;
    private string? claimNonCoveredAmountText;
    private string? claimPrescriptionAmountText;
    private string? claimMemo;
    private int? editingClaimCaseRevision;
    private bool claimCaseLegacyOwnershipBlocked;
    private string? claimCaseValidationMessage;
    private string? claimCaseConflictMessage;
    private string? claimCaseLegacyReviewMessage;
    private string? claimCaseOperationMessage;
    private IReadOnlyList<ClaimCaseVisitTypeOptionViewModel>? claimCaseVisitTypeOptions;

    public IReadOnlyList<FamilyMemberRecord> AvailableClaimFamilyMembers
    {
        get => availableClaimFamilyMembers;
        private set => SetProperty(ref availableClaimFamilyMembers, value);
    }

    public IReadOnlyList<ClaimCaseVisitTypeOptionViewModel> ClaimCaseVisitTypeOptions =>
        claimCaseVisitTypeOptions ??=
        [
            new ClaimCaseVisitTypeOptionViewModel(
                ClaimCaseValues.VisitTypeOutpatient,
                uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeOutpatient)),
            new ClaimCaseVisitTypeOptionViewModel(
                ClaimCaseValues.VisitTypeInpatient,
                uiTextProvider.Get(UiTextKeys.ProductClaimCaseVisitTypeInpatient))
        ];

    public string? ClaimCaseDisplayTitle
    {
        get => claimCaseDisplayTitle;
        set
        {
            if (SetProperty(ref claimCaseDisplayTitle, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? SelectedClaimFamilyMemberId
    {
        get => selectedClaimFamilyMemberId;
        set
        {
            if (SetProperty(ref selectedClaimFamilyMemberId, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public DateTime? ClaimTreatmentDate
    {
        get => claimTreatmentDate;
        set
        {
            if (SetProperty(ref claimTreatmentDate, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimHospitalName
    {
        get => claimHospitalName;
        set
        {
            if (SetProperty(ref claimHospitalName, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimDiagnosisCode
    {
        get => claimDiagnosisCode;
        set => SetProperty(ref claimDiagnosisCode, value);
    }

    public string? ClaimDiagnosisName
    {
        get => claimDiagnosisName;
        set => SetProperty(ref claimDiagnosisName, value);
    }

    public string? ClaimVisitType
    {
        get => claimVisitType;
        set
        {
            if (SetProperty(ref claimVisitType, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public bool ClaimHasSurgery
    {
        get => claimHasSurgery;
        set => SetProperty(ref claimHasSurgery, value);
    }

    public bool ClaimHasPrescription
    {
        get => claimHasPrescription;
        set => SetProperty(ref claimHasPrescription, value);
    }

    public string? ClaimCoveredAmountText
    {
        get => claimCoveredAmountText;
        set
        {
            if (SetProperty(ref claimCoveredAmountText, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimNonCoveredAmountText
    {
        get => claimNonCoveredAmountText;
        set
        {
            if (SetProperty(ref claimNonCoveredAmountText, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimPrescriptionAmountText
    {
        get => claimPrescriptionAmountText;
        set
        {
            if (SetProperty(ref claimPrescriptionAmountText, value))
            {
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimMemo
    {
        get => claimMemo;
        set => SetProperty(ref claimMemo, value);
    }

    public int? EditingClaimCaseRevision
    {
        get => editingClaimCaseRevision;
        private set
        {
            if (SetProperty(ref editingClaimCaseRevision, value))
            {
                OnPropertyChanged(nameof(IsClaimCaseEditMode));
                OnClaimCaseCommandStateChanged();
            }
        }
    }

    public string? ClaimCaseValidationMessage
    {
        get => claimCaseValidationMessage;
        private set => SetProperty(ref claimCaseValidationMessage, value);
    }

    public string? ClaimCaseConflictMessage
    {
        get => claimCaseConflictMessage;
        private set => SetProperty(ref claimCaseConflictMessage, value);
    }

    public string? ClaimCaseLegacyReviewMessage
    {
        get => claimCaseLegacyReviewMessage;
        private set => SetProperty(ref claimCaseLegacyReviewMessage, value);
    }

    public string? ClaimCaseOperationMessage
    {
        get => claimCaseOperationMessage;
        private set => SetProperty(ref claimCaseOperationMessage, value);
    }

    public bool IsClaimCaseEditMode => EditingClaimCaseRevision is not null;

    public bool CanCreateClaimCase =>
        !IsClaimCaseEditMode
        && claimCaseStorageService is not null
        && TryCreateClaimCaseDraft(out _);

    public bool CanSaveClaimCase =>
        IsClaimCaseEditMode
        && !claimCaseLegacyOwnershipBlocked
        && claimCaseStorageService is not null
        && TryCreateClaimCaseDraft(out _);

    public bool CanDisableClaimCase =>
        IsClaimCaseEditMode
        && !claimCaseLegacyOwnershipBlocked
        && !string.IsNullOrWhiteSpace(SelectedClaimId)
        && claimCaseStorageService is not null;

    public void StartNewClaimCase()
    {
        ResetClaimCaseEditor(clearSelection: true);
        ClearClaimCaseMessages();
    }

    public void ClearClaimCaseMessages()
    {
        ClaimCaseValidationMessage = null;
        ClaimCaseConflictMessage = null;
        ClaimCaseLegacyReviewMessage = null;
        ClaimCaseOperationMessage = null;
    }

    public async Task<bool> LoadSelectedClaimCaseAsync(
        CancellationToken cancellationToken = default)
    {
        if (claimCaseStorageService is null || string.IsNullOrWhiteSpace(SelectedClaimId))
        {
            return false;
        }

        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }
        try
        {
            ClearClaimCaseMessages();
            var record = await claimCaseStorageService.GetClaimCaseAsync(
                SelectedClaimId,
                cancellationToken);
            if (record is null)
            {
                ClaimCaseOperationMessage = uiTextProvider.Get(
                    UiTextKeys.ProductClaimCasesOperationFailedMessage);
                return false;
            }

            ApplyClaimCaseRecord(record);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> CreateClaimCaseRecordAsync(
        CancellationToken cancellationToken = default)
    {
        if (claimCaseStorageService is null)
        {
            return false;
        }

        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }
        try
        {
            ClearClaimCaseMessages();
            if (!TryCreateClaimCaseDraft(out var draft))
            {
                ClaimCaseValidationMessage = uiTextProvider.Get(
                    UiTextKeys.ProductClaimCaseValidationRequiredMessage);
                return false;
            }

            var created = await claimCaseStorageService.CreateClaimCaseAsync(
                draft,
                cancellationToken);
            await RefreshAvailableClaimCasesAsync(cancellationToken);
            SelectedClaimId = created.Id;
            ApplyClaimCaseRecord(created);
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseDraftCreatedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ArgumentException)
        {
            ClaimCaseValidationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseValidationRequiredMessage);
            return false;
        }
        catch
        {
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> SaveClaimCaseAsync(
        CancellationToken cancellationToken = default)
    {
        if (claimCaseStorageService is null
            || string.IsNullOrWhiteSpace(SelectedClaimId)
            || EditingClaimCaseRevision is null)
        {
            return false;
        }

        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }
        try
        {
            ClearClaimCaseMessages();
            if (claimCaseLegacyOwnershipBlocked)
            {
                SetClaimCaseLegacyReviewMessage();
                return false;
            }

            if (!TryCreateClaimCaseDraft(out var draft))
            {
                ClaimCaseValidationMessage = uiTextProvider.Get(
                    UiTextKeys.ProductClaimCaseValidationRequiredMessage);
                return false;
            }

            var updated = await claimCaseStorageService.UpdateClaimCaseAsync(
                SelectedClaimId,
                EditingClaimCaseRevision.Value,
                draft,
                cancellationToken);
            await RefreshAvailableClaimCasesAsync(cancellationToken);
            ApplyClaimCaseRecord(updated);
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseSavedMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimCaseConcurrencyException)
        {
            ClaimCaseConflictMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseConflictMessage);
            return false;
        }
        catch (ClaimCaseLegacyReviewRequiredException)
        {
            SetClaimCaseLegacyReviewMessage();
            return false;
        }
        catch (ArgumentException)
        {
            ClaimCaseValidationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseValidationRequiredMessage);
            return false;
        }
        catch
        {
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    public async Task<bool> DisableSelectedClaimCaseAsync(
        CancellationToken cancellationToken = default)
    {
        if (claimCaseStorageService is null
            || string.IsNullOrWhiteSpace(SelectedClaimId)
            || EditingClaimCaseRevision is null)
        {
            return false;
        }

        if (!await operationGate.WaitAsync(0, cancellationToken))
        {
            return false;
        }
        try
        {
            ClearClaimCaseMessages();
            if (claimCaseLegacyOwnershipBlocked)
            {
                SetClaimCaseLegacyReviewMessage();
                return false;
            }

            await claimCaseStorageService.DisableClaimCaseAsync(
                SelectedClaimId,
                EditingClaimCaseRevision.Value,
                cancellationToken);
            await RefreshAvailableClaimCasesAsync(cancellationToken);
            ResetClaimCaseEditor(clearSelection: true);
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseDisabledMessage);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ClaimCaseConcurrencyException)
        {
            ClaimCaseConflictMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCaseConflictMessage);
            return false;
        }
        catch (ClaimCaseLegacyReviewRequiredException)
        {
            SetClaimCaseLegacyReviewMessage();
            return false;
        }
        catch
        {
            ClaimCaseOperationMessage = uiTextProvider.Get(
                UiTextKeys.ProductClaimCasesOperationFailedMessage);
            return false;
        }
        finally
        {
            operationGate.Release();
        }
    }

    private async Task LoadClaimCaseStateAsync(CancellationToken cancellationToken)
    {
        if (claimCaseStorageService is null || familyMemberStorageService is null)
        {
            return;
        }

        AvailableClaimFamilyMembers = await familyMemberStorageService
            .GetActiveFamilyMembersAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(SelectedClaimId))
        {
            if (IsClaimCaseEditMode)
            {
                ResetClaimCaseEditor(clearSelection: false);
            }

            return;
        }

        var selected = AvailableClaims.FirstOrDefault(claim => string.Equals(
            claim.Id,
            SelectedClaimId,
            StringComparison.Ordinal));
        if (selected is not null)
        {
            ApplyClaimCaseRecord(selected);
        }
    }

    private async Task RefreshAvailableClaimCasesAsync(CancellationToken cancellationToken)
    {
        if (claimCaseStorageService is not null)
        {
            AvailableClaims = (await claimCaseStorageService.GetClaimCasesAsync(cancellationToken))
                .ToList();
        }
    }

    private void ApplyClaimCaseRecord(ClaimRecord record)
    {
        SelectedClaimId = record.Id;
        EditingClaimCaseRevision = record.Revision;
        ClaimCaseDisplayTitle = record.DisplayTitle;
        SelectedClaimFamilyMemberId = record.FamilyMemberId;
        ClaimTreatmentDate = record.ReferenceDate.ToDateTime(TimeOnly.MinValue);
        ClaimHospitalName = record.HospitalName;
        ClaimDiagnosisCode = record.DiagnosisCode;
        ClaimDiagnosisName = record.DiagnosisName;
        ClaimVisitType = ClaimCaseValues.VisitTypes.Contains(
            record.VisitType,
            StringComparer.Ordinal)
            ? record.VisitType
            : ClaimCaseValues.VisitTypeOutpatient;
        ClaimHasSurgery = record.HasSurgery;
        ClaimHasPrescription = record.HasPrescription;
        ClaimCoveredAmountText = FormatClaimAmount(record.CoveredAmount);
        ClaimNonCoveredAmountText = FormatClaimAmount(record.NonCoveredAmount);
        ClaimPrescriptionAmountText = FormatClaimAmount(record.PrescriptionAmount);
        ClaimMemo = record.Memo;

        claimCaseLegacyOwnershipBlocked = string.IsNullOrWhiteSpace(record.FamilyMemberId)
            || !AvailableClaimFamilyMembers.Any(member => string.Equals(
                member.Id,
                record.FamilyMemberId,
                StringComparison.Ordinal));
        OnClaimCaseCommandStateChanged();
        if (claimCaseLegacyOwnershipBlocked)
        {
            SetClaimCaseLegacyReviewMessage();
        }
    }

    private bool TryCreateClaimCaseDraft(out ClaimCaseDraft draft)
    {
        if (string.IsNullOrWhiteSpace(ClaimCaseDisplayTitle)
            || string.IsNullOrWhiteSpace(SelectedClaimFamilyMemberId)
            || ClaimTreatmentDate is null
            || string.IsNullOrWhiteSpace(ClaimHospitalName)
            || !ClaimCaseValues.VisitTypes.Contains(ClaimVisitType, StringComparer.Ordinal)
            || !TryParseClaimAmount(ClaimCoveredAmountText, out var coveredAmount)
            || !TryParseClaimAmount(ClaimNonCoveredAmountText, out var nonCoveredAmount)
            || !TryParseClaimAmount(ClaimPrescriptionAmountText, out var prescriptionAmount))
        {
            draft = null!;
            return false;
        }

        draft = new ClaimCaseDraft(
            ClaimCaseDisplayTitle,
            SelectedClaimFamilyMemberId,
            DateOnly.FromDateTime(ClaimTreatmentDate.Value),
            ClaimHospitalName,
            ClaimDiagnosisCode,
            ClaimDiagnosisName,
            ClaimVisitType!,
            ClaimHasSurgery,
            ClaimHasPrescription,
            coveredAmount,
            nonCoveredAmount,
            prescriptionAmount,
            ClaimMemo);
        return true;
    }

    private static bool TryParseClaimAmount(string? value, out long? amount)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            amount = null;
            return true;
        }

        var styles = NumberStyles.Integer | NumberStyles.AllowThousands;
        if ((long.TryParse(value, styles, CultureInfo.CurrentCulture, out var parsed)
                || long.TryParse(value, styles, CultureInfo.InvariantCulture, out parsed))
            && parsed >= 0)
        {
            amount = parsed;
            return true;
        }

        amount = null;
        return false;
    }

    private static string? FormatClaimAmount(long? value)
    {
        return value?.ToString("N0", CultureInfo.CurrentCulture);
    }

    private void ResetClaimCaseEditor(bool clearSelection)
    {
        if (clearSelection)
        {
            SelectedClaimId = null;
        }

        claimCaseLegacyOwnershipBlocked = false;
        EditingClaimCaseRevision = null;
        ClaimCaseDisplayTitle = null;
        SelectedClaimFamilyMemberId = null;
        ClaimTreatmentDate = DateTime.Today;
        ClaimHospitalName = null;
        ClaimDiagnosisCode = null;
        ClaimDiagnosisName = null;
        ClaimVisitType = ClaimCaseValues.VisitTypeOutpatient;
        ClaimHasSurgery = false;
        ClaimHasPrescription = false;
        ClaimCoveredAmountText = null;
        ClaimNonCoveredAmountText = null;
        ClaimPrescriptionAmountText = null;
        ClaimMemo = null;
        OnClaimCaseCommandStateChanged();
    }

    private void SetClaimCaseLegacyReviewMessage()
    {
        ClaimCaseLegacyReviewMessage = uiTextProvider.Get(
            UiTextKeys.ProductClaimCaseLegacyReviewRequiredMessage);
    }

    private void OnClaimCaseCommandStateChanged()
    {
        OnPropertyChanged(nameof(CanCreateClaimCase));
        OnPropertyChanged(nameof(CanSaveClaimCase));
        OnPropertyChanged(nameof(CanDisableClaimCase));
    }
}

public sealed record ClaimCaseVisitTypeOptionViewModel(string Value, string DisplayText);
