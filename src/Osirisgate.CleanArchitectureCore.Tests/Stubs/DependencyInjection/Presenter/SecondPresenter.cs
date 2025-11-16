using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using PresenterBase = Osirisgate.CleanArchitectureCore.Presenter.Presenter;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Presenter;

[WhenProd]
[Presenter(AsInterface = typeof(IMultiplePresenterInterface))]
public sealed class SecondPresenter : PresenterBase, IMultiplePresenterInterface
{
    public string GetName() => "Second";
}

