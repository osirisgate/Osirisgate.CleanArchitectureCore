using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using PresenterBase = Osirisgate.CleanArchitectureCore.Presenter.Presenter;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Presenter;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[WhenDev]
[WhenTest]
[Presenter]
public sealed class MultiEnvironmentPresenter : PresenterBase
{
}

