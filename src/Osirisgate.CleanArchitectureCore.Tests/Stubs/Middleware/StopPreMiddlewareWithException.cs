using Osirisgate.CleanArchitectureCore.Exception;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class StopPreMiddlewareWithException : IPreMiddleware
{
    public Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new BadRequestContentException(errors: new Dictionary<string, object>
        {
            ["message"] = "stop.pre.without.response",
            ["details"] = new Dictionary<string, object>
            {
                ["class"] = nameof(StopPreMiddlewareWithException),
            }
        });
    }
}
