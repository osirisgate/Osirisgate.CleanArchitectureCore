using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
/// <summary>
/// This class specifies an interface in the attribute but does not implement it.
/// Used to test the path if (!componentImplementsInterface) in ValidateInterfaceHasImplementation.
/// </summary>
[Middleware(AsInterface = typeof(IMiddlewareWithoutImplementation))]
public sealed class MiddlewareWithoutInterfaceImplementation : IPreMiddleware
{
    public Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }

    public string GetValue() => "test";
}

