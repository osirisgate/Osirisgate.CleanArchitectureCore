using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class LogAPostMiddleware : IPostMiddleware
{
    public async Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (presenter != null)
        {
            presenter.GetResponse()?.UpdateField("post_classA", nameof(LogAPostMiddleware));
        }

        await next(cancellationToken);
    }
}
