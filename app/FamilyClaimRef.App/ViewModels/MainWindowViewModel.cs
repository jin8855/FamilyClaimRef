namespace FamilyClaimRef.App.ViewModels;

public sealed class MainWindowViewModel
{
    public MainWindowViewModel(
        DocumentRegistrationViewModel documentRegistration,
        PolicyClaimManagementViewModel policyClaimManagement)
    {
        DocumentRegistration = documentRegistration
            ?? throw new ArgumentNullException(nameof(documentRegistration));
        PolicyClaimManagement = policyClaimManagement
            ?? throw new ArgumentNullException(nameof(policyClaimManagement));
    }

    public DocumentRegistrationViewModel DocumentRegistration { get; }

    public PolicyClaimManagementViewModel PolicyClaimManagement { get; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await DocumentRegistration.LoadTargetOptionsAsync(cancellationToken);
        await PolicyClaimManagement.LoadAsync(cancellationToken);
    }

    public async Task SelectFileAsync(CancellationToken cancellationToken = default)
    {
        await DocumentRegistration.SelectFileAsync(cancellationToken);
    }

    public async Task RegisterAsync(CancellationToken cancellationToken = default)
    {
        await DocumentRegistration.RegisterAsync(cancellationToken);
    }

    public async Task CreatePolicyAsync(CancellationToken cancellationToken = default)
    {
        if (await PolicyClaimManagement.CreatePolicyAsync(cancellationToken))
        {
            await DocumentRegistration.LoadTargetOptionsAsync(cancellationToken);
        }
    }

    public async Task CreateClaimAsync(CancellationToken cancellationToken = default)
    {
        if (await PolicyClaimManagement.CreateClaimAsync(cancellationToken))
        {
            await DocumentRegistration.LoadTargetOptionsAsync(cancellationToken);
        }
    }

    public async Task DisableSelectedPolicyAsync(CancellationToken cancellationToken = default)
    {
        if (await PolicyClaimManagement.DisableSelectedPolicyAsync(cancellationToken))
        {
            await DocumentRegistration.LoadTargetOptionsAsync(cancellationToken);
        }
    }

    public async Task DisableSelectedClaimAsync(CancellationToken cancellationToken = default)
    {
        if (await PolicyClaimManagement.DisableSelectedClaimAsync(cancellationToken))
        {
            await DocumentRegistration.LoadTargetOptionsAsync(cancellationToken);
        }
    }
}
