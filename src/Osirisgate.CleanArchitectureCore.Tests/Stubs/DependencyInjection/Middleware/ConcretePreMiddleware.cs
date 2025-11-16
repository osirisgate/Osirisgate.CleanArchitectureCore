using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ConcretePreMiddleware : IPreMiddleware
{
    public Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        return next(cancellationToken);
    }
}
