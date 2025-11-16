using Osirisgate.CleanArchitectureCore.Attributes.Type;
using PresenterBase = Osirisgate.CleanArchitectureCore.Presenter.Presenter;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Presenter;

/// <summary>
/// Specific stub for testing conflicts with multiple implementations.
/// Without environment attributes to always be scanned.
/// </summary>
[Presenter(AsInterface = typeof(IMultiplePresenterInterface))]
public sealed class ConflictTestFirstPresenter : PresenterBase, IMultiplePresenterInterface
{
    public string GetName() => "ConflictTestFirst";
}

