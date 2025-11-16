using Osirisgate.CleanArchitectureCore.Attributes.Type;
using PresenterBase = Osirisgate.CleanArchitectureCore.Presenter.Presenter;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Presenter;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
/// <summary>
/// This class specifies an interface in the attribute but does not implement it.
/// Used to test the path if (!componentImplementsInterface) in ValidateInterfaceHasImplementation.
/// </summary>
[Presenter(AsInterface = typeof(IPresenterWithoutImplementation))]
public sealed class PresenterWithoutInterfaceImplementation : PresenterBase
{
    public string GetValue() => "test";
}

