using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Middleware;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class LogBPreMiddleware : IPreMiddleware
{
    public async Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request != null)
        {
            var modifiedPayload = request.GetModifiedPayload();
            modifiedPayload.Add("pre_classB", nameof(LogBPreMiddleware));
            request.ModifiedPayload(modifiedPayload);
        }

        await next(cancellationToken);
    }
}
