using System.Windows.Input;

namespace FamilyClaimRef.App.ViewModels;

public sealed class ProductRouteCommand(
    Action<string> navigate,
    Func<string, bool> canNavigate) : ICommand
{
    private readonly Action<string> navigate =
        navigate ?? throw new ArgumentNullException(nameof(navigate));
    private readonly Func<string, bool> canNavigate =
        canNavigate ?? throw new ArgumentNullException(nameof(canNavigate));

    public event EventHandler? CanExecuteChanged
    {
        add
        {
        }
        remove
        {
        }
    }

    public bool CanExecute(object? parameter)
    {
        return parameter is string routeId && canNavigate(routeId);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not string routeId || !canNavigate(routeId))
        {
            return;
        }

        navigate(routeId);
    }
}
